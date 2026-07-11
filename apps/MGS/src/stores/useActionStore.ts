import { defineStore } from 'pinia'
import { ref ,type Ref} from 'vue'
export enum OperationalState{None,View,Add,Update,Deletet}
export function routerAction(status :OperationalState){
  switch (status){
    case OperationalState.Add:useActionStore().onAdd;break;
    case OperationalState.Update:useActionStore().onUpdate;break;
    case OperationalState.Deletet:useActionStore().onDelete;break;
  }
}
export const useActionStore = defineStore('action', () => {
  // 定义标准化的操作契约
  const onAdd = ref<(() => void | Promise<void>) | null>(null)
  const onUpdate = ref<(() => void | Promise<void>) | null>(null)
  const onDelete = ref<(() => void | Promise<void>) | null>(null)

  // 重置方法（防止路由切换后逻辑残留，符合 Fail-Fast [cite: 16]）
  function resetActions() {
    onAdd.value = null
    onUpdate.value = null
    onDelete.value = null
  }

  const OState:Ref<OperationalState> = ref(OperationalState.None)
  return { onAdd, onUpdate, onDelete, resetActions,OState }
})
