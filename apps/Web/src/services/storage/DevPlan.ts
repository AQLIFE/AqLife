import { TodoApi, type TodoDto } from "@/api/generated";
import { defineStore } from "pinia";
import { ref } from "vue";
import { ApiOption } from "./BaseOptions";
import { handle } from "@/utils/request";

export const DevPlan = defineStore("DevPlan", () => {
    const todoList = ref<TodoDto[]>([]);
    const api = new TodoApi(ApiOption)
    const getAll = async () => {
        const [response, status] = await handle(api.apiTodoGet());
        if (status) {
            todoList.value = response!;

        }
    }
    return { todoList,getAll };
});