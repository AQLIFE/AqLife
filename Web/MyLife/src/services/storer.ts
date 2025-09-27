import { defineStore } from "pinia";
import { GithubUser, type IGithubUser } from "@/data/AuthorData";
import { GithubUsersAPI } from "@/data/APIData";
import { reactive, ref, type Reactive, type Ref } from "vue";
import type { IAnchor } from "@/data/AnchorData";

const AuthorInfo = defineStore("GetUserInfo",()=> {
    const userInfo:Reactive<IGithubUser>= reactive<IGithubUser>(new GithubUser());
    const isShow:Ref<boolean> = ref(false);

    async function getUserInfo(username:string,state:boolean = false){
        const response = await GithubUsersAPI.get(username);
        if(response.status === 200){
            const data = await response.data;
            Object.assign(userInfo,data);
            if(state) isShow.value = true;
            
        }else{
            throw new Error(`Failed to fetch user info: ${response.status} ${response.statusText}`);
        }
    }
    return {userInfo,isShow,getUserInfo}
});

const DevPlan = defineStore("DevPlan",()=>{
    const isShow:Ref<boolean> = ref(false);

    function watchNav(state:boolean):void{isShow.value = state;}
    return {isShow,watchNav};
});

const Blog = defineStore("Blog",()=>{
    const isShow:Ref<boolean> = ref(false);
    const blogTitle:Ref<string> = ref('');
    const anchorList:Ref<IAnchor[]> = ref([] as IAnchor[]);
    const blogCacheList = [{blogTitle:'',blogContent:''}]; 

    return {isShow,blogTitle,anchorList,blogCacheList};
});

export {AuthorInfo,DevPlan,Blog}