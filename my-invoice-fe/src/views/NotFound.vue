<template>
  <div class="not-found-page">
    <div class="card">
      <h1>404</h1>
      <p>Trang bạn yêu cầu không tồn tại.</p>
      <button @click="goHome">Về trang chủ</button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
const router = useRouter()
const goHome = () => router.push('/')
</script>

<style scoped>
.not-found-page {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100vh;
  background: #f3f4f6;
}
.card {
  background: #fff;
  padding: 40px;
  border-radius: 8px;
  text-align: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}
.card h1 {
  font-size: 72px;
  margin: 0;
}
.card p {
  margin: 12px 0 24px;
  color: #6b7280;
}
.card button {
  background: #2563eb;
  color: white;
  border: none;
  padding: 10px 16px;
  border-radius: 6px;
  cursor: pointer;
}
</style>
