<template>
  <div class="account-workspace">
    <MgsPageHeader title="Account" description="Manage your profile and the subscription links shown on your public profile." />
    <MgsPageContainer>
    <div class="account-tabs" role="tablist" aria-label="Account sections">
      <RouterLink class="account-tab" :class="{ active: route.path === '/account/profile' }" to="/account/profile"><ElIcon><User /></ElIcon><span>Profile</span><small>个人资料</small></RouterLink>
      <RouterLink class="account-tab" :class="{ active: route.path === '/account/subscriptions' }" to="/account/subscriptions"><ElIcon><Link /></ElIcon><span>Subscriptions</span><small>订阅管理</small></RouterLink>
    </div>
    <section class="account-content"><RouterView /></section>
    </MgsPageContainer>
  </div>
</template>
<script setup lang="ts">
import { ElIcon } from 'element-plus'
import { Link, User } from '@element-plus/icons-vue'
import { useRoute } from 'vue-router'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import MgsPageContainer from '@/components/ui/MgsPageContainer.vue'
const route = useRoute()
</script>
<style scoped>
.account-workspace{min-height:100%}.account-tabs{display:flex;gap:4px;margin:0;padding:4px;width:fit-content;max-width:100%;background:var(--mgs-surface-soft);border:1px solid var(--mgs-border);border-radius:10px}.account-tab{display:grid;grid-template-columns:auto auto;column-gap:8px;row-gap:2px;align-items:center;min-width:150px;padding:9px 12px;border-radius:7px;color:var(--mgs-secondary);text-decoration:none}.account-tab .el-icon{grid-row:1 / 3;font-size:15px}.account-tab span{font-size:13px;font-weight:600}.account-tab small{color:var(--mgs-muted);font-size:10px}.account-tab.active{background:var(--mgs-surface);color:var(--mgs-accent);box-shadow:0 1px 3px rgba(23,25,28,.06)}.account-content{min-width:0;margin:16px 0 0}@media(max-width:720px){.account-tab{min-width:0;flex:1}.account-content{margin-top:12px}}
</style>