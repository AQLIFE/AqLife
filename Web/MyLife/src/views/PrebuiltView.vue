<script setup lang="ts">
import CodeRender from '@/components/CodeRender.vue';

</script>

<template>
    <ElCol>Prebuilt</ElCol>
    <CodeRender title="测试代码">
        ```csharp
        var x = obj switch{
        string => true,
        object => false,
        double => true
        };
        ```
    </CodeRender>
</template>