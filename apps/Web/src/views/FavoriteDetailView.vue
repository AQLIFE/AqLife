<script setup lang="ts">
import { computed, onBeforeUnmount, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElButton, ElEmpty, ElImage, ElMessage, ElTag } from 'element-plus'
import { ArrowLeft, ArrowRight, Share } from '@element-plus/icons-vue'
import { FavoriteApi, FileApi, TagApi, type FavoriteDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'

const route = useRoute()
const router = useRouter()
const favoriteApi = new FavoriteApi(apiConfiguration)
const fileApi = new FileApi(apiConfiguration)
const tagApi = new TagApi(apiConfiguration)

const favorite = ref<FavoriteDto | null>(null)
const tags = ref<TagDto[]>([])
const imageUrls = ref<Record<string, string>>({})
const loading = ref(false)
const notFound = ref(false)
const previousFavorite = ref<FavoriteDto | null>(null)
const nextFavorite = ref<FavoriteDto | null>(null)
const objectUrls = new Set<string>()

const typeOptions = [
  { value: 'Food', label: '食物' },
  { value: 'Book', label: '书籍' },
  { value: 'Game', label: '游戏' },
  { value: 'Tech', label: '技术' },
  { value: 'Life', label: '生活' },
  { value: 'Other', label: '其他' },
]

const mediaItems = computed(() =>
  [...(favorite.value?.media ?? [])]
    .filter(media => !!media.fileId)
    .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0)),
)
const galleryUrls = computed(() =>
  mediaItems.value
    .map(media => media.fileId ? imageUrls.value[media.fileId] : undefined)
    .filter((url): url is string => !!url),
)
const favoriteTags = computed(() =>
  (favorite.value?.tagIds ?? [])
    .map(id => tags.value.find(tag => tag.uid === id)?.name)
    .filter((name): name is string => !!name),
)
const contentTypeLabel = computed(() =>
  typeOptions.find(type => type.value === favorite.value?.contentType)?.label
    ?? favorite.value?.contentType
    ?? '收藏',
)

function releaseImageUrls() {
  for (const url of objectUrls) URL.revokeObjectURL(url)
  objectUrls.clear()
  imageUrls.value = {}
}

async function loadImages() {
  const media = mediaItems.value
  await Promise.all(media.map(async item => {
    const fileId = item.fileId
    if (!fileId || imageUrls.value[fileId]) return
    try {
      const blob = await fileApi.apiFilePreviewGet({ uID: fileId })
      const url = URL.createObjectURL(blob)
      objectUrls.add(url)
      imageUrls.value[fileId] = url
    } catch {
      // Keep the remaining gallery available when one file is missing.
    }
  }))
}

async function loadNeighbors(uid: string) {
  previousFavorite.value = null
  nextFavorite.value = null

  const pageSize = 24
  let page = 1
  while (true) {
    const result = await favoriteApi.apiFavoriteGet({ page, pageSize })
    const items = result.items ?? []
    const currentIndex = items.findIndex(item => item.uid === uid)

    if (currentIndex >= 0) {
      previousFavorite.value = currentIndex > 0 ? items[currentIndex - 1] : null
      nextFavorite.value = currentIndex < items.length - 1
        ? items[currentIndex + 1]
        : null

      if (currentIndex === 0 && page > 1) {
        const previousPage = await favoriteApi.apiFavoriteGet({ page: page - 1, pageSize })
        const previousItems = previousPage.items ?? []
        previousFavorite.value = previousItems[previousItems.length - 1] ?? null
      }
      if (currentIndex === items.length - 1 && result.hasMore) {
        const nextPage = await favoriteApi.apiFavoriteGet({ page: page + 1, pageSize })
        nextFavorite.value = nextPage.items?.[0] ?? null
      }
      return
    }

    if (!result.hasMore || items.length === 0) return
    page += 1
  }
}

function openNeighbor(item: FavoriteDto | null) {
  if (item?.uid) void router.push('/favorite/' + encodeURIComponent(item.uid))
}

