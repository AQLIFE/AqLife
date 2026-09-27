<template>
  <div class="overview">
    <MgsPageHeader title="Overview" description="Your MGS workspace at a glance" />

    <section class="metrics">
      <article v-for="metric in metrics" :key="metric.label" class="metric">
        <span>{{ metric.label }}</span>
        <strong>{{ metric.value }}</strong>
        <small>{{ metric.note }}</small>
      </article>
    </section>

    <section class="content-grid">
      <article class="panel">
        <div class="panel-head">
          <div>
            <h2>Recent articles</h2>
            <p>Continue where you left off.</p>
          </div>
          <ElButton text @click="router.push('/blog/list')">View all</ElButton>
        </div>
        <div class="article-list">
          <div
          v-for="article in articles"
          :key="article.uid ?? article.title"
          class="article"
          @click="article.uid && router.push(`/blog/${article.uid}`)"
        >
            <div class="article-icon">MD</div>
            <div class="article-copy">
              <strong>{{ article.title }}</strong>
              <span>{{ article.meta }}</span>
            </div>
            <span class="status" :class="article.status">{{ article.label }}</span>
          </div>
        </div>
      </article>

      <article class="panel activity">
        <div class="panel-head">
          <div>
            <h2>Workspace</h2>
            <p>Quick actions</p>
          </div>
        </div>
        <button class="quick-action" @click="router.push('/blog/new')">
          <span>+</span>
          <div><strong>New article</strong><small>Start from a template</small></div>
        </button>
        <button class="quick-action" @click="router.push('/blog/list')">
          <span>↗</span>
          <div><strong>Manage content</strong><small>Browse and edit articles</small></div>
        </button>
      </article>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ElButton } from 'element-plus'
import { useRouter } from 'vue-router'
import { onBeforeMount, computed, ref } from 'vue'
import { OverviewApi } from '@/api'
import { apiConfiguration } from '@/services/api'
import MgsPageHeader from '@/components/ui/MgsPageHeader.vue'
import { publishStatus } from '@/types/TableFilterOption.ts'

const router = useRouter()
const overviewApi = new OverviewApi(apiConfiguration)
const overview = ref<Awaited<ReturnType<OverviewApi['apiOverviewGet']>> | null>(null)

const metrics = computed(() => [
  { label: 'Drafts', value: overview.value?.draftCount ?? 0, note: 'Current workspace' },
  { label: 'Scheduled', value: overview.value?.scheduledCount ?? 0, note: 'Waiting to publish' },
  { label: 'Published', value: overview.value?.publishedCount ?? 0, note: 'Published articles' },
])

const articles = computed(() =>
  (overview.value?.recentFiles ?? []).map(file => ({
    uid: file.uid,
    title: file.fileName || 'Untitled article',
    meta: file.fileType ? file.fileType.replace('.', '').toUpperCase() : 'File',
    status:
      file.publishStatus === publishStatus.Published
        ? 'published'
        : file.publishStatus === publishStatus.Scheduled
          ? 'scheduled'
          : 'draft',
    label:
      file.publishStatus === publishStatus.Published
        ? 'Published'
        : file.publishStatus === publishStatus.Scheduled
          ? 'Scheduled'
          : 'Draft',
  })),
)

onBeforeMount(async () => {
  overview.value = await overviewApi.apiOverviewGet({ recentCount: 5 })
})
</script>

<style scoped>
.overview { min-height:100%;padding-bottom:32px; }
.metrics { display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:12px;padding:0 28px 14px; }
.metric,.panel { background:var(--mgs-surface);border:1px solid var(--mgs-border);border-radius:var(--mgs-radius);box-shadow:var(--mgs-shadow); }
.metric { padding:18px 20px;display:flex;flex-direction:column;gap:5px; }
.metric span,.metric small { color:var(--mgs-muted);font-size:11px; }
.metric strong { font-size:25px;line-height:1.1; }
.content-grid { display:grid;grid-template-columns:minmax(0,2fr) minmax(260px,1fr);gap:12px;padding:0 28px; }
.panel { padding:20px; }
.panel-head { display:flex;justify-content:space-between;align-items:flex-start;gap:16px;margin-bottom:14px; }
.panel h2 { margin:0;font-size:14px; }
.panel-head p { margin:4px 0 0;color:var(--mgs-muted);font-size:11px; }
.article { display:flex;align-items:center;gap:12px;padding:12px 0;border-top:1px solid var(--mgs-border);cursor:pointer; }
.article:hover { background:var(--mgs-surface-soft); }
.article-icon { width:32px;height:32px;border-radius:7px;background:var(--mgs-surface-soft);display:grid;place-items:center;font-size:9px;font-weight:700;color:var(--mgs-secondary); }
.article-copy { min-width:0;flex:1;display:flex;flex-direction:column;gap:3px; }
.article-copy strong { font-size:12px;white-space:nowrap;overflow:hidden;text-overflow:ellipsis; }
.article-copy span { color:var(--mgs-muted);font-size:10px; }
.status { font-size:10px;padding:3px 7px;border-radius:99px; }
.status.draft { background:#fff5df;color:#a56d13; }
.status.published { background:#eaf8f0;color:#2f8a59; }
.status.scheduled { background:#eef3ff;color:#3f6fe5; }
.quick-action { width:100%;display:flex;align-items:center;gap:12px;border:1px solid var(--mgs-border);background:var(--mgs-surface);border-radius:8px;padding:12px;text-align:left;cursor:pointer;margin-top:8px; }
.quick-action:hover { background:var(--mgs-surface-soft); }
.quick-action > span { width:28px;height:28px;display:grid;place-items:center;border-radius:7px;background:var(--mgs-accent-soft);color:var(--mgs-accent); }
.quick-action div { display:flex;flex-direction:column;gap:2px; }
.quick-action strong { font-size:12px; }
.quick-action small { color:var(--mgs-muted);font-size:10px; }
@media (max-width: 900px) { .metrics,.content-grid { grid-template-columns:1fr; } }
</style>
