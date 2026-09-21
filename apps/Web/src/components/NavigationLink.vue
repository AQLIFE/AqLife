<template>
    <RouterLink class="navigation-link" :to="props.route.path">
        <!-- <ElIcon size="16px"><component :is="props.route.icon"/></ElIcon> -->
        {{ props.route.title }}
    </RouterLink>
</template>

<script setup lang="ts">
import { type PropType } from 'vue';
import {type NavItemDto} from '@/types/navigation'

const props = defineProps({
    route: {type:Object as PropType<NavItemDto>,required:true}
});
</script>

<style lang="css" scoped>
.navigation-link {
  height: 60px;
  padding: 0 20px;
  margin: 0;
  font-size: 1rem;
  color:#303133;

  display: inline-flex;
  align-items: center;
  gap: 8px;
  text-decoration-line: none;
}
</style>