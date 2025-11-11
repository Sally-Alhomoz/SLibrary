<template>
  <div class="card p-4 mx-auto mt-5 shadow-lg" style="max-width: 400px;">
    <h2 class="text-center mb-1">Add New Book</h2>
    <br />

    <form @submit.prevent="Add">
      <div class="mb-3">
        <input v-model="bookTitle"
               placeholder="Title"
               required
               class="form-control" />
      </div>

      <div class="mb-3">
        <input v-model="author" placeholder="Author" required class="form-control" />
      </div>

      <button type="submit" class="btn btn-primary w-100">Add</button>

    </form>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import axios from 'axios'


  const API_BASE_URL = 'https://localhost:7037';

  const bookTitle = ref('')
  const author = ref('')

  const router = useRouter()

  const Add = async () => {
    axios.post(`${API_BASE_URL}/api/Books`, {
      Title: bookTitle.value,
      Author: author.value
    })
    alert('Book added!')
    router.push('/app/books')
  }
</script>


<!--<template>
  <div class="card p-4 mx-auto mt-5 shadow-lg" style="max-width: 400px;">
    <h2 class="text-center mb-4">Add New Book</h2>

    <form @submit.prevent="handleSubmit">
      <div class="mb-3">
        <input v-model.trim="bookTitle" placeholder="Title" required class="form-control" />
      </div>
      <div class="mb-3">
        <input v-model.trim="author" placeholder="Author" required class="form-control" />
      </div>

      <button type="submit" class="btn btn-primary w-100" :disabled="loading">
        <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
        {{ loading ? 'Adding...' : 'Add Book' }}
      </button>
    </form>-->

    <!-- Error -->
    <!--<div v-if="error" class="alert alert-danger mt-3 py-2 text-center">
      {{ error }}
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import { useConfirm } from '@/Component/ConfirmationModal.js'
  import axios from 'axios'
  import Swal from 'sweetalert2'

  const API_BASE_URL = 'https://localhost:7037'
  const bookTitle = ref('')
  const author = ref('')
  const loading = ref(false)
  const error = ref('')
  const router = useRouter()
  const { confirm } = useConfirm()

  const handleSubmit = async () => {
    // 1. Show confirmation with actual data
    const confirmed = await confirm({
      title: 'Confirm Adding Book',
      html: `
      <p class="mb-1"><strong>Title:</strong> ${bookTitle.value}</p>
      <p><strong>Author:</strong> ${author.value}</p>
    `,
      confirmButtonText: 'Add',
      cancelButtonText: 'Cancel'
    })

    if (!confirmed) return

    // 2. Send request with await + error handling
    loading.value = true
    error.value = ''

    try {
      await axios.post(`${API_BASE_URL}/api/Books`, {
        Title: bookTitle.value,
        Author: author.value
      })

      // Success
      await Swal.fire({
        icon: 'success',
        title: 'Book added!',
        timer: 1200,
        showConfirmButton: false
      })

      // Reset + redirect
      bookTitle.value = ''
      author.value = ''
      router.push('/app/books')

    } catch (err) {
      error.value = err.response?.data?.title || 'Failed to add book.'
    } finally {
      loading.value = false
    }
  }
</script>-->
