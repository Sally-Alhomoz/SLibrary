<template>
  <h2 class="text-center mb-4">Clinet Information</h2>
  <div class="row justify-content-center">
    <div class="col-md-6 col-lg-4">
      <div class="card mb-4 shadow-sm">
        <div class="card-body">

          <div class="mb-2">
            <strong>Clinet name:</strong>
            <p class="form-control-plaintext border p-2 rounded bg-light">{{client.fullName}}</p>
          </div>

          <br />

          <div>
            <strong>Phone Number:</strong>
            <p class="form-control-plaintext border p-2 rounded bg-light mb-0">{{client.phoneNo}}</p>
          </div>

          <br />

          <div>
            <strong>Address:</strong>
            <p class="form-control-plaintext border p-2 rounded bg-light mb-0">{{client.address}}</p>
          </div>
        </div>
      </div>
    </div>
  </div>

</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import { useRoute } from 'vue-router'
  import axios from 'axios'

  const route = useRoute()
  const client = ref({})
  const loading = ref(true)
  const error = ref('')

  const API_BASE_URL = 'https://localhost:7037'

  const getToken = () => {
    return localStorage.getItem('token');
  };

  const getClientInfo = async () => {
    const name = route.params.name
    if (!name) {
      error.value = 'Client name is required.'
      loading.value = false
      return
    }

    const token = getToken();
    if (!token) {
      error.value = 'Access Denied (401): You must be logged in to view client information.'
      loading.value = false
      return
    }

    const response = await axios.get(`${API_BASE_URL}/api/Clients/${encodeURIComponent(name)}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    client.value = response.data

  }

  onMounted(() => {
    getClientInfo()
  })
</script>
