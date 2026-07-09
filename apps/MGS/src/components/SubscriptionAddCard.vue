<template>
  <ElCard class="aq-add-card" shadow="hover">
    <ElRow :gutter="16" justify="center">
      <ElCol :span="12" class="button-wrapper">
        <div class="card-button plus-button" @click="handlePlusClick">
          <div class="dashed-circle">
            <el-icon class="plus-icon">
              <Plus />
            </el-icon>
          </div>
          <div v-if="plusLabel" class="label-text">{{ plusLabel }}</div>
        </div>
      </ElCol>

      <ElCol v-if="isDataValid" :span="12" class="button-wrapper">
        <div class="card-button" @click="handleUploadClick">
          <div class="dashed-circle">
            <el-icon class="upload-icon" :style="{ transform: `rotate(${uploadRotation}deg)` }">
              <Upload />
            </el-icon>
          </div>
          <div v-if="uploadLabel" class="label-text">{{ uploadLabel }}</div>
        </div>
      </ElCol>
    </ElRow>
  </ElCard>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { Plus, Upload } from '@element-plus/icons-vue';
import { ElCard, ElCol, ElMessage, ElRow } from 'element-plus';

const props = defineProps<{
  plusLabel?: string,
  uploadLabel?: string,
  dataList?: any[] | null
}>()

const emit = defineEmits(['plusClick', 'uploadClick'])

// 仅保留 Upload 图标的点击旋转角度
const uploadRotation = ref(0);

const isDataValid = computed(() => {
  return Array.isArray(props.dataList) && props.dataList.length > 0;
});

const handlePlusClick = (event: MouseEvent) => {
  // 移除了 plusRotation.value += 45
  emit('plusClick', event);
}

const handleUploadClick = (event: MouseEvent) => {
  // uploadRotation.value += 45;
  emit('uploadClick', event);
}
</script>

<style scoped>
.aq-add-card {
  box-sizing: border-box;
  /* height: calc(100% - 2px); */
  height:100%;
  /* min-height: 331.59px; */
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  transition: border-color 0.3s ease;
}

.button-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
}

.card-button {
  cursor: pointer;
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* 虚线圆形边框实现 */
.dashed-circle {
  width: 64px;
  height: 64px;
  border: 2px dashed var(--el-border-color-darker);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 12px;
  transition: border-color 0.3s ease;
}

.plus-icon, .upload-icon {
  font-size: 28px;
  color: var(--el-text-color-secondary);
  /* 增加 transform 的过渡，让复位和旋转都平滑 */
  transition: color 0.3s ease, transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.label-text {
  font-size: 14px;
  color: var(--el-text-color-regular);
}

/* ================= 悬停交互 ================= */

.card-button:hover .dashed-circle {
  border-color: var(--el-color-primary);
}

.card-button:hover .plus-icon,
.card-button:hover .upload-icon {
  color: var(--el-color-primary);
}

/* 核心改动：专属 Plus 按钮的 Hover 旋转效果 */
/* 鼠标移入时旋转 90 度（可以按需改为 45 或 180） */
.plus-button:hover .plus-icon {
  transform: rotate(45deg);
}
</style>
