<template>
  <div class="container mt-5">
    <h2 class="mb-4">Users Management</h2>
    <br />

    <!-- SEARCH -->
    <div class="row mb-4">
      <div class="col-lg-5 col-md-6 col-sm-8">
        <div class="input-group shadow-sm">
          <span class="input-group-text bg-light border-end-0">
            <i class="fas fa-search text-muted"></i>
          </span>
          <input v-model="searchQuery"
                 @input="debouncedSearch"
                 type="text"
                 class="form-control border-start-0"
                 placeholder="Search..."
                 aria-label="Search users"
                 style="font-size: 0.95rem;" />
          <button v-if="searchQuery"
                  @click="clearSearch"
                  class="btn btn-outline-secondary border-start-0"
                  type="button">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- LOADING / ERROR -->
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" style="width: 4rem; height: 4rem;">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
    <div v-else-if="error" class="alert alert-danger">{{ error }}</div>

    <!-- Table -->
    <div v-else class="table-responsive">
      <table class="table table-striped table-bordered table-hover table-sm">
        <thead class="text-center">
          <tr>
            <th @click="sortBy('username')" class="cursor-pointer user-select-none">
              Username <i :class="sortIcon('username')"></i>
            </th>
            <th @click="sortBy('email')" class="cursor-pointer user-select-none">
              Email <i :class="sortIcon('email')"></i>
            </th>
            <th @click="sortBy('role')" class="cursor-pointer user-select-none">
              Role <i :class="sortIcon('role')"></i>
            </th>
            <th @click="sortBy('isActive')" class="cursor-pointer user-select-none">
              Status <i :class="sortIcon('isActive')"></i>
            </th>
            <th v-if="isAdmin"></th>
          </tr>
        </thead>
        <tbody class="text-center">
          <tr v-for="user in users" :key="user.id">
            <td><strong>{{ user.username }}</strong></td>
            <td>{{ user.email}}</td>
            <td>{{ getRoleName(user.role) }}</td>
            <td>
              <div class="form-check form-switch d-inline-block">
                <input class="form-check-input"
                       type="checkbox"
                       :id="'switch-' + user.username"
                       :checked="user.isActive"
                       @change="toggleUserStatus(user.username)"
                       :disabled="user.username === currentUsername" />
                <label :for="'switch-' + user.username" class="ms-2">
                  <span class="badge" :class="user.isActive ? 'bg-success' : 'bg-secondary'">
                    {{ user.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </label>
              </div>
            </td>
            <td v-if="isAdmin" class="text-center">
              <button v-if="user.username !== currentUsername"
                      @click="confirm(user.username)"
                      class="btn btn-sm text-danger"
                      title="Delete user">
                <i class="fas fa-trash"></i>
              </button>
            </td>
          </tr>
        </tbody>
      </table>

      <!-- EMPTY STATE -->
      <div v-if="users.length === 0" class="text-center py-5">
        <i class="fas fa-users-slash fa-5x text-muted mb-4 opacity-50"></i>
        <h4 class="text-muted">No user found</h4>
        <p v-if="searchQuery" class="text-muted">
          No results for "<strong>{{ searchQuery }}</strong>"
        </p>
      </div>
    </div>

    <!-- PAGINATION -->
    <div class="d-flex justify-content-between align-items-center mt-5 flex-wrap gap-3">
      <paginate v-if="totalPages > 1"
                v-model="currentPage"
                :page-count="totalPages"
                :page-range="5"
                :margin-pages="2"
                :click-handler="onPageChange"
                :prev-text="'Prev'"
                :next-text="'Next'"
                :container-class="'pagination pagination-lg'"
                :page-class="'page-item'"
                :page-link-class="'page-link'"
                :prev-class="'page-item'"
                :next-class="'page-item'"
                :active-class="'active'" />

      <div class="text-muted">
        Page {{ currentPage }} of {{ totalPages || 1 }}
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue'
  import Paginate from 'vuejs-paginate-next'
  import { useConfirmWarning, successDialog } from '@/Component/Modals/ConfirmationModal'
  import api from '@/Component/AuthServices/authAPI'
  import { useAuth } from '@/Component/AuthServices/Authentication'

  const API_BASE_URL = 'https://localhost:7037'

  // State
  const users = ref([])
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
  const loading = ref(true)
  const error = ref('')
  const { isAdmin, currentUsername } = useAuth()

  // Search & Sort
  const searchQuery = ref('')
  const sortByField = ref('username')
  const sortDirection = ref('asc')

  // Debounce search
  const debouncedSearch = debounce(() => {
    currentPage.value = 1
    loadUsers()
  }, 400)

  function clearSearch() {
    searchQuery.value = ''
    debouncedSearch()
  }

  // Sorting 
  function sortBy(field) {
    if (sortByField.value === field) {
      sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
    } else {
      sortByField.value = field
      sortDirection.value = 'asc'
    }
    currentPage.value = 1
    loadUsers()
  }

  function sortIcon(field) {
    if (sortByField.value !== field) return 'fas fa-sort ms-2 opacity-50'
    return sortDirection.value === 'asc'
      ? 'fas fa-sort-up ms-2 text-primary'
      : 'fas fa-sort-down ms-2 text-primary'
  }

  // Load users
  const loadUsers = async () => {
    loading.value = true
    error.value = ''

    try {
      const response = await api.get(`${API_BASE_URL}/api/Account`, {
        params: {
          page: currentPage.value,
          pageSize: perPage.value,
          search: searchQuery.value.trim(),
          sortBy: sortByField.value,
          sortDirection: sortDirection.value
        }
      })

      const data = response.data
      users.value = data.items || data
      totalItems.value = data.totalCount || data.length
      totalPages.value = Math.ceil(totalItems.value / perPage.value)
    } catch (err) {
      error.value = 'Failed to load users: ' + (err.response?.data || err.message)
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const onPageChange = (page) => {
    currentPage.value = page
    loadUsers()
  }

  const getRoleName = (role) => ({ 0: 'User', 1: 'Admin', 2: 'Auditor' }[role] || 'Unknown')

  const getRole = () => {
    const token = localStorage.getItem('token')
    if (!token) return null
    try {
      const payload = JSON.parse(atob(token.split('.')[1]))
      return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
    } catch { return null }
  }

  const DeleteUser = async (username) => {
    await api.delete(`${API_BASE_URL}/api/Account`, {
      params: { username }
    })
    await successDialog('User deleted successfully.')
    loadUsers()
  }

  const toggleUserStatus = async (username) => {
    try {
      await api.patch(`${API_BASE_URL}/api/Account/togglestatus?username=${username}`, null)
      const user = users.value.find(u => u.username === username)
      if (user) user.isActive = !user.isActive
    } catch {
      alert('Failed to update status')
      loadUsers()
    }
  }

  const confirm = async (username) => {
    const confirmed = await useConfirmWarning().confirmDialog(
      'Delete user?',
      `Delete <strong>${username}</strong>? This action cannot be undone.`,
      'Delete'
    )
    if (confirmed) await DeleteUser(username)
  }

  // Debounce helper
  function debounce(fn, delay) {
    let timeout
    return (...args) => {
      clearTimeout(timeout)
      timeout = setTimeout(() => fn(...args), delay)
    }
  }

  onMounted(() => {
    loadUsers()
  })
</script>

<style scoped>
  .cursor-pointer {
    cursor: pointer;
  }
  .table-dark th {
    background-color: #212529 !important;
  }
  .input-group:focus-within {
    box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
  }
</style>