async function loadFavorite(uid: string) {
  loading.value = true
  notFound.value = false
  favorite.value = null
  releaseImageUrls()

  try {
    const result = await favoriteApi.apiFavoriteGet({ uID: uid, page: 1, pageSize: 1 })
    favorite.value = result.items?.find(item => item.uid === uid) ?? null
    try {
      const tagResult = await tagApi.apiTagGet({ page: 1, pageSize: 100 })
      tags.value = tagResult.items ?? []
    } catch {
      tags.value = []
    }
    notFound.value = !favorite.value
    if (favorite.value) {
      await Promise.all([
        loadImages(),
        loadNeighbors(uid).catch(() => {
          previousFavorite.value = null
          nextFavorite.value = null
        }),
      ])
    }
  } catch (error) {
    notFound.value = true
    ElMessage.warning(error instanceof Error ? error.message : '收藏详情加载失败')
  } finally {
    loading.value = false
  }
}

async function shareFavorite() {
  const url = window.location.href

  try {
    if (navigator.clipboard?.writeText) {
      await navigator.clipboard.writeText(url)
    } else {
      const textarea = document.createElement('textarea')
      textarea.value = url
      textarea.setAttribute('readonly', '')
      textarea.style.position = 'fixed'
      textarea.style.opacity = '0'
      document.body.appendChild(textarea)
      textarea.select()
      const copied = document.execCommand('copy')
      document.body.removeChild(textarea)
      if (!copied) throw new Error('浏览器不支持复制')
    }

    ElMessage.success('收藏链接已复制')
  } catch (error) {
    console.error('复制收藏链接失败:', error)
    ElMessage.error('复制失败，请手动复制当前页面地址')
  }
}

function formatDate(value?: string) {
  if (!value) return ''
  return value.slice(0, 10)
}

watch(
  () => route.params.uid,
  value => {
    if (typeof value === 'string' && value) void loadFavorite(value)
  },
  { immediate: true },
)

onBeforeUnmount(releaseImageUrls)
</script>

<template>
  <section class="favorite-detail-page">
    <div class="detail-topbar">
      <ElButton text :icon="ArrowLeft" @click="router.push('/favorite')">返回收藏</ElButton>
      <ElButton text :icon="Share" title="分享收藏" aria-label="分享收藏" @click="shareFavorite" />
    </div>

    <div v-if="loading" class="detail-state">
      <span>正在加载收藏详情…</span>
    </div>
    <ElEmpty v-else-if="notFound || !favorite" description="没有找到这条收藏，或它尚未公开">
      <ElButton type="primary" @click="router.push('/favorite')">返回收藏列表</ElButton>
    </ElEmpty>

    <article v-else class="detail-content">
      <header class="detail-header">
        <div class="detail-eyebrow">
          <span>{{ contentTypeLabel }}</span>
          <span v-if="favorite.createdAt">{{ formatDate(favorite.createdAt) }}</span>
        </div>
        <h1>{{ favorite.title || '未命名收藏' }}</h1>
        <p v-if="favorite.description" class="detail-description">{{ favorite.description }}</p>
        <div v-if="favoriteTags.length" class="detail-tags">
          <ElTag v-for="tag in favoriteTags" :key="tag" effect="plain">{{ tag }}</ElTag>
        </div>
      </header>

      <section v-if="mediaItems.length" class="detail-gallery-section">
        <div class="section-heading">
          <h2>图片集</h2>
          <span>{{ mediaItems.length }} 张图片</span>
        </div>
        <div class="detail-gallery">
          <div v-for="(media, index) in mediaItems" :key="media.fileId" class="gallery-item">
            <ElImage
              v-if="media.fileId && imageUrls[media.fileId]"
              :src="imageUrls[media.fileId]"
              :preview-src-list="galleryUrls"
              :initial-index="Math.max(0, galleryUrls.indexOf(imageUrls[media.fileId]))"
              fit="cover"
              hide-on-click-modal
              class="gallery-image"
            />
            <div v-else class="gallery-placeholder">图片加载中</div>
            <span v-if="media.role === 'Cover'" class="gallery-label">封面</span>
            <span v-else class="gallery-label">图片 {{ index + 1 }}</span>
          </div>
        </div>
      </section>

      <section class="detail-info">
        <div class="section-heading">
          <h2>收藏信息</h2>
        </div>
        <dl>
          <div>
            <dt>内容类型</dt>
            <dd>{{ contentTypeLabel }}</dd>
          </div>
          <div v-if="favorite.createdAt">
            <dt>添加时间</dt>
            <dd>{{ formatDate(favorite.createdAt) }}</dd>
          </div>
          <div v-if="favorite.updatedAt">
            <dt>更新时间</dt>
            <dd>{{ formatDate(favorite.updatedAt) }}</dd>
          </div>
          <div v-if="favorite.sourceUrl">
            <dt>来源地址</dt>
            <dd><a :href="favorite.sourceUrl" target="_blank" rel="noopener noreferrer">{{ favorite.sourceUrl }}</a></dd>
          </div>
        </dl>
      </section>

      <footer class="detail-footer">
        <ElButton
          plain
          :icon="ArrowLeft"
          :disabled="!previousFavorite"
          @click="openNeighbor(previousFavorite)"
        >
          {{ previousFavorite ? '上一篇：' + (previousFavorite.title || '未命名收藏') : '已经是第一篇' }}
        </ElButton>
        <ElButton
          plain
          :icon="ArrowRight"
          :disabled="!nextFavorite"
          @click="openNeighbor(nextFavorite)"
        >
          {{ nextFavorite ? '下一篇：' + (nextFavorite.title || '未命名收藏') : '已经是最后一篇' }}
        </ElButton>
      </footer>
    </article>
  </section>
