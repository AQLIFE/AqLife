<template>
    <ElRow class="layout" :gutter="10">
        <ElCol v-for="item, index in 17" :key="index" :span="8">
            <ElCard shadow="hover">
                <template #header>
                    <div class="header">
                        <div>{{ item }}</div>
                        <ElTag type="info">BlogType</ElTag>
                    </div>
                </template>
                <ElTree :data="demoTreeData" :props="defaultProps" empty-text="加载中,请稍后" />
                <template #footer>
                    <div>Published on {{UploadTime}}</div>
                </template>
            </ElCard>
        </ElCol>
    </ElRow>
</template>

<script setup lang="ts">
import { ElRow, ElCol, ElCard, ElTree } from 'element-plus';

interface TreeData {
    id: number;
    label: string;
    // 使用数组包裹自身，并设为可选属性（因为叶子节点可能没有 children）
    children?: TreeData[];
}

const demoTreeData: TreeData[] = [
    {
        id: 1,
        label: 'Level one 1',
        children: [
            {
                id: 4,
                label: 'Level two 1-1',
                children: [
                    {
                        id: 9,
                        label: 'Level three 1-1-1',
                    },
                    {
                        id: 10,
                        label: 'Level three 1-1-2',
                    },
                ],
            },
        ],
    },
    {
        id: 2,
        label: 'Level one 2',
        children: [
            {
                id: 5,
                label: 'Level two 1-1',
                children: [
                    {
                        id: 11,
                        label: 'Level three 1-1-1',
                    },
                    {
                        id: 12,
                        label: 'Level three 1-1-2',
                    },
                ],
            },
        ],
    }
]

const defaultProps = {
    children: 'children', // 告诉组件：子节点在 subFiles 字段里
    label: 'label',    // 告诉组件：标题在 fileName 字段里
};

const UploadTime = new Date().toDateString();
</script>


<style lang="css" scoped>
.layout {
    height: 100%;
    /* 自动填满 #content 分配给它的 1fr 空间 */
    overflow-y: auto;
    /* 开启内部滚动 */
    align-content: start;
    /*避免头部遮挡*/
    justify-content: flex-start;
}

.layout::-webkit-scrollbar {
    display: none;
}



.el-card {
    margin-bottom: 10px;
}

.el-card .header {
    display: grid;
    grid-template-columns: 3fr 1fr;
    text-align: start;
}

.el-card .el-tree {
    padding-left: 10px;
}

.el-card__footer > div{
    font-size: 12px;
    text-align: right;
    color: var(--back_color_lv2);
}
</style>
