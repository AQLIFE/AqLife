<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  ElAlert, ElButton, ElCard, ElIcon, ElMessage, ElProgress, ElUpload,
  type UploadFile, type UploadRawFile, type UploadUserFile,
} from 'element-plus'
import {
  ArrowLeft, UploadFilled, VideoCamera, Refresh, VideoPlay, VideoPause,
} from '@element-plus/icons-vue'
import { defaultFilePolicy } from '@aqlife/domain'
import { FileApi, type FileDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { tokenStore } from '@aqlife/api-client'
import { useFileStore } from '@/stores/useFileStore'
import TagSelect from '@/components/TagSelect.vue'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'

const router = useRouter()
const route = useRoute()
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()
const isVideoMode = computed(() => route.query.type === 'video')

const fileList = ref<UploadUserFile[]>([])
const videoFileList = ref<UploadUserFile[]>([])
const tags = ref<TagDto[]>([])
const uploading = ref(false)
const paused = ref(false)
const progress = ref(0)
const currentChunk = ref(0)
const chunkCount = ref(0)
const missingChunks = ref<number[]>([])
const uploadedBytes = ref(0)
const uploadSpeed = ref(0)
const remainingSeconds = ref<number | null>(null)
const uploadError = ref('')
const sessionId = ref('')
const videoFile = ref<UploadRawFile | null>(null)
const completed = ref(false)
let pauseRequested = false
let speedWindowStartedAt = 0
let speedWindowBytes = 0

const acceptType = computed(() => defaultFilePolicy.allowedUpload.join(','))
const canCommit = computed(() => fileList.value.length > 0 && !uploading.value)
const videoAccept = '.mp4,.webm'
const videoApiBase = `${apiConfiguration.basePath.replace(/\/+$/, '')}/api/File/uploads`
const sessionStorageKey = 'mgs-video-upload-session'

interface UploadSession {
  uploadId: string
  fileName: string
  fileSize: number
  chunkSize: number
  chunkCount: number
  createdAt?: string
}
interface UploadStatus {
  uploadId: string
  fileSize: number
  chunkSize: number
  chunkCount: number
  uploadedChunks: number[]
}

function authHeaders(): HeadersInit {
  const token = tokenStore.get()
  return token ? { Authorization: token.startsWith('Bearer ') ? token : `Bearer ${token}` } : {}
}

async function apiJson<T>(url: string, init: RequestInit = {}): Promise<T> {
  const response = await fetch(url, {
    credentials: 'include',
    ...init,
    headers: { ...authHeaders(), ...(init.headers ?? {}) },
  })
  if (!response.ok) {
    let message = `请求失败（${response.status}）`
    try {
      const body = await response.clone().json() as { error?: string; title?: string }
      message = body.error || body.title || message
    } catch { /* use status fallback */ }
    throw new Error(message)
  }
  if (response.status === 204) return undefined as T
  return await response.json() as T
}

function goBack() {
  router.push('/file/list')
}

function isVideo(file: File): boolean {
  return /\.(mp4|webm)$/i.test(file.name)
}

function onVideoFileChange(uploadFile: UploadFile) {
  const raw = uploadFile.raw
  if (!raw) return
  if (!isVideo(raw)) {
    videoFileList.value = []
    videoFile.value = null
    ElMessage.warning('仅支持 MP4 和 WebM 视频文件')
    return
  }
  videoFile.value = raw
  uploadError.value = ''
  completed.value = false
  progress.value = 0
  uploadedBytes.value = 0
  uploadSpeed.value = 0
  remainingSeconds.value = null
  sessionId.value = ''
  currentChunk.value = 0
  chunkCount.value = 0
}

function formatBytes(bytes: number): string {
  if (!Number.isFinite(bytes) || bytes < 0) return '0 B'
  const units = ['B', 'KB', 'MB', 'GB', 'TB']
  let value = bytes
  let unit = 0
  while (value >= 1024 && unit < units.length - 1) {
    value /= 1024
    unit++
  }
  return `${value.toFixed(unit === 0 ? 0 : 1)} ${units[unit]}`
}

const videoFileSize = computed(() => videoFile.value ? formatBytes(videoFile.value.size) : '')
const missingChunksText = computed(() => {
  if (missingChunks.value.length === 0) return '无'
  const shown = missingChunks.value.slice(0, 12).map(index => index + 1).join(', ')
  return missingChunks.value.length > 12 ? shown + ' 等（共 ' + missingChunks.value.length + ' 个）' : shown
})

const remainingText = computed(() => {
  if (remainingSeconds.value == null || !Number.isFinite(remainingSeconds.value)) return '计算中'
  if (remainingSeconds.value < 60) return `约 ${Math.max(1, Math.ceil(remainingSeconds.value))} 秒`
  return `约 ${Math.floor(remainingSeconds.value / 60)} 分 ${Math.ceil(remainingSeconds.value % 60)} 秒`
})

function getSavedSession(file: File): UploadSession | null {
  try {
    const saved = localStorage.getItem(sessionStorageKey)
    if (!saved) return null
    const parsed = JSON.parse(saved) as UploadSession
    return parsed.fileName === file.name && parsed.fileSize === file.size ? parsed : null
  } catch {
    return null
  }
}

function saveSession(session: UploadSession) {
  sessionId.value = session.uploadId
  localStorage.setItem(sessionStorageKey, JSON.stringify(session))
}

async function createOrResumeSession(file: File): Promise<{ session: UploadSession; status: UploadStatus }> {
  const saved = getSavedSession(file)
  if (saved) {
    try {
      const status = await apiJson<UploadStatus>(`${videoApiBase}/${saved.uploadId}`)
      return { session: saved, status }
    } catch {
      localStorage.removeItem(sessionStorageKey)
    }
  }

  const session = await apiJson<UploadSession>(videoApiBase, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ fileName: file.name, fileSize: file.size, chunkSize: 4 * 1024 * 1024 }),
  })
  saveSession(session)
  const status = await apiJson<UploadStatus>(`${videoApiBase}/${session.uploadId}`)
  return { session, status }
}

