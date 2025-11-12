<template>
  <div class="d-flex min-vh-100 bg-light">
    <aside class="sidebar bg-dark text-white p-3 shadow-sm">
      <h5 class="text-white mb-4 border-bottom pb-2">SLibrary</h5>
      <ul class="nav flex-column">
        <li class="nav-item">
          <router-link to="/app/dashboard" class="nav-link text-white" active-class="active">
            <i class="bi bi-house-door me-2"></i> Dashboard
          </router-link>
        </li>
        <li class="nav-item">
          <router-link to="/app/books" class="nav-link text-white" active-class="active">
            <i class="fas fa-book me-2"></i> Books
          </router-link>
        </li>

        <li class="nav-item">
          <router-link to="/app/reservations" class="nav-link text-white" active-class="active">
            <i class="fas fa-bookmark"></i> Reservations
          </router-link>
        </li>

        <li class="nav-item">
          <router-link to="/app/users" class="nav-link text-white" active-class="active">
            <i class="fas fa-users me-2"></i> Users
          </router-link>
        </li>
      </ul>
      <div class="mt-auto pt-3 border-top">
        <button @click="logout" class="btn btn-outline-danger w-100">
          <i class="bi bi-box-arrow-right me-2"></i> Logout
        </button>
      </div>
    </aside>
    
    <main class="flex-grow-1 p-4 overflow-auto">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import axios from 'axios'

const API_BASE_URL = 'https://localhost:7037'
const router = useRouter()

const logout = async () => {
  const token = localStorage.getItem('token')
  if (token) {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      const username = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
      await axios.patch(`${API_BASE_URL}/api/Account/SetInActive/${username}`, null)
    } catch (err) {
      console.error('Logout failed:', err)
    }
  }
  localStorage.removeItem('token')
  router.push('/')
}
</script>

<style scoped>
  .sidebar {
    width: 250px;
    height: 100vh;
    position: sticky;
    top: 0;
    flex-shrink: 0;
    display: flex;
    flex-direction: column;
  }

    .sidebar .nav-link {
      padding: 10px 15px;
      border-radius: 5px;
      transition: background-color 0.2s;
    }

      .sidebar .nav-link:hover {
        background-color: rgba(255, 255, 255, 0.1);
      }

      .sidebar .nav-link.active {
        background-color: rgba(255, 255, 255, 0.2);
        font-weight: bold;
      }
</style>
