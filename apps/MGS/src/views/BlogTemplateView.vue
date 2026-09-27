<template>
  <div class="template-view">
    <ElPageHeader class="header" content="新建博文" @back="router.back()">
      <template #extra>
        <ElButton @click="resetDraft">重置</ElButton>
        <ElButton type="success" :disabled="!draft.title.trim()" @click="createDraft">
          创建博文
        </ElButton>
      </template>
    </ElPageHeader>

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
          <button
            v-for="item in templates"
            :key="item.id"
            class="template-card"
            :class="{ active: item.id === selectedTemplateId }"
            type="button"
            @click="selectTemplate(item.id)"
          >
            <div class="template-icon">{{ item.icon }}</div>
            <div class="template-info">
              <div class="template-name">{{ item.name }}</div>
              <div class="template-description">{{ item.description }}</div>
              <div class="template-tags">
                <ElTag v-for="tag in item.tags" :key="tag" size="small" effect="plain">{{ tag }}</ElTag>
              </div>
            </div>
          </button>
        </div>

        <div class="panel-tip">
          <ElIcon><InfoFilled /></ElIcon>
          <span>模板只负责初始化内容，修改后的正文不会影响原模板。</span>
        </div>
      </aside>

      <main class="editor-panel">
        <ElForm :model="draft" label-position="top" class="meta-form">
          <div class="form-row">
            <ElFormItem label="标题" class="title-field">
              <ElInput v-model="draft.title" size="large" placeholder="输入博文标题" clearable />
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
              <ElInput v-model="draft.content" type="textarea" resize="none" placeholder="开始编写 Markdown..." />
            </div>
            <div class="preview">
              <MarkdownRender :markdown="draft.content" :baseurl="baseurl" />
            </div>
          </div>
        </div>

        <div class="status-bar">
          <span>{{ selectedTemplate?.name }}</span>
          <span>{{ bodyLines.length }} 行 · {{ draft.content.length }} 字符</span>
          <span class="status-ready">草稿已就绪</span>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import {
  ElButton,
  ElForm,
  ElFormItem,
  ElIcon,
  ElInput,
  ElMessage,
  ElOption,
  ElSelect,
  ElTag,
} from 'element-plus'
import { InfoFilled } from '@element-plus/icons-vue'
import { MarkdownRender } from '@aqlife/ui-shared'

type BlogTemplate = {
  id: string
  name: string
  description: string
  icon: string
  tags: string[]
  title: string
  summary: string
  content: string
}

const router = useRouter()
const baseurl = import.meta.env.VITE_API

const templates: BlogTemplate[] = [
  {
    id: 'article',
    name: '普通文章',
    description: '适合知识分享、随笔和经验总结',
    icon: 'A',
    tags: ['Blog', 'Article'],
    title: '未命名文章',
    summary: '',
    content: '# {{title}}\n\n## 背景\n\n在这里写下这篇文章的背景。\n\n## 正文\n\n开始记录你的内容……\n\n## 总结\n\n总结这篇文章的主要内容。',
  },
  {
    id: 'technical',
    name: '技术笔记',
    description: '预置环境、方案、代码和总结结构',
    icon: '</>',
    tags: ['Tech', 'Note'],
    title: '技术笔记：未命名',
    summary: '记录一个技术问题、解决方案以及最终结论。',
    content: '# {{title}}\n\n## 问题\n\n描述遇到的问题和上下文。\n\n## 环境\n\n- OS:\n- Runtime:\n- Version:\n\n## 方案\n\n记录关键方案。\n\n## 实现\n\n说明具体实现。\n\n## 总结\n\n记录最终结论和注意事项。',
  },
  {
    id: 'release',
    name: '版本发布',
    description: '适合记录版本更新、变更和迁移事项',
    icon: '↗',
    tags: ['Release', 'Changelog'],
    title: 'Release v0.0.0',
    summary: '本次版本包含的主要变更。',
    content: '# Release v0.0.0\n\n## ✨ 新增\n\n- 新功能\n\n## 🐛 修复\n\n- 修复的问题\n\n## ⚠️ 注意\n\n- 兼容性或迁移说明\n\n## 📦 升级\n\n说明升级步骤。',
  },
]

const tagOptions = ['Blog', 'Article', 'Tech', 'Note', 'Release', 'Changelog', 'MGS']
const selectedTemplateId = ref('article')
const draft = reactive({ title: '', summary: '', tags: [] as string[], content: '' })

const selectedTemplate = computed(() => templates.find(item => item.id === selectedTemplateId.value))
const bodyLines = computed(() => draft.content.split('\n'))

function applyTemplate(template: BlogTemplate) {
  draft.title = template.title
  draft.summary = template.summary
  draft.tags = [...template.tags]
  draft.content = template.content.replaceAll('{{title}}', template.title)
}

function selectTemplate(id: string) {
  const template = templates.find(item => item.id === id)
  if (!template) return
  selectedTemplateId.value = id
  applyTemplate(template)
}

