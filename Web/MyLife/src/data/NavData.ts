import {type RouteRecordRaw} from 'vue-router'
import { Comment,type Component } from 'vue';
import { routerRaw } from '@/services/router';

import AboutIcon from '@/assets/icons/AboutIcon.vue';
import ShareIcon from '@/assets/icons/ShareIcon.vue';
import BlogIcon from  '@/assets/icons/BlogIcon.vue';
import PlanIcon from  '@/assets/icons/PlanIcon.vue';
// import SkillIcon from '@/assets/icons/SkillIcon.vue';
import WishIcon from  '@/assets/icons/WishIcon.vue';
import RandomIcon from '@/assets/icons/RandomIcon.vue';

interface INavData {
    NavTitle:string;
    NavObj:RouteRecordRaw,
    NavIcon:Comment|HTMLElement|Component
}

const NavData:INavData[]=[
    {NavTitle:'关于',NavObj:routerRaw[2],NavIcon:AboutIcon},
    {NavTitle:'分享',NavObj:routerRaw[3],NavIcon:ShareIcon},
    {NavTitle:'建文',NavObj:routerRaw[4],NavIcon:BlogIcon},
    {NavTitle:'计划',NavObj:routerRaw[5],NavIcon:PlanIcon},
    // {NavTitle:'技能&成就',NavObj:routerRaw[6],NavIcon:SkillIcon},
    {NavTitle:'心愿',NavObj:routerRaw[7],NavIcon:WishIcon},
    {NavTitle:'测试',NavObj:routerRaw[8],NavIcon:RandomIcon},
]

export {NavData};