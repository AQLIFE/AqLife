<script setup lang="ts">
import { computed, onBeforeMount, onBeforeUnmount, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElButton, ElEmpty, ElMessage, ElOption, ElSelect, ElTag } from 'element-plus'
import { ArrowRight, Refresh } from '@element-plus/icons-vue'
import { FavoriteApi, FileApi, TagApi, type FavoriteDto, type TagDto } from '@/api'
import { apiConfiguration } from '@/services/api'

const router = useRouter()
const favoriteApi = new FavoriteApi(apiConfiguration)
const fileApi = new FileApi(apiConfiguration)
const tagApi = new TagApi(apiConfiguration)

const items = ref<FavoriteDto[]>([])
const tags = ref<TagDto[]>([])
const loading = ref(false)
const hasMore = ref(false)
const page = ref(1)
const selectedType = ref('')
const selectedTag = ref('')
const imageUrls = ref<Record<string, string>>({})
const objectUrls = new Set<string>()

const typeOptions = [
  { value: 'Food', label: '食物' },
  { value: 'Book', label: '书籍' },
  { value: 'Game', label: '游戏' },
  { value: 'Tech', label: '技术' },
  { value: 'Life', label: '生活' },
  { value: 'Other', label: '其他' },
]

const tagName = (id: string) => tags.value.find(tag => tag.uid === id)?.name ?? '标签'

async function loadItems(reset = false) {
  if (loading.value) return
  if (reset) {
    page.value = 1
    items.value = []
  }
  loading.value = true
  try {
    const result = await favoriteApi.apiFavoriteGet({
      page: page.value,
      pageSize: 24,
      contentType: selectedType.value || undefined,
      tagId: selectedTag.value || undefined,
    })
    const received = result.items ?? []
    items.value = reset ? received : [...items.value, ...received]
    hasMore.value = result.hasMore ?? false
    page.value = (result.page ?? page.value) + 1
    await loadImages(received)
  } catch (error) {
    ElMessage.warning(error instanceof Error ? error.message : '收藏内容加载失败')
  } finally {
    loading.value = false
  }
}

async function loadImages(favorites: FavoriteDto[]) {
  await Promise.all(favorites.map(async item => {
    const uid = item.uid
    const media = (item.media ?? []).find(value => value.fileId && (value.role === 'Cover' || value.role === 'Image'))
    if (!uid || !media?.fileId || imageUrls.value[uid]) return
    try {
      const blob = await fileApi.apiFilePreviewGet({ uID: media.fileId })
      const url = URL.createObjectURL(blob)
      objectUrls.add(url)
      imageUrls.value[uid] = url
    } catch {
      // 收藏条目仍保留文字内容；文件可能已被删除或不再公开。
    }
  }))
}

function openDetail(uid?: string) {
  if (uid) void router.push('/favorite/' + encodeURIComponent(uid))
}

function changeType(value: string) {
  selectedType.value = value
  void loadItems(true)
}
function changeTag(value: string) {
  selectedTag.value = value
  void loadItems(true)
}

onBeforeMount(async () => {
  try {
    const result = await tagApi.apiTagGet({ page: 1, pageSize: 100 })
    tags.value = result.items ?? []
  } catch {
    // 标签筛选不可用时仍可浏览全部收藏。
  }
  await loadItems(true)
})

onBeforeUnmount(() => {
  for (const url of objectUrls) URL.revokeObjectURL(url)
  objectUrls.clear()
})
</script>

<template>
  <section class="archive-page">
    <header class="archive-heading">
      <div>
        <p class="eyebrow">PERSONAL ARCHIVE</p>
        <h1>一些留存</h1>
        <p class="intro">食物、书、游戏、工具，以及那些值得留下来的小东西。</p>
      </div>
      <span class="item-count">{{ items.length }} 件</span>
    </header>

    <div class="archive-toolbar">
      <ElSelect :model-value="selectedType" clearable placeholder="所有类别" @change="changeType">
        <ElOption v-for="type in typeOptions" :key="type.value" :label="type.label" :value="type.value" />
      </ElSelect>
      <ElSelect :model-value="selectedTag" clearable filterable placeholder="所有标签" @change="changeTag">
        <ElOption v-for="tag in tags" :key="tag.uid ?? tag.name ?? 'tag'" :label="tag.name ?? '未命名标签'" :value="tag.uid ?? ''" />
      </ElSelect>
      <ElButton text :icon="Refresh" :loading="loading" @click="loadItems(true)">刷新</ElButton>
    </div>

    <div v-if="items.length" class="archive-grid">
      <article v-for="(item, index) in items" :key="item.uid" class="archive-card" role="link" tabindex="0" aria-label="查看收藏详情" @click="openDetail(item.uid)" @keydown.enter.prevent="openDetail(item.uid)" :class="`card-tone-${index % 4}`">
        <a v-if="item.sourceUrl" class="card-source" :href="item.sourceUrl" target="_blank" rel="noopener noreferrer" :aria-label="打开收藏来源" @click.stop>
          <ArrowRight />
        </a>
        <div v-if="imageUrls[item.uid ?? '']" class="card-image">
          <img :src="imageUrls[item.uid ?? '']" :alt="item.title ?? '收藏图片'" loading="lazy">
        </div>
        <div class="card-body">
          <div class="card-meta">
            <span>{{ typeOptions.find(type => type.value === item.contentType)?.label ?? item.contentType }}</span>
            <span v-if="item.createdAt">{{ item.createdAt.slice(0, 10) }}</span>
          </div>
          <h2>{{ item.title || '未命名收藏' }}</h2>
          <p v-if="item.description">{{ item.description }}</p>
          <div v-if="item.tagIds?.length" class="card-tags">
            <ElTag v-for="id in item.tagIds" :key="id" size="small" effect="plain">{{ tagName(id) }}</ElTag>
          </div>
        </div>
      </article>
    </div>
    <ElEmpty v-else-if="!loading" description="这里还没有公开的收藏" />
    <div class="archive-footer">
      <span v-if="loading">正在整理收藏…</span>
      <ElButton v-else-if="hasMore" plain @click="loadItems()">加载更多</ElButton>
      <span v-else-if="items.length">已经到底了</span>
    </div>
  </section>
