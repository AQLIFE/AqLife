<template>
    <div class="account-unavailable">

        <section class="message">

            <h1>
                无法加载账户信息
            </h1>

            <!-- <p v-if="reason === 'timeout'">
                请求超时，请检查网络连接后重试。
            </p> -->

            <p v-if="author.status === 'empty'">
                未获取到账户信息。
            </p>

            <p v-else>
                获取账户信息时发生错误。
            </p>

            <ElButton @click="retry">
                重试
            </ElButton>

        </section>

    </div>
</template>


<script setup lang="ts">
import { useAuthorInfoStore } from '@/stores/accountStore';
const author = useAuthorInfoStore()
async function retry(): Promise<void> { await author.getUser() }

</script>

<style lang="css" scoped>
.account-unavailable {
    display: flex;
    justify-content: center;
    align-self: center;
}
</style>