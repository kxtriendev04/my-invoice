<script setup>
import TheSideBar from './TheSideBar.vue'
import TheHeader from './TheHeader.vue'
import { useRoute } from 'vue-router'

const route = useRoute()
</script>

<template>
  <div class="app">
    <TheHeader />
    <div class="container">
      <TheSideBar />
      <div class="content" :class="{ 'no-padding': route.meta.noPadding }">
        <router-view></router-view>
      </div>
    </div>
  </div>
</template>

<style scoped>
.app {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  display: flex;
  flex-direction: column;
}
.container {
  display: flex;
  width: 100%;
  height: 100%;
}
.content {
  flex: 1;
  background: #f0f2f4;
  padding: 16px 20px 20px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  max-height: calc(100vh - 48px);
  min-height: 0;
}
.content.no-padding {
  padding: 0;
}
</style>
