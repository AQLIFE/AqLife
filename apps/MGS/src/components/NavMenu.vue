<template>
  <ElMenu mode="vertical" router collapse-transition v-for="item,index in routes" :key="index">
    <el-sub-menu :index="item.path" v-if="item.children && item.meta.showInNav">
      <template #title>
        <el-icon><component :is="item.meta?.navIcon" /></el-icon>
        <span>{{ item.meta?.navTitle }}</span>
      </template>
      <el-menu-item v-for="innerItem,inerIndex in item.children" :index="item.path +'/'+innerItem.path" :key="index-inerIndex">{{ innerItem.path }}</el-menu-item>
    </el-sub-menu>

    <el-menu-item :index="item.path" v-else-if="item.meta?.showInNav">
      <el-icon><component :is="item.meta?.navIcon" /></el-icon>
      <span>{{ item.meta?.navTitle }}</span>
    </el-menu-item>

  </ElMenu>
</template>
<script setup lang="ts">
import { ElMenu, ElMenuItem, ElIcon, ElSubMenu } from 'element-plus'
import { routes } from '@/router/routes'
</script>
<style lang="css" scoped></style>
