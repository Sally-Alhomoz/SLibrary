<template>
  <div class="container mt-5">
    <h2 class="mb-4">Users</h2>

    <table class="table table-striped table-bordered">
      <thead class="text-center">
        <tr>
          <th>Username</th>
          <th>Role</th>
          <th>Status</th>
          <th></th>
        </tr>
      </thead>
      <tbody class="text-center">
        <tr v-for="user in users" key="user.id">
          <td>{{user.username}}</td>
          <td>{{getRoleName(user.role)}}</td>
          <td>
            <span v-if="user.isActive">
              <span class="badge bg-success">Active</span>
            </span>
            <span v-else>
              <span class="badge bg-danger">Inactive</span>
            </span>
          </td>
          <td>

          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import axios from 'axios'

  const users = ref([])
  const error = ref('')
  const loading = ref(true)

  const API_BASE_URL = 'https://localhost:7037';


  const Read = async () => {
    loading.value = true
    error.value = ''

    const response = await axios.get(`${API_BASE_URL}/api/Account`)
    users.value = response.data

    loading.value = false
  }

  const getRoleName = (roleId) => {
    switch (roleId) {
      case 0: 
        return 'User';
      case 1: 
        return 'Admin';
      case 2: 
        return 'Editor';
      default:
        return 'Unknown';
    }
  };

  onMounted(() => {
    Read()
  })
</script>
