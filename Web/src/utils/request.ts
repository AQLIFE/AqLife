import { ElMessage } from "element-plus";
export async function handle<T>(promise: Promise<T>): Promise<[T | null, boolean]> {
    try {
        const data = await promise;
        return [data, true];
    } catch (error) {
        ElMessage.error("服务器错误"+error)
        console.error("请求错误:"+error);
        return [null, false ];
    }
}