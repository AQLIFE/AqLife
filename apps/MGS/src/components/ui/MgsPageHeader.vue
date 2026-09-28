<template>
  <div class="page-header">
    <div class="heading">
      <div v-if="back" class="back">
        <ElButton text :icon="ArrowLeft" @click="handleBack">Back</ElButton>
      </div>
      <div>
        <h1>{{ title }}</h1>
        <p v-if="description">{{ description }}</p>
      </div>
    </div>
    <div v-if="$slots.actions" class="actions">
      <slot name="actions" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ElButton } from 'element-plus'
import { ArrowLeft } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'

withDefaults(defineProps<{
  title: string
  description?: string
  back?: boolean
}>(), { back: false })

const router = useRouter()
const handleBack = () => router.back()
</script>

<style scoped>
.page-header { display:flex;align-items:flex-start;justify-content:space-between;gap:24px;padding:24px 28px 18px; }
.heading { display:flex;align-items:flex-start;gap:14px; }
.heading h1 { margin:0;font-size:22px;line-height:1.25;letter-spacing:-.02em;font-weight:650;color:var(--mgs-text); }
.heading p { margin:6px 0 0;color:var(--mgs-muted);font-size:12px; }
.back { margin-top:-4px; }
.actions { display:flex;align-items:center;gap:8px; }
</style>