</template>

<style scoped>
.archive-page { height: 100%; min-height: 0; overflow: auto; padding: 34px clamp(16px, 4vw, 48px) 48px; box-sizing: border-box; background: var(--el-bg-color); color: var(--el-text-color-primary); }
.archive-heading { display: flex; align-items: flex-end; justify-content: space-between; gap: 20px; max-width: 1120px; margin: 0 auto 28px; }
.eyebrow { margin: 0 0 8px; color: var(--el-color-primary); font-size: 11px; font-weight: 700; letter-spacing: .16em; }
.archive-heading h1 { margin: 0; font-family: Georgia, "Noto Serif SC", serif; font-size: clamp(28px, 4vw, 42px); font-weight: 500; letter-spacing: .02em; }
.intro { margin: 12px 0 0; color: var(--el-text-color-secondary); font-size: 14px; line-height: 1.8; }
.item-count { color: var(--el-text-color-placeholder); font-size: 12px; white-space: nowrap; padding-bottom: 5px; }
.archive-toolbar { display: flex; flex-wrap: wrap; gap: 10px; align-items: center; max-width: 1120px; margin: 0 auto 22px; }
.archive-toolbar :deep(.el-select) { width: 160px; }
.archive-grid { max-width: 1120px; margin: 0 auto; display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: 16px; align-items: start; }
.archive-card { position: relative; min-width: 0; overflow: hidden; border: 1px solid var(--el-border-color-lighter); border-radius: 8px; background: var(--el-fill-color-blank); transition: transform .2s ease, box-shadow .2s ease; }
.archive-card:hover, .archive-card:focus-visible { transform: translateY(-3px); box-shadow: 0 10px 28px rgb(0 0 0 / 7%); outline: none; cursor: pointer; }
.card-tone-1 { margin-top: 22px; }
.card-tone-2 { margin-top: 8px; }
.card-source { position: absolute; z-index: 1; top: 10px; right: 10px; display: grid; place-items: center; width: 30px; height: 30px; border-radius: 50%; background: rgb(255 255 255 / 88%); color: #444; }
.card-image { aspect-ratio: 4 / 3; overflow: hidden; background: var(--el-fill-color-light); }
.card-image img { display: block; width: 100%; height: 100%; object-fit: cover; transition: transform .35s ease; }
.archive-card:hover .card-image img { transform: scale(1.035); }
.card-body { padding: 16px 17px 18px; }
.card-meta { display: flex; justify-content: space-between; gap: 10px; color: var(--el-text-color-placeholder); font-size: 11px; }
.card-body h2 { margin: 10px 0 8px; font-size: 17px; font-weight: 600; line-height: 1.45; }
.card-body p { margin: 0; color: var(--el-text-color-secondary); font-size: 13px; line-height: 1.75; white-space: pre-wrap; }
.card-tags { display: flex; flex-wrap: wrap; gap: 5px; margin-top: 13px; }
.archive-footer { display: flex; justify-content: center; padding: 28px 0 8px; color: var(--el-text-color-placeholder); font-size: 12px; }
@media (max-width: 900px) { .archive-grid { grid-template-columns: repeat(2, minmax(0, 1fr)); } .card-tone-1, .card-tone-2 { margin-top: 0; } }
@media (max-width: 600px) { .archive-page { padding: 22px 14px 32px; } .archive-grid { grid-template-columns: minmax(0, 1fr); gap: 12px; } .archive-heading { margin-bottom: 20px; } }
@media (prefers-reduced-motion: reduce) { .archive-card, .card-image img { transition: none; } .archive-card:hover { transform: none; } }
</style>