function uploadChunk(url: string, chunk: Blob, onProgress: (loaded: number) => void): Promise<void> {
  return new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest()
    xhr.open('PUT', url)
    xhr.withCredentials = true
    const token = tokenStore.get()
    if (token) xhr.setRequestHeader('Authorization', token.startsWith('Bearer ') ? token : `Bearer ${token}`)
    xhr.upload.onprogress = event => {
      if (event.lengthComputable) onProgress(event.loaded)
    }
    xhr.onload = () => {
      if (xhr.status >= 200 && xhr.status < 300) resolve()
      else {
        let message = `分片上传失败（${xhr.status}）`
        try {
          const body = JSON.parse(xhr.responseText) as { error?: string; title?: string }
          message = body.error || body.title || message
        } catch { /* keep fallback */ }
        reject(new Error(message))
      }
    }
    xhr.onerror = () => {
      reject(new Error('网络连接中断，查询服务端状态后可继续上传'))
    }
    xhr.onabort = () => {
      reject(new Error('上传请求已中断'))
    }
    const form = new FormData()
    form.append('chunk', chunk, 'chunk.part')
    xhr.send(form)
  })
}

function updateSpeed(loaded: number, chunkBaseBytes: number) {
  const now = Date.now()
  if (!speedWindowStartedAt) {
    speedWindowStartedAt = now
    speedWindowBytes = loaded
    return
  }
  const elapsed = (now - speedWindowStartedAt) / 1000
  if (elapsed >= 0.75) {
    uploadSpeed.value = Math.max(0, (loaded - speedWindowBytes) / elapsed)
    speedWindowStartedAt = now
    speedWindowBytes = loaded
  }
  const effectiveBytes = Math.min(videoFile.value?.size ?? 0, chunkBaseBytes + loaded)
  uploadedBytes.value = Math.max(uploadedBytes.value, effectiveBytes)
  progress.value = videoFile.value?.size ? Math.min(100, effectiveBytes / videoFile.value.size * 100) : 0
  remainingSeconds.value = uploadSpeed.value > 0 && videoFile.value
    ? Math.max(0, (videoFile.value.size - effectiveBytes) / uploadSpeed.value)
    : null
}

