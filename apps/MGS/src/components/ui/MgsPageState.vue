<template>
  <section class="page-state" :class="{ compact }" role="status" aria-live="polite">
    <div v-if="state === 'loading'" class="state-content">
      <ElIcon class="loading-icon"><Loading /></ElIcon>
      <strong>{{ title || '正在加载...' }}</strong>
      <p v-if="description">{{ description }}</p>
    </div>

    <ElEmpty v-else-if="state === 'empty'" :description="description || '暂无数据'">
      <slot />
    </ElEmpty>

    <div v-else class="state-content error-state" role="alert">
      <ElIcon class="error-icon"><CircleCloseFilled /></ElIcon>
      <strong>{{ title || '加载失败' }}</strong>
      <p>{{ description || '暂时无法获取数据，请检查网络后重试。' }}</p>
      <ElButton v-if="retryable" type="primary" @click="$emit('retry')">
        {{ retryLabel }}
      </ElButton>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ElButton, ElEmpty, ElIcon } from 'element-plus'
import { CircleCloseFilled, Loading } from '@element-plus/icons-vue'

withDefaults(defineProps<{
  state: 'loading' | 'empty' | 'error'
  title?: string
  description?: string
  retryable?: boolean
  retryLabel?: string
  compact?: boolean
}>(), {
  retryable: false,
  retryLabel: '重试',
  compact: false,
})

defineEmits<{
  retry: []
}>()
</script>

<style scoped>
.page-state {
  display: grid;
  min-height: 260px;
  place-items: center;
  padding: 28px 16px;
  color: var(--mgs-secondary);
}

.page-state.compact {
  min-height: 150px;
  padding: 12px;
}

.state-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  text-align: center;
}

.state-content strong {
  color: var(--mgs-text);
  font-size: 14px;
}

.state-content p {
  max-width: 440px;
  margin: 0;
  color: var(--mgs-muted);
  font-size: 12px;
  line-height: 1.6;
}

.loading-icon,
.error-icon {
  font-size: 28px;
}

.loading-icon {
  color: var(--mgs-accent);
  animation: page-state-spin 1s linear infinite;
}

.error-icon {
  color: var(--mgs-danger);
}

@keyframes page-state-spin {
  to { transform: rotate(360deg); }
}
</style>
