<template>
  <ElCard class="blog-card" shadow="never" @click="handleClick">
    <div class="blog-card-content">

      <div class="blog-card-type">
        .MD
      </div>

      <h2 class="blog-card-title">
        {{ blog.fileName || '未命名文章' }}
      </h2>

      <p class="blog-card-introduction">
        {{ introduction }}
      </p>

      <div v-if="blog.tags?.length" class="blog-card-tags">
        <ElTag v-for="(tag, index) in blog.tags" :key="index" size="small" effect="plain" :type="tag.isCategory
          ? 'success'
          : 'info'
          ">
          {{ tag.name }}
        </ElTag>
      </div>

      <footer class="blog-card-footer">

        <div class="blog-card-date">
          <ElIcon>
            <Clock />
          </ElIcon>

          <span>
            {{ formatDate(blog.uploadTime) }}
          </span>
        </div>

        <div class="blog-card-meta">

          <span>
            <ElIcon>
              <View />
            </ElIcon>

            {{ blog.viewCount }}
          </span>

          <ElIcon class="blog-card-arrow">
            <ArrowRight />
          </ElIcon>

        </div>

      </footer>

    </div>
  </ElCard>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

import {
  ArrowRight,
  Clock,
  View,
} from '@element-plus/icons-vue'

import {
  ElCard,
  ElIcon,
  ElTag,
} from 'element-plus'

import type { FileDto } from '@/api'
const props = defineProps<{
  blog: FileDto
}>()

const router = useRouter()

const demo =
  '暂无文章简介，点击查看文章内容。'

const introduction = computed(() => {
  const value =
    props.blog.fileIntroduction?.trim()

  return value || demo
})

function formatDate(
  value?: string | null,
) {
  if (!value) {
    return ''
  }

  return value.slice(0, 10)
}

function handleClick() {
  router.push(
    `/preview/${props.blog.uid}`,
  )
}
</script>

<style scoped>
.blog-card {
  position: relative;
  overflow: hidden;
  cursor: pointer;
  min-height:230px;

  border-radius: 7px;
  border: 1px solid var(--el-border-color-lighter);
  background: var(--el-bg-color);

  transition:
    transform 0.2s ease,
    border-color 0.2s ease,
    box-shadow 0.2s ease;
}

.blog-card:hover {
  transform: translateY(-2px);

  border-color: var(--el-border-color);

  box-shadow:
    0 6px 18px rgb(0 0 0 / 5%);
}

.blog-card :deep(.el-card__body) {
  /* height: 100%; */
  padding: 0;
}

.blog-card-content {
  /* min-height: 300px; */

  display: flex;
  flex-direction: column;

  padding: 22px 24px 16px;
  box-sizing: border-box;
}

.blog-card-type {
  margin-bottom: 8px;

  color: var(--el-color-primary);

  font-size: 11px;
  font-weight: 600;

  letter-spacing: 0.08em;
}

.blog-card-title {
  margin: 0 0 12px;

  color: var(--el-text-color-primary);

  font-size: 18px;
  font-weight: 650;

  line-height: 1.45;

  display: -webkit-box;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 2;

  overflow: hidden;
}

.blog-card-introduction {
  margin: 0;

  color: var(--el-text-color-secondary);

  font-size: 13px;
  line-height: 1.75;

  display: -webkit-box;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 3;

  overflow: hidden;
}

.blog-card-tags {
  display: flex;

  flex-wrap: wrap;

  gap: 5px;

  padding-top: 16px;
}

.blog-card-tags :deep(.el-tag) {
  border-radius: 4px;
}

.blog-card-footer {
  display: flex;

  align-items: center;
  justify-content: space-between;

  margin-top: auto;

  padding-top: 11px;

  border-top:
    1px solid var(--el-border-color-lighter);

  color: var(--el-text-color-secondary);

  font-size: 11px;
}

.blog-card-date {
  display: flex;

  align-items: center;

  gap: 5px;
}

.blog-card-meta {
  display: flex;

  align-items: center;

  gap: 12px;
}

.blog-card-meta>span {
  display: flex;

  align-items: center;

  gap: 5px;
}

.blog-card-arrow {
  color: var(--el-text-color-placeholder);

  transition:
    color 0.2s ease,
    transform 0.2s ease;
}

.blog-card:hover .blog-card-arrow {
  color: var(--el-color-primary);

  transform: translateX(3px);
}
</style>