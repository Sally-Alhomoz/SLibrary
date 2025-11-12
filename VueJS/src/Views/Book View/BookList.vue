<template>
  <div class="container mt-5">
    <h2 class="mb-4">Book List</h2>

    <div v-if="canAdd" class="mb-3 d-flex justify-content-end">
      <router-link to="/app/books/add" class="btn btn-primary">
        <i class="bi bi-plus-circle"></i> Add Book
      </router-link>
    </div>

    <table class="table table-striped table-bordered">
      <thead class="text-center">
        <tr>
          <th>Title</th>
          <th>Author</th>
          <th>Reserved</th>
          <th>Available</th>
          <th></th>
        </tr>
      </thead>
      <tbody class="text-center">
        <tr v-for="book in books" :key="book.id">
          <td>{{ book.title }}</td>
          <td>{{ book.author }}</td>
          <td>{{book.reserved}}</td>
          <td>{{ book.available }}</td>
          <td>
            <div class="d-flex justify-content-center gap-2">
              <button v-if="isAdmin" class="btn btn-sm text-danger"
                      title="Delete Book"
                      @click="deleteBook(book.id)">
                <i class="fas fa-trash"></i>
              </button>


              <router-link :to="{ name: 'ReserveBook', params: { title: book.title } }"
                           class="btn btn-sm text-primary"
                           title="Reserve Book"
                           :disabled="book.available == 0">
                <i class="fas fa-bookmark"></i>
              </router-link>
            </div>
          </td>
        </tr>
        <tr v-if="books.length === 0">
          <td colspan="5" class="text-center text-muted">
            {{ loading ? 'Loading books...' : error ? error : 'No books found.' }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>



<script setup>
  import { ref, onMounted, computed } from 'vue'
  import axios from 'axios'

  const books = ref([])
  const error = ref('')
  const loading = ref(true)


  const API_BASE_URL = 'https://localhost:7037';

  const read = async () => {
    loading.value = true
    error.value = ''

    const response = await axios.get(`${API_BASE_URL}/api/Books`)
    books.value = response.data

    loading.value = false
  }

  const deleteBook = async (id) => {
    await axios.delete(`${API_BASE_URL}/api/Books?id=${encodeURIComponent(id)}`)

    await read()

  }

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

  const isAuditor = computed(() => {
    const role = getRole()
    return role === 'Auditor'
  })

  const canAdd = computed(() => isAdmin.value || isAuditor.value)




  onMounted(() => {
    read()
  })
</script>
