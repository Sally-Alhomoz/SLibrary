<template>
  <div class="container">
    <div class="left">
      <div class="form-wrapper">
        <h1>SLibrary</h1>
        <p class="subtitle">Welcome back</p>

        <form @submit.prevent="Login">
          <input v-model="username" type="text" placeholder="Username" required />
          <input v-model="password" type="password" placeholder="Password" required />
          <button type="submit">Sign In</button>
        </form>

        <p class="register-link">
          Don't have an account?
          <router-link to="/register">Register here</router-link>
        </p>

        <div v-if="error" class="error">{{ error }}</div>
      </div>
    </div>

    <div class="right">
      <div class="overlay"></div>
      <img src="/images/library-book.jpg" alt="Library" class="bg-image" />
      <div class="text">
        <h2>Access Thousands of Books</h2>

      </div>
    </div>
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

  const Login = async () => {
    try {
      const res = await axios.post('/api/Account/Login', {
        Username: username.value,
        Password: password.value
      })
      localStorage.setItem('token', res.data.token)
      router.push('/app/dashboard')
    } catch (err) {
      error.value = 'Invalid username or password'
    }
  }
</script>

<style scoped>
  .container {
    display: flex;
    min-height: 100vh;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  }

  .left {
    flex: 1;
    background: #ffffff;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 2rem;
  }

  .form-wrapper {
    width: 100%;
    max-width: 400px;
    text-align: center;
  }

    .form-wrapper h1 {
      font-size: 4rem;
      font-weight: bold;
      color: #46ba86;
      -webkit-background-clip: unset;
      -webkit-text-fill-color: unset;
      margin-bottom: 0.5rem;
    }

  .subtitle {
    color: #64748b;
    font-size: 1.2rem;
    margin-bottom: 2rem;
  }

  input {
    width: 100%;
    padding: 1rem 1.5rem;
    margin: 0.8rem 0;
    border: 1px solid #cbd5e1;
    border-radius: 16px;
    font-size: 1.1rem;
    background: white;
  }

    input:focus {
      outline: none;
      border-color: #46ba86;
      box-shadow: 0 0 0 4px rgba(70, 186, 134, 0.2);
    }

  button {
    width: 100%;
    padding: 1rem;
    margin-top: 1rem;
    background: #46ba86;
    color: white;
    border: none;
    border-radius: 16px;
    font-size: 1.2rem;
    font-weight: bold;
    cursor: pointer;
    transition: all 0.3s;
  }

    button:hover {
      transform: translateY(-3px);
      box-shadow: 0 10px 20px rgba(70, 186, 134, 0.4);
    }

  .register-link {
    margin-top: 2rem;
    color: #64748b;
  }

    .register-link a {
      color: #19a5f6;
      text-decoration: none;
      font-weight: 600;
    }

      .register-link a:hover {
        text-decoration: underline;
      }

  .error {
    margin-top: 1rem;
    padding: 1rem;
    background: #fee2e2;
    color: #dc2626;
    border-radius: 12px;
    border: 1px solid #fecaca;
  }

  .right {
    flex: 1;
    position: relative;
    display: none; 
    overflow: hidden;
  }

  .bg-image {
    position: absolute;
    inset: 0;
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  .overlay {
    position: absolute;
    inset: 0;
    background: rgba(0, 0, 0, 0.5);
    z-index: 1;
  }

  .text {
    position: relative;
    z-index: 2;
    color: white;
    text-align: center;
    padding: 4rem;
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: center;
  }

    .text h2 {
      font-size: 4rem;
      font-weight: bold;
      margin-bottom: 1rem;
    }

    .text p {
      font-size: 1.5rem;
      opacity: 0.9;
    }

  @media (min-width: 1024px) {
    .right {
      display: block;
    }

    .container {
      flex-direction: row;
    }
  }

  @media (max-width: 1023px) {
    .container {
      flex-direction: column;
    }

    .left {
      min-height: 100vh;
    }
  }
</style>
