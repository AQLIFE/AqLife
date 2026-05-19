<template>
    <ElForm>
        <ElRow class="topFlex" :gutter="20">
            <ElCol>
                <ElFormItem label="博客主页配置">
                    <ElCol :span="2" style="text-align: center;">
                        <ElTooltip content="请上传对应博客头像">
                            <ElUpload :auto-upload="false" :limit="1" :show-file-list="false" action="#" :on-change="(file: UploadFile) => avatar = file">
                                <ElImage :src="TryConvert()" fit="cover"
                                    style="width: 30px; height: 30px; display: block; border-radius: 4px; border: 1px dashed #d9d9d9;">
                                    <template #error>
                                        <ElIcon style="position: relative;top:2px;">
                                            <Plus />
                                        </ElIcon>
                                    </template>
                                </ElImage>
                            </ElUpload>
                        </ElTooltip>
                    </ElCol>


                    <ElCol :span="3">
                        <ElTooltip content="博客账户名">
                            <ElInput clearable placeholder="博客账户名" v-model="form.name" />
                        </ElTooltip>
                    </ElCol>

                    <ElCol :span="9">
                        <ElTooltip content="自我概述">
                            <ElInput clearable placeholder="自我概述" v-model="form.desc" />
                        </ElTooltip>
                    </ElCol>

                    <ElCol :span="10">
                        <ElTooltip content="请填写对应系统密钥">
                            <ElInput type="url" clearable placeholder="系统配置密钥" v-model="SecretKey" aria-required="true">
                                <template #prepend>system key</template>
                            </ElInput>
                        </ElTooltip>
                    </ElCol>
                </ElFormItem>
            </ElCol>

            <ElCol v-for="(item, index) in form.subscriptions" :key="index">
                <ElFormItem :label="'社交主页关联配置' + index">
                    <ElCol :span="2" style="text-align: center;">
                        <ElTooltip content="请上传对应平台的logo">
                            <ElUpload :auto-upload="false" :limit="1" :show-file-list="false" accept=".svg"
                                :on-change="(file: any) => handleFileChange(file, index)" action="#">
                                <ElImage :src="PreviewUrls[index]" fit="cover"
                                    style="width: 30px; height: 30px; display: block; border-radius: 4px; border: 1px dashed #d9d9d9;">
                                    <template #error>
                                        <ElIcon style="position: relative;top:2px;">
                                            <Plus />
                                        </ElIcon>
                                    </template>
                                </ElImage>
                            </ElUpload>
                        </ElTooltip>
                    </ElCol>


                    <ElCol :span="3">
                        <ElTooltip content="请填写对应平台的社交账户名称">
                            <ElInput clearable placeholder="平台账户名" v-model="item.aliasName" />
                        </ElTooltip>
                    </ElCol>

                    <ElCol :span="3">
                        <ElTooltip content="请填写对应的平台名称">
                            <ElInput clearable placeholder="平台名" v-model="item.subscriptionPlatform" />
                        </ElTooltip>
                    </ElCol>

                    <ElCol :span="14">
                        <ElTooltip content="请填写对应平台的个人社交主页链接(仅允许https://前缀)">
                            <ElInput type="url" clearable placeholder="平台个人主页链接" v-model="item.subscriptionLink">
                                <template #prepend>Https://</template>
                            </ElInput>
                        </ElTooltip>
                    </ElCol>

                    <ElCol :span="2">
                        <ElTooltip content="点击此处即删除该项配置">
                            <ElButton @click="removeSubscription(index)" :icon="Delete" type="danger" />
                        </ElTooltip>
                    </ElCol>
                </ElFormItem>
            </ElCol>


            <ElCol>
                <ElFormItem>
                    <ElCol :span="22" style="padding: 0px;">
                        <ElTooltip content="点击添加其他社交平台配置">
                            <ElButton :icon="CirclePlus" class="Virtual" @click="addSubscription" />
                        </ElTooltip>
                    </ElCol>
                    <ElCol :span="2" style="padding:0px 30px;">
                        <ElTooltip content="点击保存配置到服务器">
                            <ElButton :icon="Upload" @click="uploadProfile()" type="success"/>
                        </ElTooltip>
                    </ElCol>
                </ElFormItem>
            </ElCol>
        </ElRow>
    </ElForm>
</template>

<script lang="ts" setup>
import { AuthorInfo } from '@/services/storage/AuthorInfo';
import { CirclePlus, Plus, Delete, Upload } from '@element-plus/icons-vue';
import { type AccountDto, type SubscriptionDto } from '@/api/generated';
import { ElMessage, ElRow, ElImage, ElCol, ElForm, ElFormItem, ElUpload, ElInput, ElButton, ElIcon, ElTooltip, type UploadFile } from 'element-plus'
import { reactive, onUnmounted, type Ref, ref } from 'vue';

