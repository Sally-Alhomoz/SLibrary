<!--<template>
  <div class="d-flex min-vh-100 bg-light">

    <aside class="sidebar bg-dark text-white p-3 shadow-sm">
      <h5 class="text-white mb-4 border-bottom pb-2">Navigation</h5>

      <ul class="nav flex-column">
        <li class="nav-item">
          <router-link to="/dashboard" class="nav-link text-white active">
            <i class="bi bi-house-door me-2"></i> Dashboard
          </router-link>
        </li>

        <li class="nav-item">
          <router-link to="/books" class="nav-link text-white">
            <i class="bi bi-book me-2"></i> Book List
          </router-link>
        </li>
      </ul>

      <div class="mt-auto pt-3 border-top">
        <button @click="logout" class="btn btn-outline-danger w-100">
          <i class="bi bi-box-arrow-right me-2"></i> Logout
        </button>
      </div>
    </aside>

    <main class="flex-grow-1 p-4">

      <div class="d-flex justify-content-start align-items-center gap-3 mb-4">
        <h3 class="mb-0 text-primary">Welcome, {{ username }}!</h3>
      </div>

    </main>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

const API_BASE_URL = 'https://localhost:7037';

const username = ref('')
const role = ref('')
const router = useRouter()

onMounted(() => {
  const token = localStorage.getItem('token')
  if (!token) router.push('/')
  else {
    // Decode JWT to get user info
    const payload = JSON.parse(atob(token.split('.')[1]))
    username.value = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
    role.value = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  }
})

  const logout = async () => {
    const currentUsername = username.value;

    await axios.patch(
      `${API_BASE_URL}/api/Account/SetInActive/${currentUsername}`,null);

  localStorage.removeItem('token')
  router.push('/')
}
</script>-->
<!--<style>
  .sidebar {
    width: 250px;
    display: flex;
    flex-direction: column;
    height: 100vh;
    position: sticky;
    top: 0;
    text-align: left;
  }

    .sidebar .nav-link {
      padding: 10px 15px;
      border-radius: 5px;
      transition: background-color 0.2s;
    }

      .sidebar .nav-link:hover {
        background-color: rgba(255, 255, 255, 0.1);
      }
</style>-->


<template>
  <div>
    <h3 class="mb-4 text-primary">Welcome, {{ username }}!</h3>
    <p>This is your dashboard.</p>
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
