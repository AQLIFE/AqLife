export interface DevTask {
  content: string
  state: boolean
}

export const devPlanTasks: DevTask[] = [
  { content: '创建一个开发计划面板', state: true },
  { content: '重写作者面板', state: true },
  { content: '添加界面导航', state: true },
  { content: '实现开发计划的本地存储', state: false },
  { content: '实现开发计划的增加功能', state: false },
  { content: '为开发计划添加自动化序列', state: false },
  { content: '添加动态Author信息', state: true },
  { content: '添加pinia实现', state: true },
  { content: '添加加载占位-Author', state: true },
  { content: '开发一个技能树组件-大工程(延期)', state: false },
  { content: '增设分享/心愿内容', state: true },
  { content: '初始实现md文档预览', state: true },
  { content: 'md代码块初始化,实现mermaid的图表', state: true },
  { content: 'md代码块主要功能完工,可复制代码', state: true },
  { content: 'JSX代码需要被Vue组件替换,待实现', state: false },
]
