<script setup lang="ts">
import {  ElCol, ElContainer, ElHeader, ElRow } from 'element-plus';
import { RouterView } from 'vue-router';
</script>

<template>
	<ElContainer id="Mylife">
		<ElCol id="left" :span="6"></ElCol>
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
	;
}

#left {
	background-color: var(--back_color_lv1);
}

#right{
	background-color: var(--back_color_lv2);
}
</style>
