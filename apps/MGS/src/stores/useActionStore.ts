import { defineStore } from 'pinia'
import { ref ,type Ref} from 'vue'
export enum OperationalState{None,View,Add,Update,Deletet,Upload}
export const useActionStore = defineStore('action', () => {

  const OState:Ref<OperationalState> = ref(OperationalState.None)
  return { OState }
})
