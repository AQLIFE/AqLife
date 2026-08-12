<template>
  <ElMenu mode="vertical" router :default-active="route.path" collapse-transition v-for="item, index in viewRoutes" :key="index">
    <ElSubMenu :index="item.path" v-if="item.children && item.meta.showInNav && index==0">
      <template #title>
        <ElIcon>
          <component :is="item.meta?.navIcon" />
        </ElIcon>
        <ElCol :span="11">{{ item.meta?.navTitle }}</ElCol>
      </template>
      <ElMenuItem v-for="innerItem, inerIndex in item.children" :index="item.path + '/' + innerItem.path"
        :key="index + '-' + inerIndex">
        {{ innerItem.path }}
      </ElMenuItem>
    </ElSubMenu>

    <ElMenuItem :index="item.path" v-else-if="index == 1">
      <ElIcon>
        <component :is="item.meta?.navIcon" />
      </ElIcon>
      <ElCol :span="8">{{ item.meta?.navTitle }}</ElCol>
      <ElSegmented v-model="selectStatus" :options="item.children" :props="{ label: 'path', value: 'path' }" @change="handleSegmentedChange(item.path)"/>
    </ElMenuItem>

    <ElMenuItem :index="item.path" v-else>
      <ElIcon>
        <component :is="item.meta?.navIcon" />
      </ElIcon>
      <ElCol :span="8">{{ item.meta?.navTitle }}</ElCol>
    </ElMenuItem>

  </ElMenu>
</template>
<script setup lang="ts">
import { ElMenu,ElRow,ElCol, ElMenuItem, ElIcon, ElSubMenu, ElSegmented } from 'element-plus'
import { routes } from '@/router/routes'
import { useRoute, useRouter } from 'vue-router';
import { ref,computed, watch } from 'vue';
import { List, Document, View } from '@element-plus/icons-vue';
const router = useRouter()
const route = useRoute()

const viewRoutes = computed(() =>
  routes
    .filter(route => route.meta?.showInNav)
    .sort((a, b) => (a.meta?.order ?? 0) - (b.meta?.order ?? 0))
)
const selectStatus = ref('')
function handleSegmentedChange(path: string) {
  router.push(`${path}/${selectStatus.value}`)
}

watch(
  () => route.path,
  (path) => {
    if (path.startsWith('/blog/')) {
      selectStatus.value = path.split('/')[2] ?? ''
    } else {
      selectStatus.value = ''
    }
  },
  { immediate: true }
)
</script>
<style lang="css" scoped></style>
