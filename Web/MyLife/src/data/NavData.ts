import {type RouteRecordRaw} from 'vue-router'
import { routerRaw } from '@/services/router';

import AboutIcon from '@/assets/icons/AboutIcon.svg';
import ShareIcon from '@/assets/icons/ShareIcon.svg';
import BlogIcon from  '@/assets/icons/BlogIcon.svg';
import PlanIcon from  '@/assets/icons/PlanIcon.svg';
// import SkillIcon from '@/assets/icons/SkillIcon.svg';
import WishIcon from  '@/assets/icons/WishIcon.svg';

interface INavData {
    NavTitle:string;
    NavObj:RouteRecordRaw,
    NavIcon:string
}

const NavData:INavData[]=[
    {NavTitle:'关于',NavObj:routerRaw[2],NavIcon:AboutIcon},
    {NavTitle:'分享',NavObj:routerRaw[3],NavIcon:ShareIcon},
    {NavTitle:'建文',NavObj:routerRaw[4],NavIcon:BlogIcon},
    {NavTitle:'计划',NavObj:routerRaw[5],NavIcon:PlanIcon},
    // {NavTitle:'技能&成就',NavObj:routerRaw[6],NavIcon:SkillIcon},
    {NavTitle:'心愿',NavObj:routerRaw[7],NavIcon:WishIcon},
]

export {NavData};