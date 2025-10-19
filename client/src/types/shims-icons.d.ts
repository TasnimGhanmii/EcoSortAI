//created this module to stop ts from complaining about ~icons module not existing
//because ts only understand modules that are explicitly or physically present
declare module '~icons/*' {
  import type { DefineComponent } from 'vue';
  const component: DefineComponent;
  export default component;
}