async function startVideoUpload() {
  const file = videoFile.value
  if (!file || uploading.value || completed.value) return
  if (!isVideo(file)) {
    ElMessage.warning('仅支持 MP4 和 WebM 视频文件')
    return
  }

  uploading.value = true
  paused.value = false
  pauseRequested = false
  uploadError.value = ''
  speedWindowStartedAt = 0
  speedWindowBytes = 0

  try {
    const { session, status: initialStatus } = await createOrResumeSession(file)
    saveSession(session)
    chunkCount.value = session.chunkCount
    const uploaded = new Set(initialStatus.uploadedChunks)
    missingChunks.value = Array.from({ length: session.chunkCount }, (_, index) => index)
      .filter(index => !uploaded.has(index))
    const totalUploadedBytes = () => {
      let bytes = 0
      for (const index of uploaded) {
        bytes += Math.min(session.chunkSize, file.size - index * session.chunkSize)
      }
      return bytes
    }
    uploadedBytes.value = totalUploadedBytes()
    progress.value = Math.min(100, uploadedBytes.value / file.size * 100)

    for (let index = 0; index < session.chunkCount; index++) {
      if (uploaded.has(index)) {
        currentChunk.value = index + 1
        continue
      }
      if (pauseRequested) break

      currentChunk.value = index + 1
      const start = index * session.chunkSize
      const end = Math.min(file.size, start + session.chunkSize)
      const chunk = file.slice(start, end)
      const baseBytes = totalUploadedBytes()
      await uploadChunk(`${videoApiBase}/${session.uploadId}/chunks/${index}`, chunk, loaded => {
        updateSpeed(loaded, baseBytes)
      })
      uploaded.add(index)
      missingChunks.value = missingChunks.value.filter(missingIndex => missingIndex !== index)
      uploadedBytes.value = totalUploadedBytes()
      progress.value = Math.min(100, uploadedBytes.value / file.size * 100)
    }

    if (pauseRequested) {
      paused.value = true
      ElMessage.info('已暂停；再次点击“继续上传”即可从已完成分片处恢复')
      return
    }

    if (uploaded.size !== session.chunkCount) {
      throw new Error('仍有分片未上传，请继续重试')
    }

    uploadError.value = ''
    remainingSeconds.value = 0
    const ids = await apiJson<string[]>(`${videoApiBase}/${session.uploadId}/complete`, { method: 'POST' })
    const innerTags = tags.value.map(tag => tag.uid).filter((uid): uid is string => !!uid)
    const updatedIds = await Promise.all(ids.map(async uid => {
      const finalUid = innerTags.length
        ? await fileApi.apiFileTagPatch({ updateFileTagCommand: { uid, tags: innerTags } })
        : uid
      const result = await fileApi.apiFileGet({ uID: finalUid })
      return result.items ?? []
    }))
    const uploadedFiles = updatedIds.flat() as FileDto[]
    uploadedFiles.forEach(dto => fileStore.upsertFile(dto))
    localStorage.removeItem(sessionStorageKey)
    completed.value = true
    progress.value = 100
    uploadedBytes.value = file.size
    ElMessage.success('视频上传完成')
    router.push('/file/list')
  } catch (error) {
    uploadError.value = error instanceof Error ? error.message : '视频上传失败'
    paused.value = false
    try {
      const saved = getSavedSession(file)
      if (saved) {
        const status = await apiJson<UploadStatus>(`${videoApiBase}/${saved.uploadId}`)
        chunkCount.value = status.chunkCount
        const uploaded = new Set(status.uploadedChunks)
        missingChunks.value = Array.from({ length: status.chunkCount }, (_, index) => index)
          .filter(index => !uploaded.has(index))
        uploadedBytes.value = status.uploadedChunks.reduce((sum, index) =>
          sum + Math.min(status.chunkSize, file.size - index * status.chunkSize), 0)
        progress.value = Math.min(100, uploadedBytes.value / file.size * 100)
      }
    } catch { /* keep the original upload error */ }
  } finally {
    uploading.value = false
  }
}

function pauseVideoUpload() {
  // Finish the in-flight chunk first: aborting a multipart request could leave a partial
  // server-side chunk file that the status endpoint would mistakenly treat as complete.
  pauseRequested = true
}

