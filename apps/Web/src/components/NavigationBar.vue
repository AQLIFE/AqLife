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

  overflow-x: auto;
  overflow-y: hidden;
  scrollbar-width: none;
}

.navigation-bar::-webkit-scrollbar {
  display: none;
}

.navigation-bar :deep(*) {
  flex-shrink: 0;
}

@media (max-width: 900px) {
  .navigation-bar {
    justify-content: flex-center;
    padding: 0 10px;
    gap: 4px;
  }
}
</style>