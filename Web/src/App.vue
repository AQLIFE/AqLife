<script setup lang="ts">
// import { ElCol, ElContainer, ElHeader, ElMain, ElRow } from 'element-plus';
import { RouterView, useRoute } from 'vue-router';
import NavigationBar from '@/components/NavigationBar.vue';
import DevPlanCard from './components/DevPlanCard.vue';
import FAuthorCard from '@/Skeletons/FAuthorCard.vue';
import SearchBox from './components/SearchBox.vue';
// import BlogNodeTreeCard from './components/BlogNodeTreeCard.vue';
import { SidebarType } from './types/define';
import CalendarSelector from '@/components/CalendarSelector.vue';
const route = useRoute();
</script>

<template>
	<div id="baseLayout">
		<div id="tools">
			<div class="template">
				<SearchBox />
			</div>
			<div class="template">
				<CalendarSelector/>
			</div>
			<div class="template">3</div>
		</div>
		<div id="content">
			<div class="template"><NavigationBar /></div>
			<div class="template"><RouterView /></div>
		</div>
		<div id="account">
			<div class="template">
				<FAuthorCard />
			</div>
			<div class="template">
				<DevPlanCard v-if="route.meta.sidebarType === SidebarType.About" />
			</div>
		</div>
	</div>
</template>

<style scoped>
#baseLayout {
	background-color: var(--back_color_lv2);
	width: inherit;
	height: 100vh;
	display: grid;
	grid-template-columns: 20vw 1fr 20vw;
	overflow: hidden;
	-webkit-user-drag: none;
	overscroll-behavior:none;
}

/*---------------------tools------------------------ */
#tools {
	display: grid;
	grid-template-rows: 1fr 2fr 3fr;
	/* 设置 三行独立高度*/
}

#tools>.el-row {
	align-content: flex-start;
}

/*---------------------tools------------------------ */
/*---------------------content------------------------ */

#content {
	display: grid;
    grid-template-rows: auto 1fr; /* 第一行导航栏自适应，第二行占满剩余空间 */
    height: 100vh; /* 确保和父级等高 */
    overflow: hidden;
}

#content > .template:nth-child(2) {
    height: 100%;
    overflow: hidden; /* 让内部的 BlogView 自己处理滚动 */
}

/*---------------------content------------------------ */
/*---------------------account------------------------ */
#account {
	grid-template-rows: 1fr 2fr;
}

/*---------------------account------------------------ */
</style>
