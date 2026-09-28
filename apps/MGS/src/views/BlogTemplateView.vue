<template>
  <div class="template-view">
    <MgsPageHeader title="博文模板" description="选择模板并编辑文章草稿" :back="true">
      <template #actions>
        <ElButton :icon="Plus" @click="startCreatingTemplate">添加模板</ElButton>
      </template>
    </MgsPageHeader>

    <div class="workspace">
      <aside class="template-panel">
        <div class="panel-title">
          <div>
            <div class="title">选择模板</div>
            <div class="subtitle">先选一个结构，再开始写</div>
          </div>
          <ElTag size="small">{{ templates.length }} 个</ElTag>
        </div>

        <div class="template-list">
          <div
            v-for="item in templates"
            :key="item.uid"
            class="template-card"
            :class="{ active: item.uid === selectedTemplate?.uid }"
            role="button"
            tabindex="0"
            @click="selectTemplate(item.uid!)"
            @keydown.enter="selectTemplate(item.uid!)"
          >
            <div class="template-icon">M</div>
            <div class="template-info">
              <div class="template-name">{{ item.fileName || '未命名模板' }}</div>
              <div class="template-description">Version {{ item.version ?? 1 }} · {{ item.fileType || '.md' }}</div>
              <div class="template-tags">
                <ElTag v-for="tag in item.tags ?? []" :key="tag.uid" size="small" effect="plain">{{ tag.name }}</ElTag>
              </div>
              <div class="template-actions" @click.stop>
                <ElButton link type="primary" @click="selectTemplate(item.uid!)">预览</ElButton>
                <ElButton link :disabled="editing" @click="startEditingTemplate(item)">修改</ElButton>
                <ElButton link type="danger" :disabled="editing" @click="deleteTemplate(item)">删除</ElButton>
              </div>
            </div>
          </div>
        </div>

        <div class="panel-tip">
          <ElIcon><InfoFilled /></ElIcon>
          <span>模板默认只读。使用模板创建博文后，后续修改不会影响原模板。</span>
        </div>
      </aside>

      <main class="editor-panel">
        <div v-if="selectedTemplate || creatingTemplate" class="template-toolbar">
          <div>
            <div class="selected-template-name">{{ creatingTemplate ? (templateDraft.name || '新建模板') : selectedTemplate?.fileName }}</div>
            <div class="selected-template-state">
              {{ creatingTemplate ? '正在创建模板' : editing ? '正在修改模板' : '只读预览' }}
              <template v-if="!creatingTemplate"> · Version {{ selectedTemplate?.version ?? 1 }}</template>
            </div>
          </div>
          <div class="template-toolbar-actions">
            <template v-if="creatingTemplate">
              <ElButton @click="cancelCreatingTemplate">取消</ElButton>
              <ElButton type="primary" :disabled="!canSaveNewTemplate || saving" @click="saveNewTemplate">保存模板</ElButton>
            </template>
            <template v-else-if="editing">
              <ElButton @click="cancelEditing">取消</ElButton>
              <ElButton type="primary" :disabled="!templateDirty || saving" @click="saveTemplate">保存模板</ElButton>
            </template>
            <template v-else>
              <ElButton :icon="Edit" @click="startEditingTemplate(selectedTemplate)">修改</ElButton>
              <ElButton type="primary" :icon="Plus" @click="createArticle">使用此模板</ElButton>
            </template>
          </div>
        </div>

        <ElForm v-if="creatingTemplate" :model="templateDraft" label-position="top" class="meta-form">
          <ElFormItem label="模板名称">
            <ElInput v-model="templateDraft.name" size="large" placeholder="例如：技术文章模板" clearable />
          </ElFormItem>
        </ElForm>

        <ElForm v-else :model="draft" label-position="top" class="meta-form">
          <div class="form-row">
            <ElFormItem label="标题" class="title-field">
              <ElInput v-model="draft.title" size="large" placeholder="输入新博文标题" clearable :disabled="editing" />
            </ElFormItem>
            <ElFormItem label="标签" class="tags-field">
              <ElSelect
                v-model="draft.tags"
                multiple
                filterable
                allow-create
                default-first-option
                collapse-tags
                placeholder="添加标签"
                style="width: 100%"
              >
                <ElOption v-for="tag in tagOptions" :key="tag" :label="tag" :value="tag" />
              </ElSelect>
            </ElFormItem>
          </div>

          <ElFormItem label="摘要">
            <ElInput v-model="draft.summary" type="textarea" :rows="2" maxlength="160" show-word-limit placeholder="用一句话描述这篇文章" />
          </ElFormItem>
        </ElForm>

        <div class="sheet">
          <div class="sheet-toolbar">
            <div class="sheet-cell row-number">#</div>
            <div class="sheet-cell">正文 / Markdown</div>
            <div class="sheet-cell preview-heading">实时预览</div>
          </div>

          <div class="sheet-body">
            <div class="line-numbers">
              <div v-for="(_, index) in bodyLines" :key="index">{{ index + 1 }}</div>
            </div>
            <div class="markdown-input">
              <ElInput
                v-model="draft.content"
                type="textarea"
                resize="none"
                :readonly="!creatingTemplate && !editing"
                :placeholder="creatingTemplate || editing ? '编辑 Markdown 模板...' : '模板内容加载中...'"
              />
            </div>
            <div class="preview">
              <MarkdownRender :markdown="draft.content" :baseurl="baseurl" />
            </div>
          </div>
        </div>

        <div class="status-bar">
          <span>{{ creatingTemplate ? (templateDraft.name || '新建模板') : selectedTemplate?.fileName || '未选择模板' }}</span>
          <span>{{ bodyLines.length }} 行 · {{ draft.content.length }} 字符</span>
          <span :class="creatingTemplate || editing ? 'status-editing' : 'status-ready'">
            {{ creatingTemplate ? '新模板编辑中' : editing ? '模板编辑中' : '模板只读' }}
          </span>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeMount, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  ElButton,
  ElForm,
  ElFormItem,
  ElIcon,
  ElInput,
  ElMessage,
  ElMessageBox,
  ElOption,
  ElSelect,
  ElTag,
} from 'element-plus'
import { Edit, InfoFilled, Plus } from '@element-plus/icons-vue'
import { MarkdownRender } from '@aqlife/ui-shared'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import { FileApi, type FileDto } from '@/api'
import { apiConfiguration } from '@/services/api'
import { useFileStore } from '@/stores/useFileStore'
import { FileContentUpdateWorkflow, FileDeleteWorkflow } from '@/workflow'

