import {type RouteRecordRaw} from 'vue-router'
import { routerRaw } from '@/services/router';

interface INavData {
    NavTitle:string;
    NavObj:RouteRecordRaw
}

const NavData:INavData[]=[
    {NavTitle:'关于',NavObj:routerRaw[2]},
    {NavTitle:'分享',NavObj:routerRaw[3]},
    {NavTitle:'建文',NavObj:routerRaw[4]},
    {NavTitle:'计划',NavObj:routerRaw[5]},
    {NavTitle:'模板页',NavObj:routerRaw[6]},
]

export {NavData};