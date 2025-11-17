<template>
  <div>
    <h3 class="mb-4 text-primary">Welcome, {{ username }}!</h3>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'

  const username = ref('')

  onMounted(() => {
    const token = localStorage.getItem('token')
    if (token) {
      const payload = JSON.parse(atob(token.split('.')[1]))
      username.value = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
    }
  })
</script>