type BlogTemplate = FileDto

const baseurl = import.meta.env.VITE_API
const router = useRouter()
const fileApi = new FileApi(apiConfiguration)
const fileStore = useFileStore()

const templates = ref<BlogTemplate[]>([])
const selectedTemplate = ref<BlogTemplate>()
const selectedTemplateContent = ref('')
const editing = ref(false)
const creatingTemplate = ref(false)
const templateDraft = reactive({
  name: '',
})
const saving = ref(false)
const loading = ref(false)

const draft = reactive({
  title: '未命名文章',
  summary: '',
  tags: [] as string[],
  content: '',
})

const templateDirty = computed(() => draft.content !== selectedTemplateContent.value)
const canSaveNewTemplate = computed(() => Boolean(templateDraft.name.trim() && draft.content.trim()))
const bodyLines = computed(() => draft.content.split('\n'))
const tagOptions = computed(() => (selectedTemplate.value?.tags ?? []).map(tag => tag.name).filter((name): name is string => Boolean(name)))

async function loadTemplates(selectUid?: string) {
  const result = await fileApi.apiFileGet({
    includeTemplates: true,
    page: 1,
    pageSize: 100,
  })

  templates.value = (result.items ?? []).filter(item => item.isTemplate)

  if (!templates.value.length) {
    selectedTemplate.value = undefined
    selectedTemplateContent.value = ''
    draft.content = ''
    return
  }

  const uid =
    selectUid && templates.value.some(item => item.uid === selectUid)
      ? selectUid
      : selectedTemplate.value?.uid && templates.value.some(item => item.uid === selectedTemplate.value?.uid)
        ? selectedTemplate.value.uid
        : templates.value[0].uid

  if (uid) await selectTemplate(uid)
}

async function selectTemplate(uid: string) {
  const metadata = templates.value.find(item => item.uid === uid)
  if (!metadata?.uid) return

  loading.value = true
  try {
    const response = await fileApi.apiFilePreviewGetRaw({ uID: metadata.uid })
    if (response.raw.status !== 200) {
      throw new Error('模板加载失败（HTTP ' + response.raw.status + '）')
    }

    const content = await response.raw.text()
    selectedTemplate.value = metadata
    selectedTemplateContent.value = content
    draft.content = content
    draft.title = '未命名文章'
    draft.summary = ''
    draft.tags = (metadata.tags ?? []).map(tag => tag.name ?? '').filter(Boolean)
    editing.value = false
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '模板加载失败')
  } finally {
    loading.value = false
  }
}

function startCreatingTemplate() {
  selectedTemplate.value = undefined
  selectedTemplateContent.value = ''
  editing.value = false
  creatingTemplate.value = true
  templateDraft.name = ''
  draft.content = '# {{title}}\n\n'
  draft.title = '未命名文章'
  draft.summary = ''
  draft.tags = []
}

function cancelCreatingTemplate() {
  creatingTemplate.value = false
  templateDraft.name = ''
  draft.content = ''
  if (templates.value[0]?.uid) {
    void selectTemplate(templates.value[0].uid)
  }
}

