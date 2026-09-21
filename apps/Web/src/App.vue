<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router'
import NavigationBar from '@/components/NavigationBar.vue'
import DevPlan from './components/DevPlanCard.vue'
import FAuthorCard from '@/components/skeletons/FAuthorCard.vue'
import SearchBox from '@/components/SearchBox.vue'
import { SidebarType } from '@/types/sidebarType'
import TOC from './components/TOC.vue'
import BlogOverview from '@/components/BlogOverview.vue'
import { computed, type Component } from 'vue'

const route = useRoute()
const sideManager = computed(():Component|null=>{
  if(route.meta.sidebarType == SidebarType.Preview)return TOC;
  if(route.meta.sidebarType == SidebarType.About)return DevPlan;
  return null
})

</script>

<template>
  <div id="baseLayout">
    <aside id="tools">
     <div class="tools-search">
      <SearchBox/>
     </div>
     <div class="tools-content">
      <component :is="sideManager"/>
     </div>
    </aside>
    <main id="content">
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
    20vw minmax(0, 1fr) 20vw;

  overflow: hidden;

  background-color: var(--back_color_lv1);

  -webkit-user-drag: none;
  overscroll-behavior: none;
}


/* =========================================================
   左侧
   ========================================================= */

#tools {
  min-width: 0;
  min-height: 0;

  display: grid;
  border-right: 1px solid #ddd;
  grid-template-rows:
    auto minmax(0, 1fr);

  overflow: hidden;
  background: #fafafa;
}


/* 搜索框 */

.tools-search {
  min-width: 0;

  overflow: hidden;
  border-bottom: 1px solid #ddd;
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

  background-color: var(--topColor);
}


/* =========================================================
   NavigationBar
   ========================================================= */

#navigation {
  min-width: 0;

  height: 60px;

  overflow: hidden;

  background-color: var(--topColor);
  border-bottom:1px solid #ddd;
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
  background: #fafafa;
  border-left: 1px solid #ddd;
}


/* 作者 */

.account-author {
  min-width: 0;
  min-height: 0;

  overflow: hidden;
  border-bottom:1px solid #ddd;
}


/* BlogOverview / DevPlanCard */

.account-content {
  min-width: 0;
  min-height: 0;

  overflow: hidden;
}
</style>