const avatar = ref<null | UploadFile>(null);
const form = reactive<AccountDto & { subscriptions: SubscriptionDto[] }>({
    name: '',
    desc: '',
    avatar: '',
    // 默认可以给一个空数组，或者初始带一个空项
    subscriptions: []
});

const addSubscription = () => {
    form.subscriptions.push({
        aliasName: '',
        subscriptionLink: ''
    });
    ImageList.push(new File([], ''))
    PreviewUrls.push('')
};

const ImageList = reactive<File[]>([])
const PreviewUrls = reactive<string[]>([]);

const handleFileChange = (uploadFile: UploadFile, index: number) => {
    const file = uploadFile.raw;
    if (!file) return;
    const isSVG = file.type === 'image/svg+xml' || file.name.toLowerCase().endsWith('.svg');
    if (!isSVG) {
        ElMessage.error('仅支持上传 SVG 格式的图标！');

        // 关键：如果不符合要求，需要清理掉已产生的预览和记录 [cite: 198]
        if (PreviewUrls[index]) {
            URL.revokeObjectURL(PreviewUrls[index]);
            PreviewUrls[index] = '';
        }
        return;
    }

    // 存储文件用于后续上传
    ImageList[index] = file;

    // 生成预览 URL 并替换旧地址
    if (PreviewUrls[index]) {
        URL.revokeObjectURL(PreviewUrls[index]); // 释放旧内存
    }
    PreviewUrls[index] = URL.createObjectURL(file);
};

// 删除项：必须同步 splice 所有数组，DOM 才会正确刷新
const removeSubscription = (index: number) => {
    form.subscriptions.splice(index, 1);

    // 释放内存并移除预览地址
    if (PreviewUrls[index]) {
        URL.revokeObjectURL(PreviewUrls[index]);
    }
    PreviewUrls.splice(index, 1);
    ImageList.splice(index, 1);
};

const TryConvert = () => {
    if (avatar.value != null && avatar.value.raw != undefined)
        return URL.createObjectURL(avatar.value.raw);
    return '';
}

onUnmounted(() => {
    PreviewUrls.forEach(url => {
        if (url) URL.revokeObjectURL(url);
    });
});

const SecretKey: Ref<string> = ref<string>('');

const uploadProfile = async () => {
    console.log('正在执行', import.meta.env.VITE_API)

    if (form.name == null) { ElMessage.error('账户名不允许为空'); return; }
    // 必须登录
    // const [data, valid] = await handle(accountAPI.apiAccountPost({ secretKey: SecretKey.value, name: form.name, desc: form.desc! }))
    AuthorInfo()

    // if (valid == false || data == null) return;
    // 请求登录成功后, token会被写入Response Header
    // const loginSession = await accountAPI.apiAccountLoginPostRaw({ loginDto: { name: form.name!, password: SecretKey.value } })

    // const token = loginSession.raw.headers.get('Authorization')

    // if (token) {
    //     console.log(token)
    //     const ValidConfig = new Configuration({
    //         basePath: ApiOption.basePath,
    //         headers: {
    //             'Authorization': 'Bearer ' + token
    //         }
    //     })
    //     const fileAPI = new FileApi(ValidConfig)
    //     const list = []
    //     for (const e of ImageList) {
    //         if (e && e.size > 0) {
    //             const [data, status] = await handle(fileAPI.apiFileReceivePost({ file: e }));

    //             if (status) {
    //                 console.log("上传成功:", data);
    //                 list.push(data)
    //             } else {
    //                 console.error("上传失败!");
    //             }
    //         }
    //     }
    //     for (let i = 0; i < form.subscriptions.length; i++) {
    //         console.log(list[i], form.subscriptions[i].subscriptionIcon)
    //         form.subscriptions[i].subscriptionIcon = list[i]!.fileName;
    //     }
    //     accountAPI = new AccountApi(ValidConfig)// 增加token

    //     const count = await accountAPI.apiAccountSubscriptionPost({ subscriptionDto: form.subscriptions })
    //     ElMessage.success('配置成功')
    //     ElMessage.info(`上传结果:${count},${count == form.subscriptions.length}`)

    //     const user = AuthorInfo();
    //     user.userInfo = await accountAPI.apiAccountGet()
}
// };

</script>

<style lang="css" scoped>
.Virtual {
    border: 1px dashed green;
    background-color: rgba(0, 0, 0, 0);
    width: 100%;
}

.topFlex {
    align-content: flex-start;
}
</style>