async function startEditingTemplate(template: BlogTemplate) {
  if (selectedTemplate.value?.uid !== template.uid) {
    await selectTemplate(template.uid!)
  }

  creatingTemplate.value = false
  editing.value = true
}

function cancelEditing() {
  draft.content = selectedTemplateContent.value
  editing.value = false
}

async function saveNewTemplate() {
  if (!canSaveNewTemplate.value || saving.value) return

  const name = sanitizeFileName(templateDraft.name) + '.md'

  saving.value = true
  try {
    const contentFile = new File([draft.content], name, { type: 'text/markdown' })
    const uids = await fileApi.apiFileUploadPost({
      isTemplate: true,
      file: [contentFile],
    })

    const uid = uids[0]
    if (!uid) throw new Error('模板创建失败')

    creatingTemplate.value = false
    templateDraft.name = ''
    await loadTemplates(uid)
    ElMessage.success('模板已创建')
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '模板创建失败')
  } finally {
    saving.value = false
  }
}

async function saveTemplate() {
  const current = selectedTemplate.value
  if (!current?.uid || !templateDirty.value || saving.value) return

  saving.value = true
  try {
    const fileName = (current.fileName || 'template') + (current.fileType || '.md')
    const contentFile = new File([draft.content], fileName, { type: 'text/markdown' })

    const updated = await new FileContentUpdateWorkflow(
      current,
      contentFile,
      fileApi,
      fileStore,
      true,
    ).run()

    selectedTemplate.value = updated
    selectedTemplateContent.value = draft.content
    templates.value = templates.value.map(item => item.uid === updated.uid ? updated : item)
    editing.value = false
    ElMessage.success('模板已保存')
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '模板保存失败')
  } finally {
    saving.value = false
  }
}

async function deleteTemplate(template: BlogTemplate) {
  if (!template.uid) return

  try {
    await ElMessageBox.confirm(
      '确定删除「' + (template.fileName || '未命名模板') + '」吗？删除后无法继续用于创建博文。',
      '删除模板',
      {
        confirmButtonText: '删除',
        cancelButtonText: '取消',
        type: 'warning',
      },
    )

    loading.value = true
    await new FileDeleteWorkflow(template, fileApi, fileStore).run()
    templates.value = templates.value.filter(item => item.uid !== template.uid)

    if (selectedTemplate.value?.uid === template.uid) {
      selectedTemplate.value = undefined
      selectedTemplateContent.value = ''
      draft.content = ''
      editing.value = false

      if (templates.value[0]?.uid) {
        await selectTemplate(templates.value[0].uid)
      }
    }

    ElMessage.success('模板已删除')
  } catch (error) {
    if (error === 'cancel' || error === 'close') return
    ElMessage.error(error instanceof Error ? error.message : '模板删除失败')
  } finally {
    loading.value = false
  }
}

