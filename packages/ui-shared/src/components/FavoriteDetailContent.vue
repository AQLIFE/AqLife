<script setup lang="ts">
import { computed } from 'vue'
import { ElButton, ElImage, ElTag } from 'element-plus'
import { Edit } from '@element-plus/icons-vue'
import type { FavoriteDto, TagDto } from '@aqlife/api-client'

const props = withDefaults(defineProps<{
  favorite: FavoriteDto
  tags?: TagDto[]
  imageUrls?: Record<string, string>
  editable?: boolean
}>(), {
  tags: () => [],
  imageUrls: () => ({}),
  editable: false,
})

defineEmits<{ edit: [] }>()

const typeOptions = [
  { value: 'Food', label: '食物' },
  { value: 'Book', label: '书籍' },
  { value: 'Game', label: '游戏' },
  { value: 'Tech', label: '技术' },
  { value: 'Life', label: '生活' },
  { value: 'Other', label: '其他' },
]
const mediaItems = computed(() =>
  [...(props.favorite.media ?? [])]
    .filter(media => !!media.fileId)
    .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0)),
)
const galleryUrls = computed(() =>
  mediaItems.value
    .map(media => media.fileId ? props.imageUrls[media.fileId] : undefined)
    .filter((url): url is string => !!url),
)
const favoriteTags = computed(() =>
  (props.favorite.tagIds ?? [])
    .map(id => props.tags.find(tag => tag.uid === id)?.name)
    .filter((name): name is string => !!name),
)
const contentTypeLabel = computed(() =>
  typeOptions.find(type => type.value === props.favorite.contentType)?.label
    ?? props.favorite.contentType
    ?? '收藏',
)

function formatDate(value?: string) {
  return value?.slice(0, 10) ?? ''
}
</script>

<template>
  <article class="favorite-detail-content">
    <header class="detail-header">
      <div class="detail-heading-row">
        <div class="detail-eyebrow">
          <span>{{ contentTypeLabel }}</span>
          <span v-if="favorite.createdAt">{{ formatDate(favorite.createdAt) }}</span>
        </div>
        <ElButton v-if="editable" type="primary" :icon="Edit" @click="$emit('edit')">编辑收藏</ElButton>
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
          <div v-else class="gallery-placeholder">图片暂不可用</div>
          <span v-if="media.role === 'Cover'" class="gallery-label">封面</span>
          <span v-else class="gallery-label">图片 {{ index + 1 }}</span>
        </div>
      </div>
    </section>

    <section class="detail-info">
      <div class="section-heading"><h2>收藏信息</h2></div>
      <dl>
        <div><dt>内容类型</dt><dd>{{ contentTypeLabel }}</dd></div>
        <div v-if="favorite.createdAt"><dt>添加时间</dt><dd>{{ formatDate(favorite.createdAt) }}</dd></div>
        <div v-if="favorite.updatedAt"><dt>更新时间</dt><dd>{{ formatDate(favorite.updatedAt) }}</dd></div>
        <div v-if="favorite.sourceUrl">
          <dt>来源地址</dt>
          <dd><a :href="favorite.sourceUrl" target="_blank" rel="noopener noreferrer">{{ favorite.sourceUrl }}</a></dd>
        </div>
        <div><dt>发布状态</dt><dd>{{ favorite.isPublished ? '已发布' : '未发布' }}</dd></div>
      </dl>
    </section>
  </article>
</template>

<style scoped>
.favorite-detail-content { width: 100%; color: var(--el-text-color-primary); }
.detail-header { padding: 10px 0 28px; border-bottom: 1px solid var(--el-border-color-lighter); }
.detail-heading-row { display: flex; justify-content: space-between; align-items: center; gap: 16px; }
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
@media (max-width: 700px) { .detail-gallery { grid-template-columns: repeat(2, minmax(0, 1fr)); gap: 9px; } }
@media (max-width: 420px) { .detail-gallery { grid-template-columns: minmax(0, 1fr); } .detail-info dl > div { grid-template-columns: 84px minmax(0, 1fr); } }
</style>