function resumeVideoUpload() {
  paused.value = false
  void startVideoUpload()
}

function handleUploadFile() {
  if (isVideoMode.value) {
    router.push('/file/upload?type=video')
    return
  }
  router.push('/file/upload')
}

async function commit() {
  if (!canCommit.value) return
  const blobs: Blob[] = fileList.value
    .map(file => file.raw)
    .filter((raw): raw is UploadRawFile => !!raw)
  if (blobs.length === 0) {
    ElMessage.warning('没有可上传的文件')
    return
  }
  uploading.value = true
  try {
    const tempGuidGroup = await fileApi.apiFileUploadPost({ file: blobs })
    const innerTags = tags.value.map(tag => tag.uid).filter((uid): uid is string => uid != null)
    const updateTasks = tempGuidGroup.map(async uid => {
      const finalUid = innerTags.length
        ? await fileApi.apiFileTagPatch({ updateFileTagCommand: { uid, tags: innerTags } })
        : uid
      const fileDtos = await fileApi.apiFileGet({ uID: finalUid })
      return fileDtos.items ?? []
    })
    const uploadedFiles = (await Promise.all(updateTasks)).flat()
    uploadedFiles.forEach((dto: FileDto) => fileStore.upsertFile(dto))
    if (uploadedFiles.length > 0) ElMessage.success(`成功上传 ${uploadedFiles.length} 个文件`)
    fileList.value = []
    tags.value = []
    router.push('/file/list')
  } catch (error) {
    console.error('Blog upload error:', error)
    ElMessage.error(error instanceof Error ? error.message : '上传流程发生异常，请检查网络或后端日志')
  } finally {
    uploading.value = false
  }
}

watch(isVideoMode, () => {
  uploadError.value = ''
})
</script>

