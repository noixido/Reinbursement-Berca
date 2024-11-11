<template>
<div >
    <RouterView/>
  </div>
</template>

<script>
import { RouterView } from 'vue-router';
import Login from './components/Auth/Login.vue';

export default{
  components:{
    Login,
    RouterView
  },
  setup(){}
};
</script>

<style lang="scss" scoped></style>
