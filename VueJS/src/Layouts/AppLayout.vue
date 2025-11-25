<template>
  <div class="app-layout">
    <aside class="sidebar" :class="{ 'closed': !isSidebarOpen }">
      <div class="sidebar-header">
        <h4 class="logo-text" v-if="isSidebarOpen">
          <i class="fas fa-book-open logo-icon"></i> SLibrary
        </h4>
        <button @click="toggleSidebar" class="toggle-btn" title="Toggle Sidebar">
          <i class="fas fa-bars"></i>
        </button>
      </div>

      <ul class="nav-menu">
        <li class="nav-item">
          <router-link to="/app/dashboard" class="nav-link" active-class="active">
            <i class="fas fa-home me-2"></i>
            <span class="link-text" v-if="isSidebarOpen">Home</span>
          </router-link>
        </li>

        <li class="nav-item">
          <router-link to="/app/books" class="nav-link" active-class="active">
            <i class="fa fa-book me-2"></i>
            <span class="link-text" v-if="isSidebarOpen">Books</span>
          </router-link>
        </li>

        <li class="nav-item">
          <router-link to="/app/reservations" class="nav-link" active-class="active">
            <i class="fa fa-bookmark me-2"></i>
            <span class="link-text" v-if="isSidebarOpen">Reservations</span>
          </router-link>
        </li>

        <li class="nav-item" v-if="isAdmin">
          <router-link to="/app/users" class="nav-link" active-class="active">
            <i class="fas fa-users me-2"></i>
            <span class="link-text" v-if="isSidebarOpen">Users</span>
          </router-link>
        </li>
      </ul>

      <div class="sidebar-footer">

        <button @click="toggleUserMenu" class="user-profile-toggle" :class="{ 'active': isUserMenuOpen }">
          <i class="fas fa-user-circle user-icon me-2"></i>
          <span class="link-text user-name" v-if="isSidebarOpen"><strong>{{currentUsername}}</strong></span>
          <i v-if="isSidebarOpen" class="fas fa-chevron-up toggle-arrow"></i>
        </button>

        <div v-if="isUserMenuOpen" class="user-dropdown-menu">
          <router-link to="/app/profile" class="dropdown-item profile-item" @click="isUserMenuOpen = false">
            <i class="fas fa-user-circle me-2"></i> Profile
          </router-link>
          <div class="dropdown-divider"></div>
          <button @click="logout" class="dropdown-item logout-item">
            <i class="fas fa-sign-out-alt me-2 text-danger"></i> Logout
          </button>
        </div>
      </div>
    </aside>

    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<script setup>
  import { ref, computed } from 'vue'
  import { useRouter } from 'vue-router'
  import { useAuth } from '@/Component/AuthServices/Authentication'


  const isSidebarOpen = ref(true)
  const isUserMenuOpen = ref(false)
  const { isAdmin, currentUsername, logout: authLogout } = useAuth()


  const toggleSidebar = () => {
    isSidebarOpen.value = !isSidebarOpen.value
  }

  const toggleUserMenu = () => {
    isUserMenuOpen.value = !isUserMenuOpen.value
  }

  const router = useRouter();

  const logout = async () => {
    await authLogout()
    router.push('/')
  }
</script>

