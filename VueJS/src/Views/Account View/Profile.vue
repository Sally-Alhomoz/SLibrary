<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <h2 class="text-center mb-5 text-primary fw-bold">User Profile</h2>

        <div class="card shadow-lg border-0 rounded-xl">
          <div class="card-body p-5">
            <div class="text-center mb-4">

              <i class="fas fa-user-circle fa-4x text-secondary mb-3"></i>
              <h4 class="card-title fw-bold text-dark"></h4>
            </div>

            <div class="list-group-item d-flex justify-content-between align-items-center">
              <strong class="text-dark">Account Info</strong>
              <router-link to="/app/profile/editaccount" class="text-secondary" title="Edit Account">
                <i class="fa fa-pen"></i>
              </router-link>
            </div>
            <br />

            <ul class="list-group list-group-flush">
              <li class="list-group-item d-flex justify-content-between align-items-center">
                <strong class="text-dark">Username:</strong>
                <span class="text-dark">{{username}}</span>
              </li>
              <li class="list-group-item d-flex justify-content-between align-items-center">
                <strong class="text-dark">Email:</strong>
                <span class="text-dark">{{email}}</span>
              </li>
            </ul>

            <div class="mt-4 pt-3 border-top text-center">
              <router-link to="/app/profile/changepassword" class="btn btn-outline-secondary me-2">
                <i class="fas fa-cog me-2"></i> Change Password
              </router-link>
            </div>

          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue'
  import axios from 'axios'

  const API_BASE_URL = 'https://localhost:7037';

  const username = ref('')
  const email = ref('')

  const getToken = () => {
    return localStorage.getItem('token');
  };

  const getInfo = async () => {

    const token = getToken();
    if (!token) {
      error.value = 'You must be logged in to view client information.'
      loading.value = false
      return
    }


    const response = await axios.get(`${API_BASE_URL}/api/Account/GetAccountInfo`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    username.value = response.data.username
    email.value = response.data.email
  }

  onMounted(() => {
    getInfo()
  })
</script>
