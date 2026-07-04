interface IDevTask {
    content:string;
    State: boolean;
}
const DevPlan :IDevTask[]= [
    {content:'创建一个开发计划面板',State:true},
    {content:'重写作者面板',State:true},
    {content:'添加界面导航',State:true},
    {content:'实现开发计划的本地存储',State:false},
    {content:'实现开发计划的增加功能',State:false},
    {content:'为开发计划添加自动化序列',State:false},
    {content:'添加动态Author信息',State:true},
    {content:'添加pinia实现',State:true},
    {content:'添加加载占位-Author',State:true},
    {content:'开发一个技能树组件-大工程(延期)',State:false},
    {content:'增设分享/心愿内容',State:true},
    {content:'初始实现md文档预览',State:true},
    {content:'md代码块初始化,实现mermaid的图表',State:true},
    {content:'md代码块主要功能完工,可复制代码',State:true},
    {content:'JSX代码需要被Vue组件替换,待实现',State:false},
]

export {DevPlan};