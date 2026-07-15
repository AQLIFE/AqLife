<script setup lang="ts">
import { ElCol, ElMessage, ElRow } from 'element-plus'
import { RouterView, useRoute } from 'vue-router'
import NavMenu from '@/components/NavMenu.vue'
import DataTool from '@/components/DataTool.vue'
import { SidebarType } from '@/types/sidebarType'
import RegisterStep from './components/RegisterStep.vue'
import { computed, onBeforeMount } from 'vue'
import EntrancePanel from './components/EntrancePanel.vue'
import { useAccountStore } from '@/stores/useAccountStore'
import { AccountApi } from '@/api'
import { apiConfiguration } from './services/api.ts'

const accountStore = useAccountStore()
const request = new AccountApi(apiConfiguration)
const route = useRoute()

const currentComponent = computed(() => {
  if (route.meta.sidebarType == SidebarType.Register) return RegisterStep
  if (route.meta.sidebarType == SidebarType.Data) return DataTool
})

onBeforeMount(async () => {
  const response = await request.apiAccountGetRaw()
  if (response.raw.status == 200) {
    const account =await response.value()
    accountStore.systemAccount = account;
  }
})
</script>

<template>
  <ElRow>
    <ElCol :lg="4" :md="24" class="aside">
      <NavMenu v-if="accountStore.bearerToken" />
      <EntrancePanel v-else />
    </ElCol>
    <ElCol :lg="20" :md="24" class="container">
      <ElCol v-if="!(route.meta.sidebarType == SidebarType.Home)">
        <component :is="currentComponent" />
      </ElCol>
      <ElCol>
        <RouterView v-slot="{ Component }">
          <transition>
            <component :is="Component"/>
          </transition>
        </RouterView>
      </ElCol>
    </ElCol>
  </ElRow>
</template>

<style lang="css" scoped>
.aside {
  border-right: 1px dotted gainsboro;
  align-content: center;
  height: 100vh;
}

.container {
  display: flex;
  height: 100vh;
  flex-direction: column;
  overflow: hidden;
  flex-wrap: nowrap;
  /* 3. 禁止换行，确保高度计算准确 */
}

.container>.el-col:nth-child(1) {
  flex: none;
  /* 1. 禁止伸缩：它只占据内容所需的物理高度 */
  height: auto;
  /* 2. 移除任何强制高度，由内容（按钮等）撑开 */
}

.container>.el-col:nth-child(2) {
  flex: 1;
  /* 4. 核心：占据剩余所有垂直空间 */
  height: 0;
  /* 5. 技巧：设置高度为0配合flex:1，强制子元素忽略自身内容高度 */
  display: flex;
  flex-direction: column;
  overflow: hidden;
  /* 6. 防止内容溢出容器 */
}

.flex{
  display: flex;
  height:inherit;
  justify-content: center;
  align-items: center;
}
</style>
