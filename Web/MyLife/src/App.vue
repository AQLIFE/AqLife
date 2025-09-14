<script setup lang="ts">
import { ElCol, ElContainer, ElHeader, ElMain, ElRow } from 'element-plus';
import { RouterView } from 'vue-router';
import AuthorCard from '@/components/AuthorCard.vue'
// import SkillCard from './components/SkillCard.vue';
import NavigationBar from './components/NavigationBar.vue';
import DevPlan from './components/DevPlan.vue';
</script>

<template>
	<ElCol id="left" :span="8">
		<ElRow wrap="nowrap">
			<ElCol class="lcard">
				<AuthorCard />
			</ElCol>
			<!-- <ElCol class="lcard">
				<SkillCard />
			</ElCol> -->

			<ElCol class="lcard">
				<DevPlan />
			</ElCol>
		</ElRow>
	</ElCol>


	<ElCol id="right" :span="16">
		<ElContainer>
			<ElHeader id="navigation">
				<NavigationBar />
			</ElHeader>
			<ElMain id="viewer">
				<RouterView />
			</ElMain>
		</ElContainer>
	</ElCol>
</template>

<style scoped>
#left {
	background-color: var(--back_color_lv2);
}

#left>.el-row {
	align-content: flex-start;
}


.lcard {
	padding: 2vh 4vw;
}

#right {
	flex-grow: 2;
	background-color: var(--back_color_lv2);
}

#right>.el-container{height: 100vh;}

#navigation {
	margin-right: 5vw;
	padding: 0;
	height: 10vh;
	line-height: 10vh;
}

#viewer {
	height: auto;
	background-color: var(--back_color_lv1);
	margin-right: 5vw;
	padding: 0;
}
</style>
