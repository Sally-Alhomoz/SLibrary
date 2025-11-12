<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <h2 class="text-center mb-5 text-dark fw-bold">Change Password</h2>

        <div class="card shadow-lg border-0 rounded-xl">
          <div class="card-body p-5">
            <form @submit.prevent="changePassword">
              <div class="mb-3">
                <input v-model="oldPassword"
                       placeholder="Old Password"
                       type="password"
                       required
                       class="form-control" />
              </div>

              <div class="mb-3">
                <input v-model="newPassword"
                       placeholder="New Password"
                       type="password"
                       required
                       class="form-control" />
              </div>

              <div class="mb-3">
                <input v-model="confirmPassword"
                       placeholder="Confirm Password"
                       type="password"
                       required
                       class="form-control" />
              </div>

              <button type="submit" class="btn btn-primary w-100">Change</button>

            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import axios from 'axios'

  const oldPassword = ref('')
  const newPassword = ref('')
  const confirmPassword = ref('')

  const router = useRouter()

  const API_BASE_URL = 'https://localhost:7037';

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

  const getToken = () => {
    return localStorage.getItem('token');
  };

  const changePassword = async () => {
    const token = getToken();
    if (!token) {
      error.value = 'You must be logged in to view client information.'
      return
    }

    const Params = {
      OldPassword: oldPassword.value,
      NewPassword: newPassword.value,
      ConfirmPassword: confirmPassword.value
    };

    await axios.patch(`${API_BASE_URL}/api/Account`,
      Params, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    logout()
  }
</script>
