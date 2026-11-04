
<script setup lang="ts">
import { computed, ref } from 'vue'

interface Tech {
  id: string
  name: string
  description: string
  cost: number
  x: number
  y: number
  icon: string
  era: string
  requires: string[]
  unlocks: string[]
}

const technologies: Tech[] = [
  {
    id: 'biology',
    name: '基础生物学',
    description: '研究生命的基本规律，开启生物科技的发展。',
    cost: 0,
    x: 50, y: 200,
    icon: '🧬',
    era: '基础时代',
    requires: [],
    unlocks: ['农业研究', '生态学']
  },
  {
    id: 'agriculture',
    name: '农业研究',
    description: '掌握植物种植技术，提高食物生产效率。',
    cost: 30,
    x: 340, y: 70,
    icon: '🌱',
    era: '基础时代',
    requires: ['biology'],
    unlocks: ['高级农业']
  },
  {
    id: 'ecology',
    name: '生态学',
    description: '研究生态系统与环境之间的相互作用。',
    cost: 40,
    x: 340, y: 330,
    icon: '🌍',
    era: '基础时代',
    requires: ['biology'],
    unlocks: ['能源技术']
  },
  {
    id: 'advanced-agriculture',
    name: '高级农业',
    description: '解锁温室和自动化种植技术。',
    cost: 60,
    x: 650, y: 70,
    icon: '🌾',
    era: '工业时代',
    requires: ['agriculture'],
    unlocks: ['自动化']
  },
  {
    id: 'energy',
    name: '能源技术',
    description: '掌握能源转换，解锁高效发电设备。',
    cost: 80,
    x: 650, y: 330,
    icon: '⚡',
    era: '工业时代',
    requires: ['ecology'],
    unlocks: ['自动化']
  },
  {
    id: 'automation',
    name: '自动化',
    description: '利用机械与控制系统实现生产自动化。',
    cost: 120,
    x: 960, y: 200,
    icon: '⚙️',
    era: '科技时代',
    requires: ['advanced-agriculture', 'energy'],
    unlocks: []
  }
]

const researched = ref(new Set(['biology']))
const researchPoints = ref(200)
const selectedId = ref('biology')

const selected = computed(() =>
  technologies.find(t => t.id === selectedId.value)!
)

const researchedCount = computed(() => researched.value.size)

function statusOf(tech: Tech) {
  if (researched.value.has(tech.id)) return 'done'

  return tech.requires.every(id => researched.value.has(id))
    ? 'available'
    : 'locked'
}

function research(tech: Tech) {
  if (statusOf(tech) !== 'available') return
  if (researchPoints.value < tech.cost) return

  researchPoints.value -= tech.cost
  researched.value = new Set([...researched.value, tech.id])
}

function selectTech(tech: Tech) {
  selectedId.value = tech.id
}

// 连线坐标
const edges = computed(() =>
  technologies.flatMap(tech =>
    tech.requires.map(sourceId => {
      const source = technologies.find(t => t.id === sourceId)!

      return {
        id: `${sourceId}-${tech.id}`,
        x1: source.x + 100,
        y1: source.y + 54,
        x2: tech.x,
        y2: tech.y + 54,
        done: researched.value.has(tech.id)
      }
    })
  )
)
</script>

