<template>
  <div class="d-flex min-vh-100 bg-light">

    <aside class="sidebar bg-sidebar-gray text-dark p-3 shadow-sm" :class="{ 'closed': !isSidebarOpen }">

      <div class="d-flex justify-content-between align-items-center px-3 pt-3 pb-3 border-bottom border-secondary">
        <h4 class="m-0 text-white fw-bold" v-if="isSidebarOpen">SLibrary</h4>
        <button @click="toggleSidebar" class="btn btn-ghost toggle-btn" title="Toggle Sidebar">
          <i class="fas fa-bars me-2"></i>
        </button>
      </div>

      <ul class="nav flex-column flex-grow-1">

        <li class="nav-item">
          <router-link to="/app/dashboard" class="nav-link" active-class="active">
            <i class="fas fa-home me-2"></i>
            <span v-if="isSidebarOpen">Dashboard</span>
          </router-link>
        </li>

        <li class="nav-item">
          <router-link to="/app/books" class="nav-link" active-class="active">
            <i class="fa fa-book me-2"></i>
            <span v-if="isSidebarOpen">Books</span>
          </router-link>
        </li>

        <li class="nav-item">
          <router-link to="/app/reservations" class="nav-link" active-class="active">
            <i class="fa fa-bookmark me-2"></i>
            <span v-if="isSidebarOpen">Reservations</span>
          </router-link>
        </li>

        <li class="nav-item" v-if="isAdmin">
          <router-link to="/app/users" class="nav-link" active-class="active">
            <i class="fas fa-users me-2"></i>
            <span v-if="isSidebarOpen">Users</span>
          </router-link>
        </li>
      </ul>


      <div class="mt-auto pt-3 border-top">
        <router-link to="/app/profile" class="btn btn-outline-dark w-100">
          <i class="fas fa-user-circle me-2"></i>
          <span v-if="isSidebarOpen">Profile</span>
        </router-link>
      </div>

      <div class="mt-auto pt-3">
        <button @click="logout" class="btn btn-outline-danger w-100" :class="{ 'btn-sm': !isSidebarOpen }">
          <i class="fas fa-sign-out-alt me-2"></i>
          <span v-if="isSidebarOpen">Logout</span>
        </button>
      </div>
    </aside>

    <main class="flex-grow-1 p-4 overflow-auto">
      <router-view />
    </main>
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue'
  import { useRouter } from 'vue-router'
  import axios from 'axios'

  const isSidebarOpen = ref(true)

  const toggleSidebar = () => {
    isSidebarOpen.value = !isSidebarOpen.value
  }

  const API_BASE_URL = 'https://localhost:7037';
  const router = useRouter();

  const getToken = () => {
    return localStorage.getItem('token');
  };

  const getRole = () => {
    const token = localStorage.getItem('token')
    if (!token) {
      return null
    }

    const payload = JSON.parse(atob(token.split('.')[1]))
    return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  }

  const isAdmin = computed(() => {
    const role = getRole()
    return role === 'Admin'
  })


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
  .bg-sidebar-gray {
    background-color: #A9A9A9 !important;
    border-right: 1px solid #E0E0E0;
    position: relative; 
  }

  .sidebar {
    width: 250px;
    height: 100vh;
    position: sticky;
    top: 0;
    flex-shrink: 0;
    display: flex;
    flex-direction: column;
    transition: width 0.3s ease;
  }

    .sidebar.closed {
      width: 60px; 
      padding-left: 0.5rem !important;
      padding-right: 0.5rem !important;
    }

      .sidebar.closed .nav-link {
        padding: 10px 5px; 
        text-align: center;
      }

        .sidebar.closed .nav-link i.me-2 {
          margin-right: 0 !important;
        }

      .sidebar.closed h4 {
        display: none;
      }

      .sidebar.closed .border-bottom {
        justify-content: center !important;
        padding-left: 0 !important;
        padding-right: 0 !important;
      }

      .sidebar.closed .btn-outline-danger {
        padding-left: 0;
        padding-right: 0;
      }

    .sidebar .nav-link {
      color: #343a40 !important;
      padding: 10px 15px;
      border-radius: 5px;
      transition: background-color 0.2s;
    }

      .sidebar .nav-link:hover {
        background-color: #F0F0F0;
        color: #343a40 !important;
      }

      .sidebar .nav-link.active {
        background-color: #EDEDED;
        font-weight: bold;
        color: #495057 !important;
      }

    .sidebar h4 {
      color: black !important;
    }
</style>
