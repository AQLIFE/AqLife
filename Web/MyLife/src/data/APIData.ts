import axios from "axios";

const GithubUsersAPI = axios.create({
    baseURL:'https://api.github.com/users/',
})


export {GithubUsersAPI}