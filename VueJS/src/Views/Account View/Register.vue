<template>
  <div class="card p-4 mx-auto mt-5 shadow-lg" style="max-width: 400px;">
    <h1 class="text-center mb-1">Register</h1>

    <form @submit.prevent="Register">
      <div class="mb-3">
        <input v-model="username"
               placeholder="Username"
               required
               class="form-control" />
      </div>

      <div>
        <input v-model="email"
               placeholder="Email"
               required
               class="form-control" />
      </div>

      <div>
        <input v-model="password"
               placeholder="Password"
               type="password"
               required
               class="form-control" />
      </div>

      <button type="submit" class="btn btn-success w-100">Register</button>

    </form>

    <div class="card-footer text-center mt-3">
      <router-link to="/" class="text-decoration-none">
        Already have an account? Login
      </router-link>
    </div>


  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import axios from 'axios'

  const username = ref('')
  const password = ref('')
  const email = ref('')
  const error = ref('')

  const Register = async () => {
    await axios.post('/api/Account/Register', {
      Username: username.value,
      Password: password.value,
      Email: email.value
    })
  }
</script>
