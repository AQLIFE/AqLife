<script setup lang="ts">
import { ElCol, ElRow } from 'element-plus'
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
  console.log(route.meta.sidebarType)
  if (route.meta.sidebarType == SidebarType.Register) return RegisterStep
  if (route.meta.sidebarType == SidebarType.Data)return DataTool
})

onBeforeMount(async ()=>{
  const account = await request.apiAccountGet()
  accountStore.systemAccount = account;
  console.log(account?'系统已完成初始化':'系统等待初始化')
})
</script>

<template>
  <ElRow>
    <ElCol :lg="4" :md="24" class="aside">
      <NavMenu v-if="accountStore.bearerToken"/>
      <EntrancePanel v-else />
    </ElCol>
    <ElCol :lg="20" :md="24" class="container">
      <ElCol v-if="!(route.meta.sidebarType == SidebarType.Home)">
        <component :is="currentComponent" />
      </ElCol>
      <ElCol>
        <RouterView></RouterView>
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
  flex-direction: row;
  flex-wrap: wrap;
}
.container .el-col:nth-child(1){
  height:fit-content;
}
.container .el-col:nth-child(2) {
  display: inline-flex;
  align-items: center;
  height: fit-content;
  flex-grow: 1;
}
</style>
