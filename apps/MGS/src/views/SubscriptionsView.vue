<template>
  <div class="subscriptions-page">
    <div class="subscriptions-toolbar"><div><h2>Subscriptions</h2><p>管理你的公开订阅入口，它们会显示在个人主页中。</p></div><ElButton :icon="Plus" @click="add">Add subscription</ElButton></div>
    <div v-if="subscriptDtos?.length" class="subscription-grid">
      <ScriptionCard v-for="(item,index) in subscriptDtos" :key="index" :serial="index" @delete="remove" :item="item" v-model:file="files[index]" />
      <SubscriptionAddCard class="add-card" @plusClick="add" @uploadClick="update" :data-list="subscriptDtos" />
    </div>
    <ElEmpty v-else description="还没有订阅入口"><ElButton :icon="Plus" @click="add">Add subscription</ElButton></ElEmpty>
    <div v-if="subscriptDtos?.length" class="save-bar"><span>修改订阅信息或图标后保存。</span><ElButton type="primary" :loading="saving" @click="update">Save subscriptions</ElButton></div>
  </div>
</template>
<script setup lang="ts">
import { computed, ref } from 'vue'
import { ElButton, ElEmpty, ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { useAccountStore } from '@/stores/useAccountStore'
import { AccountApi, FileApi, type SubscriptionDto } from '@/api'
import SubscriptionAddCard from '@/components/SubscriptionAddCard.vue'
import ScriptionCard from '@/components/ScriptionViewCard.vue'
import { apiConfiguration } from '@/services/api'
const accountStore=useAccountStore(),subscriptDtos=computed(()=>accountStore.systemAccount?.subscriptions),files=ref<(File|null)[]>([]),saving=ref(false)
function remove(index:number){const subscriptions=accountStore.systemAccount?.subscriptions;if(!subscriptions||index<0||index>=subscriptions.length)return;subscriptions.splice(index,1);files.value.splice(index,1);ElMessage.info('已删除第 '+(index+1)+' 个订阅')}
function add(){accountStore.systemAccount?.subscriptions?.push({subscriptionLink:'',subscriptionPlatform:'',aliasName:'',subscriptionIcon:''});files.value.push(null)}
async function update(){if(!subscriptDtos.value||!valid(files.value,subscriptDtos.value))return;saving.value=true;const fileApi=new FileApi(apiConfiguration),accountApi=new AccountApi(apiConfiguration);try{const uploadTasks=files.value.map(async(item,index)=>{if(!item)return;const iconGuids=await fileApi.apiFileUploadPost({file:[item]});if(iconGuids?.length)subscriptDtos.value![index].subscriptionIcon=iconGuids[0]});await Promise.all(uploadTasks);await accountApi.apiAccountSubscriptionsPut({subscriptionDto:subscriptDtos.value});accountStore.systemAccount=await accountApi.apiAccountGet();files.value=new Array(subscriptDtos.value.length).fill(null);ElMessage.success('Subscriptions updated')}catch(error){ElMessage.error(error instanceof Error?error.message:'Subscription update failed')}finally{saving.value=false}}
function valid(files:Array<File|null>,subscriptionDtos:SubscriptionDto[]){if(files.length!==subscriptionDtos.length){ElMessage.warning('你有尚未完成的修改');return false}for(let i=0;i<files.length;i++){if(files[i]===null){ElMessage.warning('第 '+(i+1)+' 个订阅缺少图像标识');return false}}return true}
</script>
<style scoped>
.subscriptions-page{max-width:1100px}.subscriptions-toolbar{display:flex;align-items:flex-start;justify-content:space-between;gap:20px;margin-bottom:16px}.subscriptions-toolbar h2{margin:0;font-size:16px}.subscriptions-toolbar p{margin:5px 0 0;color:var(--mgs-muted);font-size:12px}.subscription-grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(250px,1fr));gap:14px;align-items:stretch}.subscription-grid :deep(.el-card){height:100%}.add-card{min-height:300px}.save-bar{display:flex;align-items:center;gap:12px;margin-top:16px;padding:12px 14px;background:var(--mgs-surface);border:1px solid var(--mgs-border);border-radius:9px;color:var(--mgs-muted);font-size:12px}.save-bar .el-button{margin-left:auto}@media(max-width:640px){.subscriptions-toolbar{flex-direction:column}.save-bar{flex-direction:column;align-items:stretch}.save-bar .el-button{margin-left:0}}
</style>