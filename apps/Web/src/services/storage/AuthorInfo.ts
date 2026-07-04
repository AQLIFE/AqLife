import { AccountApi, Configuration, type AccountDto } from "@/api/generated";
import { handle } from "@/utils/request";
import { defineStore } from "pinia";
import { ref, type Ref } from "vue";
import { ApiOption } from "./BaseOptions";

export const AuthorInfo = defineStore("GetUserInfo", () => {
    const userInfo = ref<AccountDto>({
        name: "",
        desc: "",
        avatar: null,
        subscriptions: []
    });
    // 控制骨架屏的显示
    const isShow: Ref<boolean> = ref(true);

    const api = new AccountApi(new Configuration(ApiOption))
    const getUser = async () => {
        const [response, status] = await handle(api.apiAccountGetRaw());
        // console.log(response,response)
        try{
            const data = await response?.value()
            if (status && data != null) {
                userInfo.value = data;
                isShow.value = false;
            }
        }catch{}
    }

    return { userInfo, isShow, getUser}
});