import type { SHA256 } from "./HashData"

interface ISkillNode {
    Id: SHA256               // 节点唯一标识
    Name: string             // 技能名称
    Description: string      // 技能描述
    Prerequisites: number[]  // 前置技能节点的 id 数组
    UnlockCondition: UnlockConditionGroup // 解锁条件组
    UnlockedTime?: string    // 解锁时间
}

type SkillTree = ISkillNode[]

interface IUnlockCondition{
    ConditionTitle:string; // 条件标题
    ConditionDescription:string; // 条件描述
    Unlocked: boolean        // 是否已经解锁
}

type UnlockConditionGroup = IUnlockCondition[]


export {type ISkillNode, type SkillTree, type UnlockConditionGroup, type IUnlockCondition};