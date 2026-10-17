<template>
  <main class="entrance-page">
    <section class="entrance-card">
      <header class="entrance-brand">
        <div class="brand-mark" aria-hidden="true">M</div>
        <div>
          <p class="eyebrow">MONIREPO · MGS</p>
          <p class="brand-name">MoniRepo</p>
        </div>
      </header>

      <div class="entrance-hero">
        <div class="hero-orbit" aria-hidden="true">
          <div class="orbit-ring ring-one"></div>
          <div class="orbit-ring ring-two"></div>
          <div class="hero-symbol"><span>M</span></div>
          <span class="orbit-dot dot-one"></span>
          <span class="orbit-dot dot-two"></span>
          <span class="orbit-dot dot-three"></span>
        </div>
        <p class="eyebrow hero-eyebrow">YOUR CONTENT WORKSPACE</p>
        <h1>把想法，变成值得分享的内容。</h1>
        <p class="hero-description">在一个清晰、专注的工作区里管理博文、整理素材，并轻松完成发布。</p>
      </div>

      <div class="entrance-actions">
        <ElButton class="primary-action" type="primary" size="large" @click="router.push('/login')">
          登录工作区 <ElIcon><ArrowRight /></ElIcon>
        </ElButton>
        <ElButton class="secondary-action" size="large" @click="router.push('/register')">
          初始化系统 / 创建账户
        </ElButton>
      </div>

      <div class="entrance-divider"><span>从这里开始</span></div>
      <footer class="entrance-footer">
        <span class="status-dot"></span>
        <span>你的内容工作区，已准备就绪</span>
      </footer>
    </section>
    <p class="page-footnote">专注内容创作 · 让发布流程更简单</p>
  </main>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { ElButton, ElIcon } from 'element-plus'
import { ArrowRight } from '@element-plus/icons-vue'

const router = useRouter()
</script>

<style scoped>
.entrance-page {
  width: 100%;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 18px;
  padding: 36px 20px 24px;
  background:
    radial-gradient(ellipse at 12% 8%, rgba(63, 111, 229, .09), transparent 34%),
    radial-gradient(ellipse at 90% 88%, rgba(63, 111, 229, .06), transparent 32%),
    var(--mgs-bg);
}
.entrance-card {
  width: min(100%, 560px);
  padding: 30px 38px 26px;
  border: 1px solid var(--mgs-border);
  border-radius: 16px;
  background: var(--mgs-surface);
  box-shadow: 0 20px 60px rgba(23, 25, 28, .08);
}
.entrance-brand { display: flex; align-items: center; gap: 12px; }
.brand-mark {
  width: 42px; height: 42px; display: grid; place-items: center;
  border-radius: 12px; background: var(--mgs-accent); color: #fff;
  font-size: 21px; font-weight: 750; letter-spacing: -.04em;
  box-shadow: 0 7px 16px rgba(63, 111, 229, .22);
}
.eyebrow { margin: 0 0 3px; color: var(--mgs-muted); font-size: 10px; font-weight: 700; letter-spacing: .14em; }
.brand-name { margin: 0; color: var(--mgs-text); font-size: 15px; font-weight: 700; letter-spacing: -.02em; }
.entrance-hero { padding: 14px 0 0; text-align: center; }
.hero-orbit { position: relative; width: 116px; height: 116px; margin: 0 auto 15px; display: grid; place-items: center; }
.orbit-ring { position: absolute; border: 1px solid rgba(63, 111, 229, .16); border-radius: 50%; }
.ring-one { inset: 5px; }
.ring-two { inset: 17px; border-style: dashed; }
.hero-symbol { width: 60px; height: 60px; display: grid; place-items: center; border-radius: 18px; background: var(--mgs-accent-soft); color: var(--mgs-accent); font-size: 30px; font-weight: 750; }
.orbit-dot { position: absolute; width: 7px; height: 7px; border-radius: 50%; background: var(--mgs-accent); }
.dot-one { top: 11px; right: 26px; }
.dot-two { bottom: 19px; left: 9px; opacity: .55; }
.dot-three { right: 7px; bottom: 28px; width: 5px; height: 5px; opacity: .35; }
.hero-eyebrow { margin-bottom: 10px; color: var(--mgs-accent); }
.entrance-hero h1 { max-width: 420px; margin: 0 auto; color: var(--mgs-text); font-size: clamp(24px, 4vw, 31px); line-height: 1.35; letter-spacing: -.04em; }
.hero-description { max-width: 390px; margin: 12px auto 0; color: var(--mgs-secondary); font-size: 13px; line-height: 1.9; }
.entrance-actions { display: grid; gap: 10px; margin-top: 25px; }
.entrance-actions :deep(.el-button) { width: 100%; height: 45px; margin: 0; border-radius: 9px; font-weight: 650; }
.entrance-actions :deep(.primary-action) { box-shadow: 0 7px 16px rgba(63, 111, 229, .16); }
.entrance-actions :deep(.primary-action .el-icon) { margin-left: 8px; }
.entrance-actions :deep(.secondary-action) { border-color: var(--mgs-border); color: var(--mgs-secondary); }
.entrance-actions :deep(.secondary-action:hover) { border-color: var(--mgs-accent); color: var(--mgs-accent); background: var(--mgs-accent-soft); }
.entrance-divider { display: flex; align-items: center; gap: 12px; margin-top: 22px; color: var(--mgs-muted); font-size: 10px; }
.entrance-divider::before, .entrance-divider::after { content: ''; flex: 1; height: 1px; background: var(--mgs-border); }
.entrance-footer { display: flex; align-items: center; justify-content: center; gap: 8px; margin-top: 14px; color: var(--mgs-muted); font-size: 11px; }
.status-dot { width: 6px; height: 6px; border-radius: 50%; background: var(--mgs-success); box-shadow: 0 0 0 3px rgba(57, 166, 106, .1); }
.page-footnote { margin: 0; color: var(--mgs-muted); font-size: 11px; text-align: center; }
@media (max-width: 520px) {
  .entrance-page { justify-content: flex-start; padding: 20px 12px; }
  .entrance-card { padding: 23px 20px 22px; border-radius: 13px; }
  .hero-orbit { width: 96px; height: 96px; }
  .hero-symbol { width: 52px; height: 52px; }
  .entrance-hero { padding-top: 22px; }
}
</style>
