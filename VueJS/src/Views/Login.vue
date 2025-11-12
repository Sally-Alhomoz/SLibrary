<template>
  <div class="card p-4 mx-auto mt-5 shadow-lg" style="max-width: 400px;">
    <h1 class="text-center mb-1">SLibrary</h1>
    <h2 class="text-center h4 mb-4">Login</h2>

    <form @submit.prevent="login">
      <div class="mb-3">
        <input v-model="username"
               placeholder="Username"
               required
               class="form-control" />
      </div>

      <div class="mb-3">
        <input v-model="password" type="password" placeholder="Password" required class="form-control" />
      </div>

      <button type="submit" class="btn btn-success w-100">Login</button>

    </form>

    <div class="card-footer text-center">
      <p class="mb-0 text-muted">
        Create an account?
        <router-link :to="{name:'Register'}" class="text-decoration-none">Register</router-link>
      </p>
    </div>

    <p v-if="error" class="alert alert-danger mt-3 text-center py-2">
      {{ error }}
    </p>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import axios from 'axios'

  const username = ref('')
  const password = ref('')
  const error = ref('')
  const router = useRouter()

  const login = async () => {
    try {
      const res = await axios.post('/api/Account/Login', {
        Username: username.value,
        Password: password.value
      })
      localStorage.setItem('token', res.data.token)
      router.push('/app')
    } catch (err) {
      error.value = 'Invalid username or password'
    }
  }
</script>


<style scoped>
 /* .login {
    max-width: 400px;
    margin: 100px auto;
    padding: 30px;
    border: 1px solid #ddd;
    border-radius: 12px;
    text-align: center;
    background: #f8f9fa;
    font-family: 'Segoe UI', Tahoma, sans-serif;
  }

  input, button {
    width: 100%;
    padding: 12px;
    margin: 8px 0;
    border: 1px solid #ccc;
    border-radius: 8px;
    font-size: 16px;
  }

  button {
    background: #28a745;
    color: white;
    font-weight: bold;
    cursor: pointer;
  }

    button:hover {
      background: #218838;
    }

  .error {
    color: #dc3545;
    margin-top: 10px;
  }*/
</style>
