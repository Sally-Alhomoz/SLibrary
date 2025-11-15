<template>
  <div class="card p-4 mx-auto mt-5 shadow-lg" style="max-width: 400px;">
    <h2 class="text-center mb-1">Add New Book</h2>
    <br />

    <form @submit.prevent="confirm">
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
  import Swal from 'sweetalert2'


  const API_BASE_URL = 'https://localhost:7037';

  const bookTitle = ref('')
  const author = ref('')

  const router = useRouter()


  const confirm = async () => {
    const result = await Swal.fire({
      title: 'Add Book?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Add',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: 'gray',
      showCloseButton: true
    })

    if (result.isConfirmed) {
      await Add()
    }
  }

  const Add = async () => {
    axios.post(`${API_BASE_URL}/api/Books`, {
      Title: bookTitle.value,
      Author: author.value
    })

    await Swal.fire({
      title: 'Success!',
      text: 'Book Added',
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    })

    router.push('/app/books')
  }
</script>
