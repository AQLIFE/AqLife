<template>
  <ElCol>
    subscriptList
    <ElRow>
      <ElCol :span="3" v-for="(item, index) in subscriptDtos" :key="index" class="gap">
        <ScriptionCard :serial="index" @delete="remove" :item="item"/>
      </ElCol>

      <ElCol :span="3">
        <SubscriptionAddCard @plusClick="add" @uploadClick="update" :data-list="subscriptDtos" />
      </ElCol>
    </ElRow>
  </ElCol>
</template>

<script setup lang="ts">
import { Link, Plus, Position, QuestionFilled } from '@element-plus/icons-vue'
import { ElRow, ElCol, ElMessage } from 'element-plus'
import { useAccountStore } from '@/stores/useAccountStore'
import { computed, ref, watch, type Reactive } from 'vue'
import type { SubscriptionDto } from '@/api'
import SubscriptionAddCard from '@/components/SubscriptionAddCard.vue'
import ScriptionCard from '@/components/ScriptionViewCard.vue'

const accountStore = useAccountStore()
// 使用 computed，只要 Store 里的数据一变，这里会自动更新，并且依然保持响应式
const subscriptDtos = computed(() => accountStore.systemAccount?.subscriptions)

function remove(index: number): void {
  const len = accountStore.systemAccount?.subscriptions?.length
  if (len && index+1<=len) accountStore.systemAccount?.subscriptions?.splice(index, index+ 1)
  ElMessage.info(`删除[${index},${index+1}]`)
}

function add() {
  accountStore.systemAccount?.subscriptions?.push({
    subscriptionLink: '',
    subscriptionPlatform: '',
    aliasName: '',
    subscriptionIcon: '',
  })
}
function update(){

}
// onBeforeMount(() => {subscriptDtos = useAccountStore().systemAccount?.subscriptions})
</script>

<style lang="css" scoped>
.link {
  border: 3px solid #dcdfe6;
  border-radius: 12px;
  overflow: hidden;
}

.link .icon {
  position: relative;
  top: 3px;
  border-radius: 25%;
}
.link .content {
  background-color: #dcdfe6;
  align-self: center;
}

.gap {
  margin-right: 20px;
}
</style>
