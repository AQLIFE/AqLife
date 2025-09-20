import type { Component } from "vue";

interface IAuthor{
    name :string;
    desc:string;
    contacts:IAuthorContact[];
}
interface IAuthorContact{
    platform:string;
    link:string;
    icon:Component;
}

interface IGithubUser {
  login: string;           // 用户名
  avatar_url: string;      // 头像URL
  name: string | null;     // 显示名称
  html_url: string;        // 个人主页URL
  bio: string | null;      // 个人简介
}


class GithubUser implements IGithubUser {
  login = "";
  avatar_url = "";
  name = null;
  html_url = "";
  bio = null;
}

export type { IAuthor, IAuthorContact, IGithubUser};
export { GithubUser };