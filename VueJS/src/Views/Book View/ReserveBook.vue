<template>
  <div class="card p-4 mx-auto mt-5 shadow-lg" style="max-width: 500px;">
    <h2 class="text-center mb-1">Reserve Book </h2>
    <br />

    <form @submit.prevent="Reserve">
      <div class="mb-3">
        <label for="bookTitle" class="form-label">Book Title</label>
        <input v-model="bookTitle"
               id="bookTitle"
               placeholder="Enter book title"
               required
               class="form-control" />
      </div>

      <br />

      <div class="mb-3">
        <label for="clientName" class="form-label">Client Name</label>
        <input v-model="clientName"
               id="clientName"
               placeholder="Client name"
               required
               class="form-control" />
      </div>

      <br />

      <div class="mb-4">
        <label for="phoneNo" class="form-label">Phone Number</label>
        <input v-model="phoneNo"
               id="phoneNo"
               placeholder="Phone number"
               required
               type="tel"
               class="form-control" />
      </div>

      <div class="mb-4">
        <label for="address" class="form-label">Address</label>
        <input v-model="address"
               id="address"
               placeholder="Client address"
               required
               class="form-control" />
      </div>

      <button type="submit" class="btn btn-primary w-100 btn-lg shadow-sm">Reserve</button>

    </form>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import { useRoute, useRouter } from 'vue-router'
  import axios from 'axios'

  const route = useRoute()
  const router = useRouter()
  const client = ref({})
  const loading = ref(true)
  const error = ref('')

  const bookTitle = ref('')
  const clientName = ref('')
  const phoneNo = ref('')
  const address = ref('')

  const API_BASE_URL = 'https://localhost:7037'

  const getToken = () => {
    return localStorage.getItem('token');
  };

  onMounted(() => {
    const title = route.params.title
    if (!title) {
      error.value = 'Book title is required.'
    } else {
      bookTitle.value = decodeURIComponent(title)
    }
  })

  const Reserve = async () => {
    const token = getToken();
    if (!token) {
      error.value = 'You must be logged in to view client information.'
      loading.value = false
      return
    }

    const url = `${API_BASE_URL}/api/Reservations/Reserve`;
    const params = {
      title: bookTitle.value,
      clientname: clientName.value,
      phoneNo: phoneNo.value,
      address: address.value
    }

    const response = await axios.post(url, null, {
      params,
      headers: {
        Authorization: `Bearer ${token}`
      }
    })
    router.push('/app/books')
  }


</script>