<template>
  <div class="tech-app">
    <header class="topbar">
      <div>
        <div class="brand">TECHNOLOGY TREE</div>
        <div class="subtitle">科技研发中心 / RESEARCH CENTER</div>
      </div>

      <div class="stats">
        <div class="stat">
          <span>⚡</span>
          {{ researchPoints }} RP
        </div>
        <div class="stat">
          {{ researchedCount }} / {{ technologies.length }} 已研发
        </div>
      </div>
    </header>

    <main class="workspace">
      <section class="tree-area">
        <div class="era-label era-one">基础时代</div>
        <div class="era-label era-two">工业时代</div>
        <div class="era-label era-three">科技时代</div>

        <svg class="connections"
             viewBox="0 0 1200 560"
             preserveAspectRatio="xMinYMin meet">
          <defs>
            <marker id="arrow" markerWidth="8"
                    markerHeight="8" refX="6" refY="3"
                    orient="auto">
              <path d="M0,0 L0,6 L6,3 Z" fill="#657b83"/>
            </marker>
          </defs>

          <path
            v-for="edge in edges"
            :key="edge.id"
            :d="`M ${edge.x1} ${edge.y1}
                C ${edge.x1 + 60} ${edge.y1},
                  ${edge.x2 - 60} ${edge.y2},
                  ${edge.x2} ${edge.y2}`"
            :class="edge.done ? 'edge done' : 'edge'"
            marker-end="url(#arrow)"
          />
        </svg>

        <button
          v-for="tech in technologies"
          :key="tech.id"
          class="tech-node"
          :class="[
            'node-' + statusOf(tech),
            { selected: selectedId === tech.id }
          ]"
          :style="{ left: tech.x + 'px', top: tech.y + 'px' }"
          @click="selectTech(tech)"
        >
          <div class="node-icon">{{ tech.icon }}</div>
          <div class="node-info">
            <div class="node-era">{{ tech.era }}</div>
            <div class="node-name">{{ tech.name }}</div>
            <div class="node-cost">
              {{ statusOf(tech) === 'done'
                ? '✓ 已完成'
                : `${tech.cost} RP` }}
            </div>
          </div>
          <span class="node-status">
            {{ statusOf(tech) === 'done' ? '✓' :
               statusOf(tech) === 'available' ? '●' : '🔒' }}
          </span>
        </button>
      </section>

      <aside class="detail-panel">
        <div class="panel-title">TECHNOLOGY DETAILS</div>

        <div class="detail-icon">{{ selected.icon }}</div>

        <div class="detail-era">{{ selected.era }}</div>
        <h2>{{ selected.name }}</h2>

        <p class="description">
          {{ selected.description }}
        </p>

        <div class="detail-divider"></div>

        <div class="detail-row">
          <span>研发成本</span>
          <strong>{{ selected.cost }} RP</strong>
        </div>

        <div class="detail-row">
          <span>前置科技</span>
          <strong>
            {{ selected.requires.length
              ? selected.requires.map(id =>
                  technologies.find(t => t.id === id)?.name
                ).join('、')
              : '无' }}
          </strong>
        </div>

        <div class="detail-divider"></div>

        <div class="unlock-title">解锁内容</div>
        <div
          v-for="item in selected.unlocks"
          :key="item"
          class="unlock-item"
        >
          <span>✦</span> {{ item }}
        </div>

        <div class="research-action">
          <button
            v-if="statusOf(selected) === 'available'"
            :disabled="researchPoints < selected.cost"
            @click="research(selected)"
          >
            {{ researchPoints >= selected.cost
              ? '开始研发'
              : '研发点数不足' }}
          </button>

          <button v-else-if="statusOf(selected) === 'done'"
                  disabled>
            ✓ 已完成研发
          </button>

          <button v-else disabled>
            🔒 前置科技未完成
          </button>
        </div>
      </aside>
    </main>

    <footer class="footer">
      <span>MONIREPO / TECHNOLOGY SYSTEM</span>
      <span>RESEARCH PROGRESS:
        {{ Math.round(researchedCount / technologies.length * 100) }}%
      </span>
    </footer>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.tech-app {
  min-height: 680px;
  background: #101a20;
  color: #dce8e8;
  font-family: Inter, "Microsoft YaHei", sans-serif;
  border: 1px solid #354951;
  overflow: hidden;
}

.topbar {
  height: 76px;
  padding: 16px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #17262d;
  border-bottom: 1px solid #354951;
}

.brand {
  font-weight: 800;
  letter-spacing: 3px;
  color: #a9d8ce;
  font-size: 16px;
}

.subtitle, .panel-title {
  font-size: 10px;
  letter-spacing: 2px;
  color: #718c94;
  margin-top: 5px;
}

.stats {
  display: flex;
  gap: 12px;
}

.stat {
  padding: 9px 14px;
  background: #24373e;
  border: 1px solid #40565d;
  font-size: 12px;
  border-radius: 4px;
}

.workspace {
  display: flex;
  min-height: 560px;
  overflow: auto;
}

.tree-area {
  position: relative;
  flex: 1;
  min-width: 1200px;
  height: 560px;
  overflow: hidden;
  background-color: #132128;
  background-image:
    linear-gradient(#30434a33 1px, transparent 1px),
    linear-gradient(90deg, #30434a33 1px, transparent 1px);
  background-size: 32px 32px;
}

.era-label {
  position: absolute;
  top: 20px;
  font-size: 11px;
  letter-spacing: 2px;
  color: #627b82;
}

.era-one { left: 50px; }
.era-two { left: 650px; }
.era-three { left: 960px; }

.connections {
  position: absolute;
  width: 1200px;
  height: 560px;
  inset: 0;
  pointer-events: none;
}

.edge {
  fill: none;
  stroke: #52666d;
  stroke-width: 2;
  stroke-dasharray: 6 5;
}

.edge.done {
  stroke: #73c9a6;
  stroke-width: 3;
  stroke-dasharray: none;
}

.tech-node {
  position: absolute;
  width: 200px;
  height: 108px;
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  text-align: left;
  color: #b3c5c8;
  background: #23343b;
  border: 1px solid #526970;
  border-radius: 5px;
  cursor: pointer;
  transition: .2s;
  z-index: 2;
}

.tech-node:hover {
  transform: translateY(-3px);
  border-color: #a9d8ce;
}

.tech-node.selected {
  box-shadow: 0 0 0 2px #8acdb8;
}

.node-icon {
  font-size: 28px;
}

.node-info {
  flex: 1;
}

.node-era {
  font-size: 9px;
  color: #80999e;
  margin-bottom: 6px;
}

.node-name {
  font-weight: 700;
  font-size: 13px;
}

.node-cost {
  font-size: 10px;
  margin-top: 8px;
  color: #9eb5b8;
}

.node-status {
  font-size: 12px;
}

.node-done {
  border-color: #69bc98;
  background: #1d403b;
  color: #b9f0d6;
}

.node-available {
  border-color: #d6b46b;
  background: #403a29;
  color: #f2d99c;
}

.node-locked {
  opacity: .65;
}

.detail-panel {
  width: 290px;
  min-width: 290px;
  padding: 24px 20px;
  background: #1b2b32;
  border-left: 1px solid #40535a;
}

.detail-icon {
  font-size: 42px;
  margin-top: 35px;
}

.detail-era {
  margin-top: 18px;
  font-size: 11px;
  color: #a9c7bb;
}

.detail-panel h2 {
  font-size: 20px;
  margin: 8px 0 16px;
}

.description {
  font-size: 12px;
  line-height: 1.8;
  color: #9db1b4;
  min-height: 65px;
}

.detail-divider {
  height: 1px;
  background: #3b5058;
  margin: 20px 0;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  gap: 8px;
  font-size: 11px;
  margin: 15px 0;
}

.detail-row span {
  color: #81999e;
}

.detail-row strong {
  text-align: right;
  font-weight: 500;
}

.unlock-title {
  font-size: 12px;
  margin-bottom: 12px;
}

.unlock-item {
  font-size: 12px;
  padding: 8px 0;
  color: #a9d8ce;
}

.unlock-item span {
  margin-right: 8px;
}

.research-action {
  margin-top: 32px;
}

.research-action button {
  width: 100%;
  height: 42px;
  background: #527e6d;
  border: 1px solid #8ec7a8;
  color: white;
  font-weight: bold;
  cursor: pointer;
  border-radius: 3px;
}

.research-action button:disabled {
  background: #35464b;
  color: #819397;
  border-color: #46575c;
  cursor: not-allowed;
}

.footer {
  height: 38px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
  font-size: 9px;
  letter-spacing: 1px;
  color: #718990;
  background: #17262d;
  border-top: 1px solid #354951;
}

@media (max-width: 700px) {
  .workspace {
    flex-direction: column;
  }

  .detail-panel {
    width: 100%;
  }
}
</style>
