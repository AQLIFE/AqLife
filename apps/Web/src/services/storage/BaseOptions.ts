
const Token = '';
export const ApiOption = {
    basePath: import.meta.env.VITE_API ?? 'http://localhost:5110',
    headers: {
        'Authorization': Token
    }
}
