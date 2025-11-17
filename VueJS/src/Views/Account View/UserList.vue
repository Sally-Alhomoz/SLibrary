<template>
  <div class="container mt-5">
    <h2 class="mb-4">Users</h2>

    <table class="table table-striped table-bordered">
      <thead class="text-center">
        <tr>
          <th>Username</th>
          <th>Role</th>
          <th>Status</th>
          <th v-if="isAdmin"></th>
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
          <td v-if="isAdmin">
            <button v-if="user.username !== currentUsername" class="btn btn-sm text-danger"
                    title="Delete User"
                    @click="confirm(user.username)">
              <i class="fas fa-trash"></i>

            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>


<script setup>
  import { ref, onMounted, computed } from 'vue'
  import axios from 'axios'

  const users = ref([])
  const error = ref('')
  const loading = ref(true)

  const API_BASE_URL = 'https://localhost:7037';

  const confirm = async (username) => {
    const result = await Swal.fire({
      title: 'Delete user?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#D33',
      cancelButtonColor: 'gray',
      showCloseButton: true
    })

    if (result.isConfirmed) {
      await DeleteUser(username)
    }
  }

  const getCurrentUsername = () => {
    const token = localStorage.getItem('token')
    if (!token) {
      return null
    }
    const payload = JSON.parse(atob(token.split('.')[1]))
    return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
  }

  const currentUsername = computed(() => {
    return getCurrentUsername();
  });


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
        return 'Auditor'
    }
  };

  const DeleteUser = async (username) => {
    const token = localStorage.getItem('token')
    if (!token) {
      alert('You must be logged in.')
      return
    }

    await axios.delete(`${API_BASE_URL}/api/Account`, {
      params: { username },
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    await Swal.fire({
      title: 'Success!',
      text: 'User deleted.',
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    })

    await Read() 
   
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

  onMounted(() => {
    Read()
  })
</script>
