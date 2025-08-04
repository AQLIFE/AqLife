<script setup lang="ts">
import { ElCol, ElContainer, ElHeader, ElRow } from 'element-plus';
import { RouterView } from 'vue-router';
import AuthorCard from '@/components/AuthorCard.vue'
import SkillCard from './components/SkillCard.vue';
</script>

<template>
	<ElContainer id="Mylife">
		<ElCol id="left" :span="6">
			<ElRow wrap="nowrap">
				<ElCol class="lcard">
					<AuthorCard />
				</ElCol>
				<ElCol class="lcard">
					<SkillCard />
				</ElCol>
			</ElRow>

		</ElCol>
		<ElCol :span="18" id="right">
			<ElHeader height="120">Header</ElHeader>
			<ElCol>
				<RouterView />
			</ElCol>
		</ElCol>
	</ElContainer>
</template>

<style scoped>
#Mylife {
	width: 100vw;
	height: 100vh;
}

#left {
	background-color: var(--back_color_lv2);
}

#left>.el-row {
	height: inherit;
}

.lcard {
	padding:1vw  4vw ;
}

#right {
	background-color: var(--back_color_lv2);
}
</style>