function sanitizeFileName(value: string) {
  const normalized = value
    .trim()
    .replace(/[\\/:*?"<>|]/g, '-')
    .slice(0, 64)

  return normalized || '未命名文章'
}

async function createArticle() {
  const template = selectedTemplate.value
  if (!template?.uid || !selectedTemplateContent.value) return

  const title = draft.title.trim() || '未命名文章'
  const content = selectedTemplateContent.value.replaceAll('{{title}}', title)
  const fileName = sanitizeFileName(title) + '.md'

  try {
    loading.value = true

    const uids = await fileApi.apiFileUploadPost({
      isTemplate: false,
      file: [new File([content], fileName, { type: 'text/markdown' })],
    })

    const uid = uids[0]
    if (!uid) throw new Error('博文创建失败')

    let article = (await fileApi.apiFileGet({ uID: uid })).items?.[0]
    if (!article) throw new Error('博文创建后无法读取文件')

    const tagUids = (template.tags ?? [])
      .map(tag => tag.uid)
      .filter((uid): uid is string => Boolean(uid))

    if (tagUids.length) {
      await fileApi.apiFileTagPatch({
        updateFileTagCommand: {
          uid,
          tags: tagUids,
        },
      })
      article = (await fileApi.apiFileGet({ uID: uid })).items?.[0] ?? article
    }

    fileStore.upsertFile(article)
    ElMessage.success('博文草稿已创建')
    await router.push('/blog/' + uid + '/edit')
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '博文创建失败')
  } finally {
    loading.value = false
  }
}

onBeforeMount(async () => {
  try {
    loading.value = true
    await loadTemplates()
  } catch (error) {
    ElMessage.error(error instanceof Error ? error.message : '模板列表加载失败')
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.template-view { min-height: 0; height: 100%; display: flex; flex-direction: column; overflow: hidden; background: var(--mgs-bg); }
.workspace { min-height: 0; flex: 1; display: grid; grid-template-columns: 280px minmax(0, 1fr); padding: 0 28px 18px; gap: 18px; }
.template-panel { min-height: 0; display: flex; flex-direction: column; border: 1px solid var(--mgs-border); border-radius: var(--mgs-radius); background: var(--mgs-surface); overflow: hidden; }
.panel-title { display: flex; align-items: center; justify-content: space-between; padding: 16px; border-bottom: 1px solid var(--mgs-border); }
.title { font-size: 15px; font-weight: 650; }
.subtitle { margin-top: 4px; color: var(--mgs-muted); font-size: 12px; }
.template-list { flex: 1; min-height: 0; overflow: auto; padding: 8px; }
.template-card { width: 100%; display: flex; gap: 12px; padding: 12px; margin-bottom: 8px; text-align: left; border: 1px solid transparent; border-radius: 8px; background: transparent; cursor: pointer; }
.template-card:hover { background: var(--mgs-surface-soft); }
.template-card.active { border-color: #b9c9f7; background: var(--mgs-accent-soft); }
.template-icon { width: 38px; height: 38px; flex: 0 0 38px; display: grid; place-items: center; border-radius: 7px; background: var(--mgs-surface-soft); color: var(--mgs-accent); font-weight: 700; font-family: monospace; }
.template-name { color: var(--mgs-text); font-size: 14px; font-weight: 600; }
.template-description { margin-top: 4px; color: var(--mgs-muted); font-size: 12px; line-height: 1.5; }
.template-tags { display: flex; flex-wrap: wrap; gap: 4px; margin-top: 7px; }
.template-actions { display: flex; gap: 2px; margin-top: 7px; }
.panel-tip { display: flex; gap: 7px; padding: 12px 14px; color: var(--mgs-muted); font-size: 12px; line-height: 1.5; border-top: 1px solid var(--mgs-border); }
.editor-panel { min-width: 0; min-height: 0; display: flex; flex-direction: column; padding: 0; }
.template-toolbar { min-height: 64px; display: flex; align-items: center; justify-content: space-between; gap: 16px; }
.selected-template-name { font-size: 16px; font-weight: 650; }
.selected-template-state { margin-top: 4px; color: var(--mgs-muted); font-size: 12px; }
.template-toolbar-actions { display: flex; align-items: center; gap: 8px; }
.meta-form { flex: 0 0 auto; }
.form-row { display: grid; grid-template-columns: minmax(0, 1.7fr) minmax(240px, 1fr); gap: 14px; }
.meta-form :deep(.el-form-item) { margin-bottom: 12px; }
.sheet { min-height: 0; flex: 1; display: flex; flex-direction: column; overflow: hidden; border: 1px solid var(--mgs-border); border-radius: var(--mgs-radius); background: var(--mgs-surface); box-shadow: var(--mgs-shadow); }
.sheet-toolbar { display: grid; grid-template-columns: 46px minmax(0, 1fr) minmax(0, 1fr); flex: 0 0 34px; border-bottom: 1px solid var(--mgs-border); background: var(--mgs-surface-soft); }
.sheet-cell { display: flex; align-items: center; padding: 0 10px; border-right: 1px solid var(--mgs-border); color: var(--mgs-muted); font-size: 12px; }
.row-number { justify-content: center; padding: 0; }
.preview-heading { border-right: 0; }
.sheet-body { min-height: 0; flex: 1; display: grid; grid-template-columns: 46px minmax(0, 1fr) minmax(0, 1fr); }
.line-numbers { overflow: hidden; padding-top: 8px; text-align: center; background: var(--mgs-surface-soft); color: var(--mgs-muted); font: 13px/22px monospace; user-select: none; }
.markdown-input { min-width: 0; border-right: 1px solid var(--mgs-border); }
.markdown-input :deep(.el-textarea), .markdown-input :deep(.el-textarea__inner) { height: 100%; }
.markdown-input :deep(.el-textarea__inner) { border: 0; border-radius: 0; padding: 8px 12px; resize: none; box-shadow: none; font: 14px/22px 'Cascadia Code PL', monospace; }
.markdown-input :deep(.el-textarea__inner[readonly]) { background: var(--mgs-surface); cursor: default; }
.preview { min-width: 0; overflow: auto; padding: 10px 18px; }
.status-bar { height: 30px; flex: 0 0 30px; display: flex; align-items: center; gap: 18px; color: var(--mgs-muted); font-size: 12px; }
.status-bar span:last-child { margin-left: auto; }
.status-ready { color: var(--mgs-success); }
.status-editing { color: var(--mgs-accent); }
.hidden-input { display: none; }
@media (max-width: 900px) {
  .workspace { grid-template-columns: 220px minmax(0, 1fr); padding: 0 16px 16px; }
  .sheet-toolbar, .sheet-body { grid-template-columns: 36px minmax(0, 1fr); }
  .preview, .preview-heading { display: none; }
}
</style>