<template>
  <div class="upload-workspace" v-loading="uploading && !isVideoMode">
    <MgsPageHeader
      :title="isVideoMode ? 'Video Upload' : 'Upload'"
      :description="isVideoMode ? '断点续传大视频文件，上传中可查看进度、速度和剩余时间' : '上传文章文件，并在上传前统一设置标签'"
      :back="true"
    >
      <template #actions>
        <ElButton :icon="ArrowLeft" @click="goBack">返回文件列表</ElButton>
        <template v-if="!isVideoMode">
          <ElButton type="primary" :icon="UploadFilled" :disabled="!canCommit" @click="commit">开始上传</ElButton>
        </template>
      </template>
    </MgsPageHeader>

    <template v-if="isVideoMode">
      <div class="video-upload-content">
        <ElCard shadow="never" class="upload-card">
          <template #header><div class="section-title">视频文件</div></template>
          <ElUpload
            v-model:file-list="videoFileList"
            :accept="videoAccept"
            drag
            :limit="1"
            :auto-upload="false"
            :disabled="uploading || completed"
            :on-change="onVideoFileChange"
            :on-remove="() => { videoFile = null; videoFileList = [] }"
          >
            <ElIcon class="upload-icon"><VideoCamera /></ElIcon>
            <div class="el-upload__text">将视频拖到这里，或 <em>点击选择视频</em></div>
            <template #tip><div class="upload-tip">支持 MP4、WebM；大文件将自动分片上传</div></template>
          </ElUpload>

          <div v-if="videoFile" class="video-file-summary">
            <div class="video-file-name">{{ videoFile.name }}</div>
            <div class="upload-tip">{{ videoFileSize }}</div>
          </div>

          <div v-if="videoFile && (uploading || paused || uploadError || completed)" class="progress-panel">
            <div class="progress-heading">
              <strong>{{ completed ? '上传完成' : paused ? '已暂停' : uploadError ? '上传遇到问题' : '正在上传' }}</strong>
              <strong>{{ Math.floor(progress) }}%</strong>
            </div>
            <ElProgress :percentage="Math.floor(progress)" :stroke-width="10" :status="completed ? 'success' : uploadError ? 'exception' : undefined" />
            <div class="progress-stats">
              <span>{{ formatBytes(uploadedBytes) }} / {{ formatBytes(videoFile.size) }}</span>
              <span>分片 {{ currentChunk }} / {{ chunkCount }}</span>
            </div>
            <div class="progress-stats">
              <span>速度 {{ uploadSpeed > 0 ? `${formatBytes(uploadSpeed)}/s` : '计算中' }}</span>
              <span>剩余 {{ remainingText }}</span>
            </div>
            <div v-if="chunkCount > 0" class="missing-chunks">
              <strong>待补传分片（{{ missingChunks.length }}）</strong>
              <span>{{ missingChunksText }}</span>
            </div>
            <ElAlert v-if="uploadError" :title="uploadError" type="error" :closable="false" show-icon />
            <div class="progress-actions">
              <ElButton v-if="uploading" :icon="VideoPause" @click="pauseVideoUpload">暂停（当前分片完成后生效）</ElButton>
              <ElButton v-else-if="!completed" type="primary" :icon="uploadError ? Refresh : VideoPlay" @click="resumeVideoUpload">
                {{ uploadError ? '重试 / 恢复上传' : '继续上传' }}
              </ElButton>
            </div>
          </div>
          <ElButton
            v-if="videoFile && !uploading && !paused && !uploadError && !completed"
            class="start-video-button"
            type="primary"
            :icon="UploadFilled"
            @click="startVideoUpload"
          >开始断点上传</ElButton>
        </ElCard>

        <ElCard shadow="never" class="tag-card">
          <template #header><div class="section-title">标签</div></template>
          <TagSelect v-model:tag-list="tags" :select-disabled="!videoFile || uploading || completed" />
          <p class="helper-text">标签会在视频合并并保存后应用到该文件。</p>
          <div class="upload-tip">上传任务 ID：{{ sessionId || '开始上传后生成' }}</div>
        </ElCard>
      </div>
    </template>

    <template v-else>
      <div class="upload-content">
        <ElCard shadow="never" class="upload-card">
          <template #header><div class="section-title">文件</div></template>
          <ElUpload v-model:file-list="fileList" :accept="acceptType" drag multiple show-file-list :auto-upload="false">
            <ElIcon class="upload-icon"><UploadFilled /></ElIcon>
            <div class="el-upload__text">将文件拖到这里，或 <em>点击选择文件</em></div>
            <template #tip><div class="upload-tip">允许上传：{{ acceptType }}</div></template>
          </ElUpload>
        </ElCard>
        <ElCard shadow="never" class="tag-card">
          <template #header><div class="section-title">标签</div></template>
          <TagSelect v-model:tag-list="tags" :select-disabled="fileList.length === 0" />
          <p class="helper-text">标签会应用到本次上传的所有文件。</p>
        </ElCard>
      </div>
    </template>
  </div>
</template>

<style scoped>
.upload-workspace { min-height: 100%; display: flex; flex-direction: column; }
.upload-content, .video-upload-content { display: grid; grid-template-columns: minmax(0, 1fr) 320px; gap: 16px; padding-bottom: 24px; }
.upload-card, .tag-card { min-width: 0; }
.section-title { font-weight: 600; }
.upload-icon { font-size: 52px; margin-bottom: 10px; }
.upload-tip, .helper-text { color: var(--mgs-muted); font-size: 13px; }
.video-file-summary { display: flex; justify-content: space-between; align-items: center; gap: 16px; margin-top: 18px; padding: 12px; border-radius: 8px; background: var(--el-fill-color-light); }
.video-file-name { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; font-weight: 600; }
.progress-panel { display: flex; flex-direction: column; gap: 12px; margin-top: 24px; }
.progress-heading, .progress-stats { display: flex; justify-content: space-between; align-items: center; gap: 12px; }
.progress-stats { color: var(--mgs-muted); font-size: 13px; }
.missing-chunks { display: flex; flex-direction: column; gap: 4px; color: var(--mgs-muted); font-size: 13px; overflow-wrap: anywhere; }
.missing-chunks strong { color: var(--el-text-color-primary); font-weight: 600; }
.progress-actions { display: flex; justify-content: flex-end; }
.start-video-button { margin-top: 20px; }
@media (max-width: 900px) { .upload-content, .video-upload-content { grid-template-columns: 1fr; } }
</style>
