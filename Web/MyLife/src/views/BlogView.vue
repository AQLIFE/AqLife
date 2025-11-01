<script setup lang="ts">
import { ElCol, ElRow } from 'element-plus';
import { type Ref, ref, onMounted,markRaw, type Component } from 'vue';
import { marked } from 'marked';
import mermaid from 'mermaid';
import mdContentRaw from '@/assets/NET Core 开发要点.md?raw'; // Vite 支持 ?raw 导入文本
import { Blog } from '@/services/storer';
import type { IAnchor } from '@/data/AnchorData';
import CodeRender from '@/components/CodeRender.vue';

const updateTime = '2024-06-20';
const description = '文章内容仅供参考，如有错误，欢迎指正。';

const { title, content } = extractTitleAndContent(mdContentRaw);

const blog = Blog();
blog.blogTitle = title;
blog.anchorList = extractHeadings(content);
const htmlContent: Ref<string | Promise<string>> | string | undefined = blog.isCache ? blog.blogCacheList.find(i => i.blogTitle == 'NET Core 开发要点')?.blogContent : ref(marked(content));


onMounted(async () => {
    mermaid.initialize({ startOnLoad: false });
    await mermaid.run({
        querySelector: '.language-mermaid',
    });

    if (!blog.isCache) {
        // 首次加载,添加锚点和行号,并缓存
        addIdsToHeadings('blogContent', blog.anchorList);
        addCodeBlocksRender();
        blog.blogCacheList.push({ blogTitle: blog.blogTitle, blogContent: CacheBlog() as string });
    }

});


// 收集标题信息,并将其移除渲染内容
function extractTitleAndContent(md: string) {
    const lines = md.split(/\r?\n/);
    let title = '';
    let startIdx = 0;
    for (let i = 0; i < lines.length; i++) {
        if (lines[i].startsWith('# ')) {
            title = lines[i].replace(/^# /, '').trim();
            startIdx = i + 1;
            break;
        }
    }
    return {
        title,
        content: lines.slice(startIdx).join('\n')
    };
}

// 解析md 文件,并提取所有标题,返回锚点列表
function extractHeadings(md: string): IAnchor[] {
    const headingReg = /^(#{1,6})\s(.+)\n$/gm;
    const headings: IAnchor[] = [];
    let match;
    let idx = 0;
    while ((match = headingReg.exec(md)) !== null) {
        const level = match[1].length;
        const title = match[2].trim();
        const id = `heading-${idx}-${encodeURIComponent(title.replace(/\\s+/g, '-'))}`;
        headings.push({ title, id, level });
        idx++;
    }
    return headings;
}

// 为所有标题添加 id 属性,以便锚点链接跳转
function addIdsToHeadings(containerId: string, headings: IAnchor[]): void {
    const container = document.getElementById(containerId);
    if (!container) return;
    let idx = 0;
    container.querySelectorAll('h1, h2, h3, h4, h5, h6').forEach(el => {
        el.id = headings[idx]?.id || `heading-${idx}`;
        idx++;
    });
}

// 123
function addCodeBlocksRender() {
    // console.log(htmlContent)
    
    const codeBlock = document.querySelectorAll('pre code')

    codeBlock.forEach(codeEl => {
        const CodeType: Ref<string> = ref('');
        if (codeEl.classList.contains('language-mermaid')) return;

        codeEl.classList.forEach(element => {
            CodeType.value = element.match(/language-([\w\S]+)/)?.[1] || 'txt';
        });

        const code = codeEl.textContent || '';
        const lines = code.split('\n');
        const numberedHtml = lines.map((line, idx) =>
            `<span class="code-line"><span style="color:var(--back_color_lv5);">${idx + 1}</span> ${line}</span>`
        ).join('\n');
        const component:Component = markRaw(CodeRender)
        
        
        codeEl.innerHTML = `<${component} CodeType=${CodeType.value}>${numberedHtml}</${component}>`;
        
    });
}


function CacheBlog(): HTMLElement | string | null {
    return document.getElementById('blogContent')?.innerHTML || '';
}
</script>

<template>

    <ElCol class="blog">
        <ElCol class="blogTitle">{{ blog.blogTitle }}</ElCol>
        <ElCol class="description">
            <ElRow>
                <ElCol>{{ description }}</ElCol>
                <ElCol>Update:{{ updateTime }}</ElCol>
            </ElRow>
        </ElCol>
        <div id="blogContent" v-html="htmlContent" />
    </ElCol>
</template>



<style scoped>
.blogTitle {
    font-size: 2rem;
    font-weight: bold;
    margin: 2vh 0;
    text-align: center;
}

.description {
    font-size: 1rem;
    text-align: center;
}

.description>.el-row {
    justify-content: center;
    height: auto;
    font-size: small;
    color: var(--back_color_lv4);
    background-color: var(--back_color_lv2);
}

#blogContent {
    margin: 4vh 8vw;
    line-height: 2rem;
    font-size: 1.2rem;
    height: 100vh !important;
}
</style>