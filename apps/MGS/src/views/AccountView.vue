<template>
  <div class="account-workspace">
    <MgsPageHeader title="Account" description="Manage your profile and the subscription links shown on your public profile." />
    <div class="account-tabs" role="tablist" aria-label="Account sections">
      <RouterLink class="account-tab" :class="{ active: route.path === '/account/profile' }" to="/account/profile"><ElIcon><User /></ElIcon><span>Profile</span><small>个人资料</small></RouterLink>
      <RouterLink class="account-tab" :class="{ active: route.path === '/account/subscriptions' }" to="/account/subscriptions"><ElIcon><Link /></ElIcon><span>Subscriptions</span><small>订阅管理</small></RouterLink>
    </div>
    <section class="account-content"><RouterView /></section>
  </div>
</template>
<script setup lang="ts">
import { ElIcon } from 'element-plus'
import { Link, User } from '@element-plus/icons-vue'
import { useRoute } from 'vue-router'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
const route = useRoute()
</script>
<style scoped>
.account-workspace{min-height:100%;padding-bottom:28px}.account-tabs{display:flex;gap:4px;margin:0 28px;padding:4px;width:fit-content;max-width:calc(100% - 56px);background:var(--mgs-surface-soft);border:1px solid var(--mgs-border);border-radius:10px}.account-tab{display:grid;grid-template-columns:auto auto;column-gap:8px;row-gap:2px;align-items:center;min-width:150px;padding:9px 12px;border-radius:7px;color:var(--mgs-secondary);text-decoration:none}.account-tab .el-icon{grid-row:1 / 3;font-size:15px}.account-tab span{font-size:13px;font-weight:600}.account-tab small{color:var(--mgs-muted);font-size:10px}.account-tab.active{background:var(--mgs-surface);color:var(--mgs-accent);box-shadow:0 1px 3px rgba(23,25,28,.06)}.account-content{min-width:0;margin:16px 28px 0}@media(max-width:720px){.account-tabs{width:calc(100% - 24px);max-width:none;margin:0 12px}.account-tab{min-width:0;flex:1}.account-content{margin:12px 12px 0}}
</style>