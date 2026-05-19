import { Configuration } from "@/api/generated";

export const ApiOption = new Configuration({
    basePath: import.meta.env.VITE_API ?? 'http://localhost:5110',
    headers: {
        'Authorization': ''
    }
})