<style scoped>

    .app-layout {
        display: flex;
        min-height: 100vh;
        width: 100%;
        font-family: 'Inter', sans-serif;
        background-color: var(--color-main-bg);

  }


    .sidebar {
        width: 250px;
      height: 100vh;
      position: sticky;
      top: 0;
      flex-shrink: 0;
      background-color: #fcfcfc;
      box-shadow: 2px 0 10px rgba(0, 0, 0, 0.03);
      display: flex;
      flex-direction: column;
      padding: 1rem;
      transition: width 0.3s ease, padding 0.3s ease;

  }


    .sidebar-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.5rem 0.5rem 1rem 0.5rem;
      margin-bottom: 0.5rem;

  }

  .logo-text {
    font-size: 1.25rem;
    font-weight: 700;
    color: #000000;
    display: flex;
    align-items: center;
    font-family: 'Segoe UI'
  }

  .logo-icon {
    color: #46ba86;
    margin-right: 0.8rem;
    font-size: 1.5rem;
  }

  .toggle-btn {
    background: none;
    border: none;
    color: #4B5563;
    cursor: pointer;
    padding: 0.5rem;
    font-size: 1.2rem;
    border-radius: 6px;
    transition: background-color 0.2s;
  }

    .toggle-btn:hover {
      background-color: #E5E7EB;
    }

    .nav-menu {
      list-style: none;
      padding: 0;
      margin: 0;
      flex-grow: 1;

  }

    .nav-item {
      margin-bottom: 0.25rem;

  }

  .nav-link {
    display: flex;
    align-items: center;
    text-decoration: none;
    color: #4B5563;
    padding: 0.6rem 0.75rem;
    border-radius: 6px;
    transition: background-color 0.2s, color 0.2s;
  }

  .link-text {
    white-space: nowrap;
    overflow: hidden;
    flex-grow: 1;
    font-family: 'Segoe UI'
  }

  .nav-link:hover {
    background-color: #f4f4f5;
    color: #000000;
  }


    .nav-link.active {
      background-color: #f4f4f5;
      color: #46ba86;
      font-weight: 600;
      position: relative;

  }

      .nav-link.active::before {
        content: '';
        position: absolute;
        left: 0;
        top: 0;
        bottom: 0;
        width: 3px;
        background-color: #ECFDF5;
        border-radius: 6px 0 0 6px;
      }

    .nav-link i.me-2 {
      margin-right: 0.75rem;
      font-size: 1.1rem;

      color: #9CA3AF; 
      transition: color 0.2s;

  }

  .nav-link.active i.me-2 {
    color: #46ba86;
  }

    .sidebar-footer {
      padding-top: 1rem;
      border-top: 1px solid #4B5563;
      margin-top: auto;
      position: relative;
      padding-bottom: 0.5rem;

  }


  .user-profile-toggle {
    width: 100%;
    display: flex;
    align-items: center;
    justify-content: flex-start;
    text-align: left;
    padding: 0.6rem 0.75rem;
    border-radius: 6px;
    background: none;
    border: none;
    cursor: pointer;
    color: #4B5563;
    transition: background-color 0.2s;
  }

    .user-profile-toggle:hover,
    .user-profile-toggle.active {
      background-color: #E5E7EB;
    }

    .user-profile-toggle .user-name {
        margin-right: auto;
        font-weight: 500;

  }

    .user-profile-toggle .user-icon {
        font-size: 1.5rem;
        margin-right: 0.75rem;
        color: #9CA3AF; 

  }

    .user-profile-toggle .toggle-arrow {
        font-size: 0.75rem;
        transition: transform 0.2s ease;

  }

    .user-profile-toggle.active .toggle-arrow {
        transform: rotate(180deg);

  }

    .user-dropdown-menu {
      position: absolute;
      bottom: calc(100% + 10px);
      left: 10px;
      right: 10px;
      background-color: white;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
      border-radius: 8px;
      padding: 0.5rem 0;
      z-index: 10;

  }

  .dropdown-item {
    display: flex;
    align-items: center;
    width: 100%;
    padding: 0.6rem 1rem;
    color: #4B5563;
    text-decoration: none;
    background: none;
    border: none;
    cursor: pointer;
    transition: background-color 0.15s;
    font-size: 0.95rem;
  }

    .dropdown-item:hover {
      background-color: #E5E7EB;
    }

      .dropdown-item i.me-2 {
          margin-right: 0.75rem;
          font-size: 1.1rem;
          color: #9CA3AF; 

    }

    .logout-item {
      color: #DC2626;

  }

  .dropdown-divider {
    height: 1px;
    background-color: #E5E7EB;
    margin: 0.5rem 0;
  }


    .sidebar.closed {
      width: 70px;
      padding: 1rem 0.5rem;

  }

      .sidebar.closed .link-text,
      .sidebar.closed .user-name,
      .sidebar.closed .toggle-arrow {
          display: none;

    }

      .sidebar.closed .sidebar-header {
          justify-content: center;
          padding: 0;
          margin-bottom: 1.5rem;

    }

      .sidebar.closed .nav-link,
      .sidebar.closed .user-profile-toggle {
          justify-content: center;
          padding: 0.6rem;

    }

      .sidebar.closed .nav-link i.me-2,
      .sidebar.closed .user-profile-toggle .user-icon {
          margin-right: 0 !important;

  }

    .sidebar.closed .logo-text {
        display: none;

  }

    .sidebar.closed .user-dropdown-menu {
        left: 70px;
        bottom: 10px;
        width: 200px;

  }

    .main-content {
      flex-grow: 1;
      padding: 2rem;
      overflow-y: auto;

  }
</style>
