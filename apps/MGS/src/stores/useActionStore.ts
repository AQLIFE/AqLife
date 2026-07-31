import { defineStore } from 'pinia'
import { ref ,type Ref} from 'vue'
export enum OperationalState{None,View,Add,Update,Deletet}
// export function routerAction(status :OperationalState){
//   switch (status){
//     case OperationalState.Add:useActionStore().onAdd;break;
//     case OperationalState.Update:useActionStore().onUpdate;break;
//     case OperationalState.Deletet:useActionStore().onDelete;break;
//   }
// }
export const useActionStore = defineStore('action', () => {

  const OState:Ref<OperationalState> = ref(OperationalState.None)
  return { OState }
})
