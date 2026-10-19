<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import NavigationBar from '@/components/NavigationBar.vue'
import DevPlan from './components/DevPlan.vue'
import FAuthorCard from '@/components/skeletons/FAuthorCard.vue'
import SearchBox from '@/components/SearchBox.vue'
import { SidebarType } from '@/types/sidebarType'
import BlogOverview from '@/components/BlogOverview.vue'
import { computed, ref } from 'vue'
import { ElButton, ElDrawer } from 'element-plus'
import { Menu, Search, User } from '@element-plus/icons-vue'
import TocTree from './components/TocTree.vue'
import { useArticleStore } from './stores/articleStore.ts'

const articleStore = useArticleStore()
const route = useRoute()
const mobileDrawerOpen = ref(false)
const mobilePanel = ref<'search' | 'sidebar' | 'account'>('search')

function openMobilePanel(panel: 'search' | 'sidebar' | 'account') {
  mobilePanel.value = panel
  mobileDrawerOpen.value = true
}
const sideManager = computed(() => {
    switch (route.meta.sidebarType) {
        case SidebarType.Preview:
            return {
                component: TocTree,
                props: {
                    items: articleStore.toc,
                    title: articleStore.blogTitle,
                },
            }

        case SidebarType.About:
            return {
                component: DevPlan,
                props: {},
            }

        default:
            return null
    }
})

</script>

<template>
  <div id="baseLayout">
    <aside id="tools">
     <div class="tools-search">
      <SearchBox/>
     </div>
     <div class="tools-content">
      <component :is="sideManager?.component" v-bind="sideManager?.props"/>
     </div>
    </aside>
    <main id="content">
      <div class="mobile-toolbar" aria-label="移动端工具栏">
        <ElButton :icon="Search" text @click="openMobilePanel('search')">
          搜索
        </ElButton>
        <ElButton v-if="sideManager" :icon="Menu" text @click="openMobilePanel('sidebar')">
          {{ route.meta.sidebarType === SidebarType.Preview ? '目录' : '工具' }}
        </ElButton>
        <ElButton :icon="User" text @click="openMobilePanel('account')">
          个人
        </ElButton>
      </div>
      <div id="navigation">
        <NavigationBar/>
      </div>
      <div id="mainViewport">
        <RouterView/>
      </div>
    </main>
    <aside id="account">
      <div class="account-author">
        <FAuthorCard/>
      </div>
      <div class="account-content">
        <BlogOverview/>
      </div>
    </aside>

    <ElDrawer
      v-model="mobileDrawerOpen"
      class="mobile-drawer"
      :title="mobilePanel === 'search' ? '搜索' : mobilePanel === 'sidebar' ? (route.meta.sidebarType === SidebarType.Preview ? '文章目录' : '工具') : '个人与博文'"
      :size="mobilePanel === 'account' ? 'min(92vw, 420px)' : 'min(90vw, 360px)'"
      :append-to-body="true"
      destroy-on-close
    >
      <div v-if="mobilePanel === 'search'" class="mobile-drawer-search">
        <SearchBox />
      </div>
      <div v-else-if="mobilePanel === 'sidebar'" class="mobile-drawer-sidebar">
        <component :is="sideManager?.component" v-bind="sideManager?.props" />
      </div>
      <div v-else class="mobile-drawer-account">
        <FAuthorCard />
        <BlogOverview />
      </div>
    </ElDrawer>
  </div>
</template>

<style scoped>
#baseLayout {
  width: 100%;
  height: 100%;

  min-width: 0;
  min-height: 0;

  display: grid;

  grid-template-columns:
    300px minmax(0, 1fr) 300px;

  overflow: hidden;

  background-color: var(--back_color_lv1);

  -webkit-user-drag: none;
  overscroll-behavior: none;
}


.mobile-toolbar {
  display: none;
}

/* =========================================================
   左侧
   ========================================================= */

#tools {
  min-width: 0;
  min-height: 0;

  display: grid;
  border-right: 1px solid var(--el-border-color-lighter);
  grid-template-rows:
    auto minmax(0, 1fr);

  overflow: hidden;
  /* background: #fafafa; */
}


/* 搜索框 */

.tools-search {
  min-width: 0;

  overflow: hidden;
  /* border-bottom: 1px solid #ddd; */
}


/* TOC / ADemo */

.tools-content {
  min-width: 0;
  min-height: 0;

  overflow: hidden;
}


/* =========================================================
   中间
   ========================================================= */

#content {
  min-width: 0;
  min-height: 0;

  display: grid;

  grid-template-rows:
    60px minmax(0, 1fr);
    

  overflow: hidden;

  /* background-color: var(--topColor); */
}


/* =========================================================
   NavigationBar
   ========================================================= */

#navigation {
  min-width: 0;

  height: 60px;

  overflow: hidden;

  /* background-color: var(--topColor); */
  border-bottom:1px solid var(--el-border-color-lighter);
}


/* =========================================================
   RouterView 容器
   ========================================================= */

#mainViewport {
  min-width: 0;
  min-height: 0;

  overflow: hidden;
}


/* =========================================================
   右侧
   ========================================================= */

#account {
  min-width: 0;
  min-height: 0;

  display: grid;

  grid-template-rows:
    auto 1fr;

  overflow: hidden;
  /* background: #fafafa; */
  border-left: 1px solid var(--el-border-color-lighter);
}


/* 作者 */

.account-author {
  min-width: 0;
  min-height: 0;

  overflow: hidden;
  /* border-bottom:1px solid #ddd; */
}


/* BlogOverview / DevPlanCard */

.account-content {
  min-width: 0;
  min-height: 0;

  overflow: hidden;
}

.mobile-drawer-search,
.mobile-drawer-sidebar,
.mobile-drawer-account {
  width: 100%;
  height: 100%;
  min-width: 0;
  min-height: 0;
  overflow: auto;
}

.mobile-drawer-search {
  display: flex;
  flex-direction: column;
}

.mobile-drawer-search :deep(.search-box) {
  min-height: 100%;
  overflow: visible;
}

.mobile-drawer-sidebar {
  padding-bottom: 16px;
}

.mobile-drawer-account {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.mobile-drawer-account :deep(.author-skeleton) {
  height: auto;
  flex: 0 0 auto;
}

@media (max-width: 1100px) {
  #baseLayout {
    grid-template-columns: 240px minmax(0, 1fr) 240px;
  }
}

@media (max-width: 900px) {
  #baseLayout {
    height: 100dvh;
    min-height: 100dvh;
    padding-top: env(safe-area-inset-top, 0px);
    padding-bottom: env(safe-area-inset-bottom, 0px);
    grid-template-columns: minmax(0, 1fr);
    grid-template-rows: minmax(0, 1fr);
  }

  #tools,
  #account {
    display: none;
  }

  #content {
    grid-template-rows: 44px 52px minmax(0, 1fr);
  }

  .mobile-toolbar {
    display: flex;
    align-items: center;
    justify-content: space-around;
    gap: 8px;
    padding: 0 10px;
    border-bottom: 1px solid var(--el-border-color-lighter);
    background: var(--el-bg-color);
  }

  .mobile-toolbar :deep(.el-button) {
    min-height: 36px;
    margin: 0;
    padding: 8px 12px;
  }

  #navigation {
    height: 52px;
  }
}

@media (max-width: 480px) {
  #content {
    grid-template-rows: 44px 48px minmax(0, 1fr);
  }

  #navigation {
    height: 48px;
  }
}
</style>