</template>

<style scoped>
.favorite-detail-page { height: 100%; min-height: 0; box-sizing: border-box; overflow-y: auto; overflow-x: hidden; overscroll-behavior-y: contain; padding: 26px clamp(16px, 4vw, 48px) 56px; background: var(--el-bg-color); color: var(--el-text-color-primary); }
.detail-topbar, .detail-content { width: min(100%, 920px); margin: 0 auto; }
.detail-topbar { display: flex; align-items: center; justify-content: space-between; gap: 16px; margin-bottom: 28px; }
.detail-state { display: grid; place-items: center; min-height: 240px; color: var(--el-text-color-secondary); }
.detail-header { padding: 10px 0 28px; border-bottom: 1px solid var(--el-border-color-lighter); }
.detail-eyebrow { display: flex; flex-wrap: wrap; gap: 14px; color: var(--el-text-color-placeholder); font-size: 12px; }
.detail-header h1 { margin: 14px 0 12px; font-family: Georgia, "Noto Serif SC", serif; font-size: clamp(28px, 4vw, 42px); font-weight: 500; line-height: 1.35; }
.detail-description { max-width: 760px; margin: 0; color: var(--el-text-color-secondary); font-size: 15px; line-height: 1.9; white-space: pre-wrap; }
.detail-tags { display: flex; flex-wrap: wrap; gap: 7px; margin-top: 18px; }
.detail-gallery-section, .detail-info { padding-top: 28px; }
.section-heading { display: flex; align-items: baseline; justify-content: space-between; gap: 12px; margin-bottom: 16px; }
.section-heading h2 { margin: 0; font-size: 18px; font-weight: 600; }
.section-heading > span { color: var(--el-text-color-placeholder); font-size: 12px; }
.detail-gallery { display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: 12px; }
.gallery-item { position: relative; overflow: hidden; min-width: 0; aspect-ratio: 4 / 3; border-radius: 8px; background: var(--el-fill-color-light); }
.gallery-image, .gallery-image :deep(.el-image__inner) { display: block; width: 100%; height: 100%; }
.gallery-image { cursor: zoom-in; }
.gallery-placeholder { display: grid; place-items: center; width: 100%; height: 100%; color: var(--el-text-color-placeholder); font-size: 12px; }
.gallery-label { position: absolute; left: 9px; bottom: 9px; padding: 4px 8px; border-radius: 4px; background: rgb(0 0 0 / 58%); color: #fff; font-size: 11px; pointer-events: none; }
.detail-info dl { margin: 0; border-top: 1px solid var(--el-border-color-lighter); }
.detail-info dl > div { display: grid; grid-template-columns: 112px minmax(0, 1fr); gap: 16px; padding: 13px 0; border-bottom: 1px solid var(--el-border-color-lighter); font-size: 13px; }
.detail-info dt { color: var(--el-text-color-placeholder); }
.detail-info dd { min-width: 0; margin: 0; overflow-wrap: anywhere; }
.detail-info dd a { color: var(--el-color-primary); text-decoration: none; }
.detail-footer { display: flex; justify-content: space-between; align-items: center; gap: 12px; padding-top: 28px; }
@media (max-width: 700px) { .detail-gallery { grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 9px; } }
@media (max-width: 420px) { .detail-gallery { grid-template-columns: minmax(0, 1fr); } .detail-info dl > div { grid-template-columns: 84px minmax(0, 1fr); } }
</style>
