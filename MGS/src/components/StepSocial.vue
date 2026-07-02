<template>
  <ElForm>
    <ElFormItem :label="'社交主页关联配置' + index" v-for="(item, index) in source" :key="index">
      <ElTooltip content="请上传对应平台的logo">
        <ElUpload :auto-upload="false" :limit="1" :show-file-list="false" accept=".svg" :on-change="(file: any) => handleFileChange(file, index)" action="#">
          <ElImage :src="PreviewUrls[index]" fit="cover" style="   width: 30px;   height: 30px;   display: block;   border-radius: 4px;   border: 1px dashed #d9d9d9;">
            <template #error>
              <ElIcon style="position: relative; top: 2px">
                <Plus />
              </ElIcon>
            </template>
          </ElImage>
        </ElUpload>
      </ElTooltip>

      <ElTooltip content="请填写对应平台的社交账户名称">
        <ElInput clearable placeholder="平台账户名" v-model="item.aliasName" />
      </ElTooltip>

      <ElTooltip content="请填写对应的平台名称">
        <ElInput clearable placeholder="平台名" v-model="item.subscriptionPlatform" />
      </ElTooltip>

      <ElTooltip content="请填写对应平台的个人社交主页链接(仅允许https://前缀)">
        <ElInput
          type="url"
          clearable
          placeholder="平台个人主页链接"
          v-model="item.subscriptionLink"
        >
          <template #prepend>Https://</template>
        </ElInput>
      </ElTooltip>

      <ElTooltip content="点击此处即删除该项配置">
        <ElButton @click="removeSubscription(index)" :icon="Delete" type="danger" />
      </ElTooltip>
    </ElFormItem>

    <ElFormItem>
      <ElTooltip content="点击返回上一页">
        <ElButton :icon="CirclePlus" class="Virtual" @click="addSubscription" />
      </ElTooltip>
      <ElTooltip content="点击添加其他社交平台配置">
        <ElButton :icon="CirclePlus" class="Virtual" @click="addSubscription" />
      </ElTooltip>

      <ElTooltip content="点击保存配置到服务器">
        <ElButton :icon="Upload" @click="saveProfile" type="success" :disabled="!isActive" />
      </ElTooltip>
    </ElFormItem>
  </ElForm>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { ElCol, ElForm, ElFormItem, ElButton, ElInput, ElImage, ElTooltip, ElIcon, ElUpload,} from 'element-plus'
const PreviewUrls =ref([])
</script>
