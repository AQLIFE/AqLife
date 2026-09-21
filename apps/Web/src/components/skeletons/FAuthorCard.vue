<template>
    <ElSkeleton class="author-skeleton" :animated="true" :throttle="300" :loading="status != 'success'">
        <template #template>
            <div class="profile">
                <ElSkeletonItem variant="image" class="avatar" />

                <div class="author-info">
                    <ElSkeletonItem variant="text" class="author-name" />

                    <ElSkeletonItem variant="text" class="author-desc" />
                </div>
            </div>

            <div class="subscriptions">
                <ElSkeletonItem v-for="index in 5" :key="index" variant="image" class="subscription-icon" />
            </div>
        </template>

        <template #default>
            <AuthorCard />
        </template>
    </ElSkeleton>
</template>

<script lang="ts" setup>
import {
    ElSkeleton,
    ElSkeletonItem,
} from 'element-plus'

import {
    storeToRefs,
} from 'pinia'


import {
    useAuthorInfoStore,
} from '@/stores/useAuthorInfoStore'

import AuthorCard from '@/components/AuthorCard.vue'

const authorInfoStore =
    useAuthorInfoStore()

const { status } =
    storeToRefs(authorInfoStore)

</script>

<style scoped>
.author-skeleton {
    width: 100%;
    height: 100%;

    min-width: 0;
    min-height: 0;

    display: flex;
    flex-direction: column;

    overflow: hidden;

    background-color: var(--topColor);
}


.profile {
    display: grid;

    grid-template-columns: 72px minmax(0, 1fr);

    gap: 16px;

    align-items: center;

    padding: 20px;
}


.avatar {
    width: 72px;
    height: 72px;
}


.author-info {
    min-width: 0;

    display: flex;
    flex-direction: column;

    gap: 10px;
}


.author-name {
    width: 50%;
    height: 20px;
}


.author-desc {
    width: 80%;
    height: 14px;
}


.subscriptions {
    display: flex;

    flex-wrap: wrap;

    gap: 12px;

    padding: 0 20px 20px;
}


.subscription-icon {
    width: 28px;
    height: 28px;
}
</style>