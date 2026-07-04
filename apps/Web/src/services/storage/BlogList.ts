import { defineStore } from "pinia";
import { ref, type Ref } from "vue";

export const Blog = defineStore("Blog", () => {
    const isShow: Ref<boolean> = ref(false);
    const blogTitle: Ref<string> = ref('');

    return { isShow, blogTitle };
});