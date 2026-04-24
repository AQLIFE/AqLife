<script setup lang="ts">
import CodeRender from '@/components/CodeRender.vue';
import UpLoadFileCard from '@/components/UpLoadFileCard.vue';

</script>

<template>
    <ElCol>Prebuilt</ElCol>
    <!-- <CodeRender CodeType="csharp">
        ```csharp
        var x = obj switch{
        string => true,
        object => false,
        double => true
        };
        ```
    </CodeRender> -->
    <UpLoadFileCard />
</template>