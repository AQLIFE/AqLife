import { defineStore } from "pinia";
import { GithubUser, type IGithubUser } from "@/data/AuthorData";
import { GithubUsersAPI } from "@/data/APIData";
import { reactive, ref, type Reactive, type Ref } from "vue";

const AuthorInfo = defineStore("GetUserInfo",()=> {
    const userInfo:Reactive<IGithubUser>= reactive<IGithubUser>(new GithubUser());
    const isReady:Ref<boolean> = ref(false);

    async function getUserInfo(username:string,state:boolean = false){
        const response = await GithubUsersAPI.get(username);
        if(response.status === 200){
            const data = await response.data;
            Object.assign(userInfo,data);
            if(state) isReady.value = true;
            
        }else{
            throw new Error(`Failed to fetch user info: ${response.status} ${response.statusText}`);
        }
    }
    return {userInfo,isReady,getUserInfo}
});

const DevPlan = defineStore("DevPlan",()=>{
    const isShowDevPlan:Ref<boolean> = ref(false);

    function watchNav(state:boolean):void{isShowDevPlan.value = state;}
    return {isShowDevPlan,watchNav};
});

export {AuthorInfo,DevPlan}