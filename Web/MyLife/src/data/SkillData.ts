import {   HashSHADefault, type HashSHA } from "@/data/HashData"

interface ISkillNode {
    Id: HashSHA       // 节点唯一标识
    Name: string             // 技能名称
    Description: string      // 技能描述
    Prerequisites: HashSHA[]  // 前置技能节点的 id 数组
    UnlockCondition: UnlockConditionGroup // 解锁条件组
    UnlockedTime?: string    // 解锁时间
}

type SkillTree = ISkillNode[]

interface IUnlockCondition {
    ConditionTitle: string; // 条件标题
    ConditionDescription: string; // 条件描述
    Unlocked: boolean        // 是否已经解锁
}

type UnlockConditionGroup = IUnlockCondition[]


const demoData: SkillTree = [
    {
        Id:'6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B' as HashSHA,
        Name:'Web',
        Description:'基于超文本和HTTP的全球性、动态交互的、跨平台的分布式图形信息系统',
        Prerequisites:[],
        UnlockCondition:[]
    },
    {
        Id: 'D4735E3A265E16EEE03F59718B9B5D03019C07D8B6C51F90DA3A666EEC13AB35' as HashSHA,
        Name: "HTML",
        Description: "超文本标记语言，是构建网页的基础。",
        Prerequisites: [
            '6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B' as HashSHA
        ],
        UnlockCondition: [
            {
                ConditionTitle:'了解 HTML 标签',
                ConditionDescription:'学习并使用常用标签制作页面或者组件',
                Unlocked:false
            },
            {
                ConditionTitle:'了解 HTML 标签常用属性',
                ConditionDescription:'学习并使用常用的标签属性',
                Unlocked:false
            },
            {
                ConditionTitle:'了解 HTML DOM 状态',
                ConditionDescription:'学习 DOM 冒泡机制以及各DOM常见状态',
                Unlocked:false
            }
        ]
    },
    {
        Id: '4E07408562BEDB8B60CE05C1DECFE3AD16B72230967DE01F640B7E4729B49FCE' as HashSHA,
        Name: "CSS",
        Description: "层叠样式表，用于描述网页的外观和格式。",
        Prerequisites: [
            'D4735E3A265E16EEE03F59718B9B5D03019C07D8B6C51F90DA3A666EEC13AB35' as HashSHA
        ],
        UnlockCondition: [
            {
                ConditionTitle:'了解 CSS 属性',
                ConditionDescription:'学习常用属性并为组件附加上对应效果',
                Unlocked:false
            },
            {
                ConditionTitle:'了解 CSS 选择器',
                ConditionDescription:'学习常用DOM选择器,并实践',
                Unlocked:false
            },
            {
                ConditionTitle:'了解 CSS 媒体查询',
                ConditionDescription:'实现单一页面/组件的不同大小显示',
                Unlocked:false
            },
            {
                ConditionTitle:'了解 CSS 动画设置',
                ConditionDescription:'学习并设置组件的动画效果',
                Unlocked:false
            },
            {
                ConditionTitle:'学习 CSS 常用布局',
                ConditionDescription:'实践布局 Flex/Grid',
                Unlocked:false
            }
        ]

    },
    {
        Id: HashSHADefault,
        Name: "JavaScript",
        Description: "一种编程语言，常用于网页开发以实现动态效果。",
        Prerequisites: [
            '6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B' as HashSHA
        ],
        UnlockCondition: [
            {
                ConditionTitle: "完成JavaScript基础语法学习",
                ConditionDescription: "掌握JavaScript的基本语法和使用方法。",
                Unlocked: false,
            }
        ]
    },
    {
        Id: 'E7F6C011776E8DB7CD330B54174FD76F7D0216B612387A5FFCFB81E6F0919683' as HashSHA,
        Name: "NET Core",
        Description: "一种编程语言，常用于动态网站或API开发以实现动态效果。",
        Prerequisites: [
            '6B86B273FF34FCE19D6B804EFF5A3F5747ADA4EAA22F1D49C01E52DDB7875B4B' as HashSHA
        ],
        UnlockCondition: [
            {
                ConditionTitle: "完成JavaScript基础语法学习",
                ConditionDescription: "掌握JavaScript的基本语法和使用方法。",
                Unlocked: false,
            }
        ]
    }
]

export { demoData,type ISkillNode, type SkillTree, type UnlockConditionGroup, type IUnlockCondition };