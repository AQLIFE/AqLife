<template>
  <el-card class="aq-add-card" shadow="hover" @click="$emit('click')">
    <div class="content-wrapper">
      <!-- 虚线圆形边框容器 -->
      <div class="dashed-circle">
        <el-icon class="plus-icon"><Plus /></el-icon>
      </div>
      <div v-if="label" class="label-text">{{ label }}</div>
    </div>
  </el-card>
</template>

<script setup lang="ts">

// 遵循你之前建立的“类型契约”习惯 [cite: 181, 210]
defineProps<{
  label?: string
}>()

defineEmits(['click'])
</script>

<style scoped>
.aq-add-card {
  cursor: pointer;
  /* 保持与你之前 Subscription 列表一致的宽高比 [cite: 207] */
  min-height: 180px; 
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  border: 1px solid var(--el-border-color-lighter);
}

/* 核心居中容器 */
.content-wrapper {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

/* 虚线圆形边框实现 */
.dashed-circle {
  width: 64px;
  height: 64px;
  border: 2px dashed var(--el-border-color-darker);
  border-radius: 50%; /* 圆形 */
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 12px;
  transition: all 0.3s ease;
}

.plus-icon {
  font-size: 28px;
  color: var(--el-text-color-secondary);
  transition: all 0.3s ease;
}

.label-text {
  font-size: 14px;
  color: var(--el-text-color-regular);
}

/* 悬停交互效果：体现“工业级”精致感 */
.aq-add-card:hover {
  border-color: var(--el-color-primary);
  background-color: var(--el-color-primary-light-9);
}

.aq-add-card:hover .dashed-circle {
  border-color: var(--el-color-primary);
  border-style: solid; /* 悬停时虚线转实线，增强反馈 */
  transform: scale(1.1);
}

.aq-add-card:hover .plus-icon {
  color: var(--el-color-primary);
  transform: rotate(90deg); /* 增加一个旋转动画，更显灵动 */
}
</style>