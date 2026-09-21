<template>
  <div class="author-card">
    <div class="profile">
      <ElImage v-if="userInfo?.avatar" :src="previewRequest(userInfo.avatar)" class="avatar">
        <template #error>
          <ElIcon class="avatar-error">
            <Picture />
          </ElIcon>
        </template>
      </ElImage>

      <div class="author-info">
        <div class="author-name">
          {{ userInfo?.name }}
        </div>

        <div class="author-desc">
          {{ userInfo?.desc }}
        </div>
      </div>
    </div>

    <div v-if="userInfo?.subscriptions?.length" class="subscriptions">
      <ElLink v-for="item in userInfo.subscriptions" :key="item.subscriptionPlatform!" class="subscription"
        target="_blank" underline="never" :href="item.subscriptionLink ?? ''">
        <ElImage :src="item.subscriptionIcon
            ? previewRequest(item.subscriptionIcon)
            : ''
          " class="subscription-icon">
          <template #error>
            <ElIcon>
              <Picture />
            </ElIcon>
          </template>
        </ElImage>
      </ElLink>
    </div>
  </div>
</template>

<script lang="ts" setup>
import {
  ElImage,
  ElIcon,
  ElLink,
} from 'element-plus'

import {
  Picture,
} from '@element-plus/icons-vue'

import {
  storeToRefs,
} from 'pinia'

import {
  useAuthorInfoStore,
} from '@/stores/useAuthorInfoStore'

const authorInfoStore = useAuthorInfoStore()

const { userInfo } =
  storeToRefs(authorInfoStore)

const previewRequest = (guid: string) =>
  `${import.meta.env.VITE_API}/api/file/preview?title=&uid=${guid}`
</script>

<style scoped>
.author-card {
  width: 100%;

  min-width: 0;
  min-height: 0;

  display: flex;
  flex-direction: column;

  overflow: hidden;

  background-color: var(--topColor);
}


/* =========================
   用户信息
   ========================= */

.profile {
  display: grid;

  grid-template-columns: 72px minmax(0, 1fr);

  gap: 16px;

  align-items: center;

  padding: 20px;
}


/* 头像 */

.avatar {
  width: 72px;
  height: 72px;

  flex-shrink: 0;

  overflow: hidden;
}

.avatar :deep(.el-image__inner) {
  width: 100%;
  height: 100%;

  object-fit: cover;
}

.avatar-error {
  width: 100%;
  height: 100%;

  display: flex;

  align-items: center;
  justify-content: center;

  font-size: 36px;
}


/* 用户信息 */

.author-info {
  min-width: 0;

  display: flex;
  flex-direction: column;

  gap: 8px;
}

.author-name {
  overflow: hidden;

  font-size: 20px;
  font-weight: 600;

  text-overflow: ellipsis;
  white-space: nowrap;
}

.author-desc {
  overflow: hidden;

  color: var(--back_color_lv5);

  font-size: 13px;

  line-height: 1.5;

  display: -webkit-box;
  -webkit-box-orient: vertical;
  -webkit-line-clamp: 2;
}


/* =========================
   订阅链接
   ========================= */

.subscriptions {
  min-width: 0;

  display: flex;
  flex-wrap: wrap;

  gap: 12px;

  padding: 0 20px 20px;
}

.subscription {
  width: 36px;
  height: 36px;

  display: flex;

  align-items: center;
  justify-content: center;

  flex-shrink: 0;
}

.subscription-icon {
  width: 28px;
  height: 28px;
}

.subscription-icon :deep(.el-image__inner) {
  width: 100%;
  height: 100%;

  object-fit: contain;
}
</style>