<script setup lang="ts">
// import { useClipboard } from '@vueuse/core'
import { ElCol, ElRow,ElMessage } from 'element-plus';
import { type Ref, ref, onMounted } from 'vue';
import { marked } from 'marked';
import mermaid from 'mermaid';
import mdContentRaw from '@/assets/NET Core 开发要点.md?raw'; // Vite 支持 ?raw 导入文本
import { Blog } from '@/services/storer';
// import { CopyDocument,ElIcon } from '@element-plus/icons-vue';
import type { IAnchor } from '@/data/AnchorData';

const updateTime = '2024-06-20';
const description = '文章内容仅供参考，如有错误，欢迎指正。';


const { title, content } = extractTitleAndContent(mdContentRaw);

const blog = Blog();
blog.blogTitle = title;
blog.anchorList = extractHeadings(content);
const htmlContent: Ref<string | Promise<string>> | string | undefined = blog.isCache ? blog.blogCacheList.find(i => i.blogTitle == 'NET Core 开发要点')?.blogContent : ref(marked(content));

// 添加复制状态
const copied = ref(false)

// 复制函数
const copyCode = async (code: string) => {
    try {
        await navigator.clipboard.writeText(code)
        ElMessage({
            showClose: true,
            message: '代码已复制到剪贴板,若有建议请发信私聊,感谢支持!',
            type: 'success',
        })
        copied.value = true
        // 1.5秒后重置状态
        setTimeout(() => {
            copied.value = false
        }, 1500)
    } catch{
        ElMessage.error('复制失败，当前访问暂不支持')
        // console.error('复制失败:', '用户未授权')
    }
}

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
    const headingReg = /^(#{1,3})\s(.+)\n$/gm;
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

// 狗屎里有黄金,将代码块替换为 CodeRender 组件逻辑去渲染
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
        // console.log('处理代码块:', code);

        const lines = code.split('\n');
        const numberedHtml = lines.map((line, idx) =>
            `<div><span class="CodeLine">${idx + 1}</span><span class='txt'>${line}</span></div>`
        ).join('\n');

        const container = codeEl.parentElement;
        if (container) {
            container.addEventListener('click', (event) => {
                const target = event.target as HTMLElement;
                if (target.classList.contains('copyButton')) {
                    copyCode(code);
                }
            });
        }
        // 优雅的魔鬼 : 绝妙的Copy实现


        codeEl.innerHTML = `<div class="code" >
                <div class="codeHeader" >
                    <div class="codeType" > ${CodeType.value} </div>
                    <div class= "codeTitle" > </div>
                    <div class= "copyButton" title="${copied.value ? '已复制' : '点此复制以下代码'}"> ${copied.value ? 'Copy completed ✓' : 'Copy'} </div></div>
                    <div class= "codeContent" > ${numberedHtml} </div>
                </div>`
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

<style>
.row {
    display: flex;
    flex-direction: row;
    flex-wrap: nowrap;
    margin: 0;
    padding: 0;
}

.CodeLine {
    display: inline-block;
    width: 20px;
    padding: 0 2vh;
    border-right: 1px dashed var(--back_color_lv5);
    color: var(--back_color_lv5);
}

span.txt {
    padding: 0;
    margin: 0;
}

.code {
    background-color: var(--back_color_lv1);
    display: flex;
    flex-direction: column;
    flex-wrap: nowrap;
}

.codeHeader {
    display: flex !important;
    flex-direction: row;
    line-height: 50px;
    max-height: 50px;
}

.codeHeader>.codeType {
    display: flexbox;
    padding: 0 1vw;

}

.codeHeader>.codeTitle {
    display: flexbox;
    flex-grow: 1;
    text-align: center;
}

.codeHeader>.copyButton {
    background-color: var(--back_color_lv3);
    padding: 0 1vw;
}

.codeContent {
    background-color: var(--back_color_lv3);
    overflow-y: hidden;
    overflow-x: auto;
    flex-grow: 1;
    display: flex;
    flex-direction: column;
    justify-items: start;
}
</style>