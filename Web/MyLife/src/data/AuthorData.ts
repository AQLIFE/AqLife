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