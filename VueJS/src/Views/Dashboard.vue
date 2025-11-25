<template>
  <div class="dashboard-container">
    <div class="welcome-section">
      <h1 class="welcome-title">Welcome back, {{ currentUsername }}!</h1>
    </div>

    <div class="stats-grid">
      <div class="stat-card available">
        <div class="icon-circle">
          <i class="fas fa-book-open"></i>
        </div>
        <div class="stat-info">
          <h3>{{ totalAvailableBooks }}</h3>
          <p class="text-muted">Available Books</p>
        </div>
        <div class="stat-bg"></div>
      </div>

      <div class="stat-card reservations">
        <div class="icon-circle">
          <i class="fas fa-history"></i>
        </div>
        <div class="stat-info">
          <h3>{{ totalUserReservations }}</h3>
          <p class="text-muted">Total Reservations</p>
        </div>
        <div class="stat-bg"></div>
      </div>

      <div class="stat-card active">
        <div class="icon-circle">
          <i class="fas fa-calendar-check"></i>
        </div>
        <div class="stat-info">
          <h3>{{ activeUserReservations }}</h3>
          <p class="text-muted">Active Now</p>
        </div>
        <div class="stat-bg"></div>
        <div class="action-btn">
          <router-link to="/app/reservations">View All →</router-link>
        </div>
      </div>
    </div>

    <div class="actions-grid">
      <router-link to="/app/books" class="action-card">
        <i class="fas fa-search"></i>
        <h4>Browse Books</h4>
        <p class="text-muted">Explore the collection</p>
      </router-link>

      <router-link to="/app/reservations" class="action-card">
        <i class="fas fa-bookmark"></i>
        <h4>Reservations</h4>
        <p class="text-muted">Track your books</p>
      </router-link>

      <router-link to="/app/profile" class="action-card">
        <i class="fas fa-user-circle"></i>
        <h4>My Profile</h4>
        <p class="text-muted">Account settings</p>
      </router-link>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue'
  import { useAuth } from '@/Component/AuthServices/Authentication'
  import api from '@/Component/AuthServices/authAPI'

  const { currentUsername } = useAuth()
  const API_BASE_URL = 'https://localhost:7037'

  const totalAvailableBooks = ref(0)
  const totalUserReservations = ref(0)
  const activeUserReservations = ref(0)

  const avaialbeBookCount = async () => {
    try {
      const response = await api.get(`${API_BASE_URL}/api/Books/AvailableCount`);

      if (response.data && typeof response.data.availableCount === 'number') {
        totalAvailableBooks.value = response.data.availableCount;
      } else {
        totalAvailableBooks.value = 0;
      }

    } catch (err) {
      console.error('Failed to load available book count:', err);
      totalAvailableBooks.value = 0;
    }
  }

  const userReservations = async () => {
    try {
      let response = await api.get(`${API_BASE_URL}/api/Reservations/GetUserReservation`);
      if (response.data && typeof response.data.totalReservations === 'number') {
        totalUserReservations.value = response.data.totalReservations;
      } else {
        totalUserReservations.value = 0;
      }
    } catch (err) {
      console.error('Error fetching total reservations:', err);
      totalUserReservations.value = 0;
    }
  }

  const userActiveReservations = async () => {
    try {
      let response = await api.get(`${API_BASE_URL}/api/Reservations/GetActiveReservation`);
      if (response.data && typeof response.data.activeReservations === 'number') {
        activeUserReservations.value = response.data.activeReservations;
      } else {
        activeUserReservations.value = 0;
      }
    } catch (err) {
      console.error('Error fetching active reservations:', err);
      activeUserReservations.value = 0;
    }
  }

  onMounted(() => {
    avaialbeBookCount()
    userReservations()
    userActiveReservations()
  })

</script>

<style scoped>
  .dashboard-container {
    min-height: 100vh;
    background: #ffff;
    padding: 3rem 2rem;
    font-family: 'Segoe UI';
  }

  .welcome-section {
    text-align: center;
    margin-bottom: 5rem;
  }

  .welcome-title {
    font-size: 3.5rem;
    font-weight: 800;
    color: #46ba86;
    margin: 0;
    letter-spacing: -1px;
  }


  .stats-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 2rem;
    margin-bottom: 4rem;
  }

  .stat-card {
    position: relative;
    background: white;
    padding: 0.8rem 1.8rem;
    border-radius: 50px;
    box-shadow: 0 10px 30px rgba(0,0,0,0.08);
    overflow: hidden;
    transition: all 0.4s ease;
    border: 1px solid rgba(70, 186, 134, 0.1);
  }
    .stat-card:hover {
      transform: none;
      box-shadow: 0 10px 30px rgba(0,0,0,0.08);
    }

    .stat-card .icon-circle {
      width: 70px;
      height: 70px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 2.5rem;
      color: white;
      margin-bottom: 1.5rem;
      z-index: 2;
      position: relative;
    }

    .stat-card.available .icon-circle {
      background: #059669;
    }

    .stat-card.reservations .icon-circle {
      background: #059669;
    }

    .stat-card.active .icon-circle {
      background: #059669;
    }

  .stat-info h3 {
    font-size: 3rem;
    font-weight: 800;
    color: #1f2937;
    margin: 0;
  }



  .stat-bg {
    position: absolute;
    top: -50%;
    right: -50%;
    width: 200%;
    height: 200%;
    border-radius: 50%;
    background: #46ba86;
    opacity: 0.05;
    z-index: 1;
  }

  .action-btn {
    margin-top: -2.2rem; 
    margin-bottom: 1rem;
    text-align: right;
  }

    .action-btn a {
      background: #46ba86;
      color: white;
      padding: 0.6rem 1.5rem;
      border-radius: 50px;
      text-decoration: none;
      font-weight: 600;
      font-size: 1rem;
      box-shadow: 0 8px 20px rgba(70, 186, 134, 0.3);
      transition: all 0.3s;
      display: inline-block;
      position: relative;
      z-index: 10;
    }

      .action-btn a:hover {
        transform: translateY(-3px);
        box-shadow: 0 12px 25px rgba(70, 186, 134, 0.4);
      }

  .actions-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 2rem;
  }

  .action-card {
    background: white;
    padding: 2.5rem 2rem;
    border-radius: 20px;
    text-align: center;
    box-shadow: 0 8px 25px rgba(0,0,0,0.07);
    transition: all 0.4s ease;
    text-decoration: none;
    color: inherit;
    border: 1px solid rgba(70, 186, 134, 0.1);
  }

    .action-card:hover {
      transform: translateY(-10px);
      box-shadow: 0 20px 40px rgba(70, 186, 134, 0.15);
    }

    .action-card i {
      font-size: 3rem;
      margin-bottom: 1rem;
      color: #94a3b8;
      transition: color 0.3s;
    }

    .action-card:hover i {
      color: #46ba86;
    }

    .action-card h4 {
      font-size: 1.4rem;
      font-weight: 700;
      color: #1f2937;
      margin: 0 0 0.5rem;
    }

</style>
