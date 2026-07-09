<template>
  <ElCol>
    subscriptList
    <ElRow style="align-items: stretch;">
      <ElCol :span="3" v-for="(item, index) in subscriptDtos" :key="index" class="gap">
        <ScriptionCard :serial="index" @delete="remove" :item="item" v-model:file="files[index]" />
      </ElCol>

      <ElCol :span="3">
        <SubscriptionAddCard @plusClick="add" @uploadClick="update" :data-list="subscriptDtos" />
      </ElCol>
    </ElRow>
  </ElCol>
</template>

<script setup lang="ts">
import { ElRow, ElCol, ElMessage } from 'element-plus'
import { useAccountStore } from '@/stores/useAccountStore'
import { computed, onBeforeMount, ref, watch, type Ref } from 'vue'
import { AccountApi, FileApi, type SubscriptionDto } from '@/api'
import SubscriptionAddCard from '@/components/SubscriptionAddCard.vue'
import ScriptionCard from '@/components/ScriptionViewCard.vue'
import { apiConfiguration } from '@/services/api'

const accountStore = useAccountStore()
// 使用 computed，只要 Store 里的数据一变，这里会自动更新，并且依然保持响应式
const subscriptDtos = computed(() => accountStore.systemAccount?.subscriptions)

// onBeforeMount(()=>console.log(useAccountStore().systemAccount?.subscriptions))
const files = ref<(File | null)[]>([]) // 初始化为空数组

function remove(index: number): void {
  const len = accountStore.systemAccount?.subscriptions?.length
  if (len && index + 1 <= len) {
    accountStore.systemAccount?.subscriptions?.splice(index, index + 1)
    files.value.splice(index, 1)
  }
  ElMessage.info(`删除[${index + 1}]订阅`)
}

function add() {
  accountStore.systemAccount?.subscriptions?.push({
    subscriptionLink: '',
    subscriptionPlatform: '',
    aliasName: '',
    subscriptionIcon: '',
  })
  files.value.push(null)
}
async function update() {
  if (!valid(files.value, subscriptDtos.value!)) return

  const fileApi = new FileApi(apiConfiguration)
  const accountApi = new AccountApi(apiConfiguration)

  try {
    // 2. 并行上传所有文件，提升吞吐量 [cite: 9]
    const uploadTasks = files.value.map(async (item, index) => {
      if (!item) return

      const iconGuids = await fileApi.apiFileUploadPost({ file: [item] }) // 包装为数组)

      // 3. 回填 GUID 到对应的 DTO 契约对象 [cite: 210]
      if (iconGuids && iconGuids.length > 0) {
        subscriptDtos.value![index].subscriptionIcon = iconGuids[0]
      }
    })

    await Promise.all(uploadTasks)
    ElMessage.success('所有图标上传并关联成功')
    if (subscriptDtos.value != undefined) {
      const guid = await accountApi.apiAccountSubscriptionsPut({ subscriptionDto: subscriptDtos.value })
      console.log(guid)
    }
  } catch (error) {
    ElMessage.error('上传过程中发生异常')
    console.error(error)
  }
}

function valid(files: Array<File | null>, subscriptionDtos: SubscriptionDto[]): boolean {
  if (!files || !subscriptionDtos) return false

  // 校验长度一致性
  if (files.length !== subscriptionDtos.length) {
    ElMessage.warning('你有尚未完成的修改')
    return false
  }

  // 校验每一项是否都选择了图像 (对齐全栈校验契约 [cite: 37, 43])
  for (let i = 0; i < files.length; i++) {
    if (files[i] === null) {
      ElMessage.warning(`第 ${i + 1} 个订阅缺少图像标识`)
      return false
    }
  }

  return true
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
