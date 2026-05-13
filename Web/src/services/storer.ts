import { defineStore } from "pinia";
import { reactive, ref, type Reactive, type Ref } from "vue";
import type { IAnchor } from "@/data/AnchorData";
import { Configuration, type AccountDto } from "@/api/generated";

const ApiOption = new Configuration({
    basePath:'http://localhost:5110',
    headers:{
        'Authorization':''
    }
})

const AuthorInfo = defineStore("GetUserInfo",()=> {
    const userInfo:Reactive<AccountDto>= reactive<AccountDto>({
        name:"尚未配置博客账户名称",
        subscriptions:[]
    });
    // 控制骨架屏的显示
    const isShow:Ref<boolean> = ref(false);

    return {userInfo,isShow}
});

const DevPlan = defineStore("DevPlan",()=>{
    const todoList = ref([]); 
    function fetchTodos() { /* ... */ }

    return { todoList, fetchTodos };
});

const Blog = defineStore("Blog",()=>{
    const isShow:Ref<boolean> = ref(false);
    const isCache:Ref<boolean> = ref(false);
    const blogTitle:Ref<string> = ref('');
    const anchorList:Ref<IAnchor[]> = ref([] as IAnchor[]);
    const blogCacheList = [{blogTitle:'',blogContent:''}]; 

    function watchNav(state:boolean):void{isShow.value = state;}

    return {isShow,isCache,blogTitle,anchorList,blogCacheList,watchNav};
});

export {AuthorInfo,DevPlan,Blog,ApiOption}