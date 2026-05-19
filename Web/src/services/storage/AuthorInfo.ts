import { AccountApi, type AccountDto } from "@/api/generated";
import { handle } from "@/utils/request";
import { defineStore } from "pinia";
import { ref, type Ref } from "vue";
import { ApiOption } from "./BaseOptions";

export const AuthorInfo = defineStore("GetUserInfo", () => {
    const userInfo = ref<AccountDto>({
        name: "",
        desc: "",
        subscriptions: []
    });
    // 控制骨架屏的显示
    const isShow: Ref<boolean> = ref(true);

    const api = new AccountApi(ApiOption)
    const getUser = async () => {
        const [response, status] = await handle(api.apiAccountGet());
        if (status) {
            userInfo.value = response!;
            isShow.value = false;
        }
    }
    // const register = async(account:AccountDto,key:string)=>{
    //     // const [data, valid] = await handle(accountAPI.apiAccountPost({ secretKey: key, name: account.name, desc: account.desc }))
    // }

    return { userInfo, isShow, getUser}
});