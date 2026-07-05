import { hashShaDefault, type HashSHA } from '@/data/hashData'

interface SkillNode {
  id: HashSHA
  name: string
  description: string
  prerequisites: HashSHA[]
  unlockCondition: UnlockConditionGroup
  unlockedTime?: string
}

type SkillTree = SkillNode[]

interface UnlockCondition {
  conditionTitle: string
  conditionDescription: string
  unlocked: boolean
}

type UnlockConditionGroup = UnlockCondition[]

export const demoSkillTree: SkillTree = [
  {
    id: '6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B' as HashSHA,
    name: 'Web',
    description: '基于超文本和HTTP的全球性、动态交互的、跨平台的分布式图形信息系统',
    prerequisites: [],
    unlockCondition: [],
  },
  {
    id: hashShaDefault,
    name: 'JavaScript',
    description: '一种编程语言，常用于网页开发以实现动态效果。',
    prerequisites: [
      '6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B' as HashSHA,
    ],
    unlockCondition: [
      {
        conditionTitle: '完成JavaScript基础语法学习',
        conditionDescription: '掌握JavaScript的基本语法和使用方法。',
        unlocked: false,
      },
    ],
  },
]

export type { SkillNode, SkillTree, UnlockConditionGroup, UnlockCondition }
