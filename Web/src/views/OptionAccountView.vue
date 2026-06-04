<template>
    <ElForm>
        <ElRow class="topFlex" :gutter="20">
            <ElCol>
                <ElFormItem label="博客主页配置">
                    <ElCol :span="2" style="text-align: center;">
                        <ElTooltip content="请上传对应博客头像">
                            <ElUpload :auto-upload="false" :limit="1" :show-file-list="false" action="#"
                                :on-change="handleAvatarChange">
                                <ElImage :src="avatarPreview" fit="cover"
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
                            <ElInput type="password" clearable placeholder="系统配置密钥" v-model="SecretKey"
                                aria-required="true" show-password>
                                <template #prepend>key</template>
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
                            <ElButton :icon="Upload" @click="saveProfile" type="success" :disabled="!isActive" />
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
import { Configuration, type AccountDto, type SubscriptionDto } from '@/api/generated';
import { AccountApi, FileApi } from '@/api/generated/apis';
import { ElMessage, ElRow, ElImage, ElCol, ElForm, ElFormItem, ElUpload, ElInput, ElButton, ElIcon, ElTooltip, type UploadFile } from 'element-plus'
import { reactive, onMounted, onUnmounted, type Ref, ref, computed } from 'vue';
import { ApiOption } from '@/services/storage/BaseOptions';

const avatar = ref<null | File>(null);
const avatarPreview = ref<string>('');
const isActive = computed(() => {
    const val = SecretKey.value;
    return val.length > 0 && !/\s/.test(val);
});

const form = reactive<AccountDto & { subscriptions: SubscriptionDto[] }>({
    name: '',
    desc: '',
    avatar: null,// GUID字符串，后端会根据这个字符串找到对应的文件并返回URL
    subscriptions: []
});

const ImageList = reactive<File[]>([])
const PreviewUrls = reactive<string[]>([]);
const SecretKey: Ref<string> = ref<string>('');

/**
 * 添加订阅项：在数据结构中添加新项，并为其预留文件和预览URL位置
 */
const addSubscription = () => {
    form.subscriptions.push({
        aliasName: '',
        subscriptionLink: '',
        subscriptionIcon: null,
        subscriptionPlatform: ''
    });
    ImageList.push(new File([], ''))
    PreviewUrls.push('')
};

/**
 * 处理文件变化：验证格式、存储文件、生成预览URL
 * @param uploadFile 
 * @param index 
 */
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

/**
 * 移除订阅项：删除数据、释放预览URL内存、清理文件记录
 * @param index 
 */
const removeSubscription = (index: number) => {
    form.subscriptions.splice(index, 1);

    // 释放内存并移除预览地址
    if (PreviewUrls[index]) {
        URL.revokeObjectURL(PreviewUrls[index]);
    }
    PreviewUrls.splice(index, 1);
    ImageList.splice(index, 1);
};


/**
 * 尝试转换头像文件为预览URL，如果没有文件则返回空字符串
 * @returns 头像预览URL或空字符串
 */
const handleAvatarChange = (uploadFile: UploadFile) => {
    const file = uploadFile.raw;
    if (!file) return;

    // 存储真正的二进制 File 对象
    avatar.value = file;

    // 清理旧的预览链接释放内存
    if (avatarPreview.value) {
        URL.revokeObjectURL(avatarPreview.value);
    }
    // 生成新的预览链接
    avatarPreview.value = URL.createObjectURL(file);
};

/**
 * 组件卸载时清理所有预览URL的内存，防止内存泄漏
 */
onUnmounted(() => {
    PreviewUrls.forEach(url => {
        if (url) URL.revokeObjectURL(url);
    });
    if (avatarPreview.value) {
        URL.revokeObjectURL(avatarPreview.value);
    }
});



onMounted(async () => {
    try {
        await AuthorInfo().getUser();
        const acc = AuthorInfo().userInfo;
        if (acc) {
            form.name = acc.name || '';
            form.desc = acc.desc || '';
            form.avatar = acc.avatar || null;
            form.subscriptions = acc.subscriptions || [];
            if (acc.avatar) {
                // 注意：如果后端返回的是 GUID，需要拼接上后端的图片查看 API 地址
                // 如果后端直接返回完整 URL，则直接赋值：
                avatarPreview.value = `${import.meta.env.VITE_API}/api/file/download?title=&id=${acc.avatar}`;
            }
        }
    } catch (e) { }
});

const saveProfile = async () => {
    try {
        // 1. 初始化请求 API
        let accountReq = new AccountApi(new Configuration(ApiOption));

        // 2. 注册基础信息
        await accountReq.apiAccountPost({ secretKey: SecretKey.value, accountDto: form });

        // 3. 登录获取 Token
        // 修复原 then(result, response) 的参数错位问题
        const loginRawResponse = await accountReq.apiAccountLoginPostRaw({
            loginDto: { name: form.name, password: SecretKey.value }
        });

        console.log('登录成功返回:', loginRawResponse);

        // 注意：根据你后端框架的不同，token 可能在 headers 里，也可能在 loginResponse.data 里
        // 如果是从 headers 获取，确保大小写与后端一致（有时是小写 'authorization'）
        const headers = loginRawResponse.raw.headers;
        const token = headers.get('Authorization') || headers.get('authorization');
        if (!token) {
            ElMessage.error('登录成功，但未获取到有效的 Authorization Token');
            return;
        }

        // 4. 保存 Token 并初始化文件 API
        ApiOption.headers['Authorization'] = 'Bearer '+token;
        const fileApi = new FileApi(new Configuration(ApiOption));

        // 5. 上传头像
        if (avatar.value) {
            try {
                const guid = await uploadFile(avatar.value as File, fileApi);
                form.avatar = guid;
                console.log('头像上传成功，GUID:', guid);
            } catch (err: any) {
                ElMessage.error('头像上传出错');
                return; // 如果头像必须上传成功，这里拦截；若非必需，可去掉 return
            }
        }

        // 6. 循环上传订阅图标（严格按顺序等待）
        for (let i = 0; i < ImageList.length; i++) {
            const f = ImageList[i];
            if (f && (f as File).size && form.subscriptions?.[i]) {
                try {
                    const uid = await uploadFile(f as File, fileApi);
                    form.subscriptions[i].subscriptionIcon = uid;
                    console.log(`第 ${i + 1} 张订阅图片上传成功，GUID:`, uid);
                } catch (err: any) {
                    ElMessage.error(`第 ${i + 1} 张订阅图片上传出错: ${err?.message || ''}`);
                    // 决定是否终止：如果要终止，写 return; 如果跳过继续，写 continue;
                }
            }
        }
        console.log(form)
        // 7. 更新 account 完成最终配置
        accountReq = new AccountApi(new Configuration(ApiOption));
        await accountReq.apiAccountPatch({ accountDto: form });

        ElMessage.success('配置已保存');

    } catch (err: any) {
        // 统一捕获上述链条中所有未被内部 try-catch 拦截的请求错误
        console.error('保存配置流程出错:', err);
        ElMessage.error(err?.message || '操作失败，请检查网络或输入');
    }
};

const uploadFile = async (file: File, fileApi: FileApi): Promise<string | undefined> => {
    const res = await fileApi.apiFileReceivePost({ file: [file] });
    if (res && res.length > 0) return res[0].uid;
    throw new Error('上传失败');
};

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