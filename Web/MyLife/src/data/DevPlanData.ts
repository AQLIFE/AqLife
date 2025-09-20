interface IDevTask {
    content:string;
    State: boolean;
}
const DevPlan :IDevTask[]= [
    {content:'创建一个开发计划面板',State:true},
    {content:'重写作者面板',State:true},
    {content:'添加界面导航',State:false},
    {content:'实现开发计划的本地存储',State:false},
    {content:'实现开发计划的增加功能',State:true},
    {content:'为开发计划添加自动化序列',State:true},
    {content:'添加动态Author信息',State:true},
    {content:'添加pinia实现',State:true},
    {content:'添加加载占位-Author',State:true},
]

export {DevPlan};