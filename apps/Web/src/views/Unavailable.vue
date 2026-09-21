<template>
    <main class="account-unavailable">

        <section class="message">

            <h1>
                无法加载账户信息
            </h1>

            <p v-if="reason === 'timeout'">
                请求超时，请检查网络连接后重试。
            </p>

            <p v-else-if="reason === 'empty'">
                未获取到账户信息。
            </p>

            <p v-else>
                获取账户信息时发生错误。
            </p>

            <button @click="retry">
                重试
            </button>

        </section>

    </main>
</template>