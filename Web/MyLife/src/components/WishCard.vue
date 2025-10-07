<template>
    <ElCard shadow="hover" class="WishCard" :style="{backgroundImage: 'url(' + (props.wishImage)+')'}">
        <ElCol class="WishContent">{{ props.wishName }}</ElCol>
    </ElCard>
</template>
<script setup lang="ts">
const props = defineProps<{wishImage:string,wishName:string}>()

</script>

<style scoped>
.WishCard{
    position:sticky;
    height:30vh;
    background-repeat: no-repeat;
    background-size:cover;
}

.WishContent{
    width: 100%;
    position:absolute;
    bottom: 0;left: 0;
    background-color: rgba(255,255,255,0.5);
    text-align: center;
}
</style>