<template>
    <nav class="navigation-bar">
        <NavigationLink v-for="(item, index) in navList" :key="item.path" :route="item" :tabindex="index + 2" />
    </nav>
</template>

<script lang="ts" setup>
import NavigationLink from '@/components/NavigationLink.vue';
import { useNavigation } from '@/composables/useNavigation';

const { navList } = useNavigation();

</script>

<style scoped>
.navigation-bar {
  width: 100%;
  height: 60px;

  display: flex;
  align-items: center;
  justify-content: center;

  overflow: hidden;
}
</style>