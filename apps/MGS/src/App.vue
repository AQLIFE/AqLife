<script setup lang="ts">
import { ElCol, ElMessage, ElRow } from 'element-plus'
import { RouterView, useRoute } from 'vue-router'
import NavMenu from '@/components/NavMenu.vue'
import DataTool from '@/components/DataTool.vue'
import { SidebarType } from '@/types/sidebarType'
import RegisterStep from './components/RegisterStep.vue'
import { computed, onBeforeMount, onMounted } from 'vue'
import EntrancePanel from './components/EntrancePanel.vue'
import { useAccountStore } from '@/stores/useAccountStore'
import { AccountApi } from '@/api'
import { apiConfiguration } from './services/api.ts'
import {ref } from 'vue'

const accountStore = useAccountStore()
const request = new AccountApi(apiConfiguration)
const route = useRoute()

const currentComponent = computed(() => {
  if (route.meta.sidebarType == SidebarType.Register) return RegisterStep
  if (route.meta.sidebarType == SidebarType.Data) return DataTool
})

const lodding = ref<boolean>(true)

onBeforeMount(async () => {
  const response = await request.apiAccountGetRaw()
  if (response.raw.status == 200) {
    const account = await response.value()
    accountStore.systemAccount = account;
    lodding.value =false
  }else if(response.raw.status == 204)
  lodding.value =false
  else{
    ElMessage.error('未配置账户,请进入注册流程')
    lodding.value =false
  }
})
// onMounted(async ()=>{

//    const response = await request.apiAccountGetRaw()
//   if (response.raw.status == 200) {
//     const account = await response.value()
//     accountStore.systemAccount = account;
//     lodding.value=false
//   }
// })
</script>

<template>
  <ElRow v-loading.fullscreen.lock="lodding">
    <ElCol :lg="4" :md="24" class="aside">
      <NavMenu v-if="accountStore.bearerToken" />
      <EntrancePanel v-else />
    </ElCol>
    <ElCol :lg="20" :md="24" class="container">
      <RouterView v-slot="{ Component }">
        <transition>
          <component :is="Component" />
        </transition>
      </RouterView>
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
  display: block;
  height: 100vh;
}
</style>
