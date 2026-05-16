import { defineStore } from "pinia";
import { reactive, ref, type Reactive, type Ref } from "vue";

import { Configuration, type AccountDto } from "@/api/generated";

const ApiOption = new Configuration({
    basePath:'http://localhost:5110',
    headers:{
        'Authorization':''
    }
})

const AuthorInfo = defineStore("GetUserInfo",()=> {
    const userInfo:Reactive<AccountDto>= reactive<AccountDto>({
        name:"",
        subscriptions:[]
    });
    // 控制骨架屏的显示
    const isShow:Ref<boolean> = ref(true);

    return {userInfo,isShow}
});

const DevPlan = defineStore("DevPlan",()=>{
    const todoList = ref([]); 
    function fetchTodos() { /* ... */ }

    return { todoList, fetchTodos };
});

const Blog = defineStore("Blog",()=>{
    const isShow:Ref<boolean> = ref(false);
    const blogTitle: Ref<string> = ref('');

    return { isShow, blogTitle };
});

export {AuthorInfo,DevPlan,Blog,ApiOption}