function resetDraft() {
  if (!selectedTemplate.value) return
  applyTemplate(selectedTemplate.value)
  ElMessage.success('已恢复当前模板')
}

function createDraft() {
  ElMessage.success('Demo：已生成博文草稿，下一步可接入现有上传/创建 API')
}

applyTemplate(templates[0])
</script>

<style scoped>
.template-view { height: 100vh; display: flex; flex-direction: column; overflow: hidden; background: var(--el-bg-color-page); }
.header { height: 54px; flex: 0 0 54px; padding: 0 18px; border-bottom: 1px solid var(--el-border-color-light); background: var(--el-bg-color); }
.workspace { min-height: 0; flex: 1; display: grid; grid-template-columns: 290px minmax(0, 1fr); }
.template-panel { min-height: 0; display: flex; flex-direction: column; border-right: 1px solid var(--el-border-color-light); background: var(--el-bg-color); }
.panel-title { display: flex; align-items: center; justify-content: space-between; padding: 18px; border-bottom: 1px solid var(--el-border-color-lighter); }
.title { font-size: 16px; font-weight: 600; }
.subtitle { margin-top: 4px; color: var(--el-text-color-secondary); font-size: 12px; }
.template-list { flex: 1; min-height: 0; overflow: auto; padding: 10px; }
.template-card { width: 100%; display: flex; gap: 12px; padding: 12px; margin-bottom: 8px; text-align: left; border: 1px solid transparent; border-radius: 8px; background: transparent; cursor: pointer; }
.template-card:hover { background: var(--el-fill-color-light); }
.template-card.active { border-color: var(--el-color-primary-light-5); background: var(--el-color-primary-light-9); }
.template-icon { width: 38px; height: 38px; flex: 0 0 38px; display: grid; place-items: center; border-radius: 6px; background: var(--el-fill-color); color: var(--el-color-primary); font-weight: 700; font-family: monospace; }
.template-name { color: var(--el-text-color-primary); font-size: 14px; font-weight: 600; }
.template-description { margin-top: 4px; color: var(--el-text-color-secondary); font-size: 12px; line-height: 1.5; }
.template-tags { display: flex; gap: 4px; margin-top: 7px; }
.panel-tip { display: flex; gap: 7px; padding: 12px 14px; color: var(--el-text-color-secondary); font-size: 12px; line-height: 1.5; border-top: 1px solid var(--el-border-color-lighter); }
.editor-panel { min-width: 0; min-height: 0; display: flex; flex-direction: column; padding: 14px 18px 0; }
.meta-form { flex: 0 0 auto; }
.form-row { display: grid; grid-template-columns: minmax(0, 1.7fr) minmax(240px, 1fr); gap: 14px; }
.meta-form :deep(.el-form-item) { margin-bottom: 12px; }
.sheet { min-height: 0; flex: 1; display: flex; flex-direction: column; overflow: hidden; border: 1px solid var(--el-border-color); border-radius: 6px; background: var(--el-bg-color); }
.sheet-toolbar { display: grid; grid-template-columns: 46px minmax(0, 1fr) minmax(0, 1fr); flex: 0 0 34px; border-bottom: 1px solid var(--el-border-color); background: var(--el-fill-color-light); }
.sheet-cell { display: flex; align-items: center; padding: 0 10px; border-right: 1px solid var(--el-border-color-lighter); color: var(--el-text-color-secondary); font-size: 12px; }
.row-number { justify-content: center; padding: 0; }
.preview-heading { border-right: 0; }
.sheet-body { min-height: 0; flex: 1; display: grid; grid-template-columns: 46px minmax(0, 1fr) minmax(0, 1fr); }
.line-numbers { overflow: hidden; padding-top: 8px; text-align: center; background: var(--el-fill-color-lighter); color: var(--el-text-color-placeholder); font: 13px/22px monospace; user-select: none; }
.markdown-input { min-width: 0; border-right: 1px solid var(--el-border-color); }
.markdown-input :deep(.el-textarea), .markdown-input :deep(.el-textarea__inner) { height: 100%; }
.markdown-input :deep(.el-textarea__inner) { border: 0; border-radius: 0; padding: 8px 12px; resize: none; box-shadow: none; font: 14px/22px 'Cascadia Code PL', monospace; }
.preview { min-width: 0; overflow: auto; padding: 10px 18px; }
.status-bar { height: 30px; flex: 0 0 30px; display: flex; align-items: center; gap: 18px; color: var(--el-text-color-secondary); font-size: 12px; }
.status-bar span:last-child { margin-left: auto; }
.status-ready { color: var(--el-color-success); }
@media (max-width: 900px) {
  .workspace { grid-template-columns: 220px minmax(0, 1fr); }
  .sheet-toolbar, .sheet-body { grid-template-columns: 36px minmax(0, 1fr); }
  .preview, .preview-heading { display: none; }
}
</style>
