<template>
  <div class="container mt-5">
    <h2 class="mb-4">Reservations List</h2>
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
                 aria-label="Search reservations"
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

    <div v-else class="table-responsive">
      <table class="table table-striped table-bordered table-hover table-sm">
        <thead class="text-center">
          <tr>
            <th @click="sortBy('reservedBy')" class="cursor-pointer user-select-none">
              Reserved By <i :class="sortIcon('reservedBy')"></i>
            </th>
            <th @click="sortBy('clientName')" class="cursor-pointer user-select-none">
              Client Name <i :class="sortIcon('clientName')"></i>
            </th>
            <th @click="sortBy('bookTitle')" class="cursor-pointer user-select-none">
              Book Title <i :class="sortIcon('bookTitle')"></i>
            </th>
            <th @click="sortBy('reservedDate')" class="cursor-pointer user-select-none">
              Reserve Date <i :class="sortIcon('reservedDate')"></i>
            </th>
            <th @click="sortBy('releaseDate')" class="cursor-pointer user-select-none">
              Release Date <i :class="sortIcon('releaseDate')"></i>
            </th>
            <th>Reservation Period</th>
          </tr>
        </thead>
        <tbody class="text-center">
          <tr v-for="res in reservations" :key="res.id">
            <td>{{ res.reservedBy }}</td>
            <td>
              <button class="btn btn-sm text-primary"
                      title="client info"
                      @click="clientInfoModal(res)">
                <i class="fa fa-eye"></i>
              </button>
              {{ res.clientName }}
            </td>
            <td>{{ res.bookTitle }}</td>
            <td>{{ formatDate(res.reservedDate) }}</td>
            <td>
              <span v-if="res.releaseDate">
                {{formatDate(res.releaseDate)}}
              </span>

              <button v-else
                      type="button"
                      class="btn btn-link text-danger p-0 action-button"
                      title="Release Book"
                      @click="confirmRelease(res)">
                <i class="fas fa-arrow-alt-circle-up me-1"></i> Release
              </button>
            </td>
            <td>
              <span v-if="res.releaseDate">
                <span class="text-dark">{{ formatPeriod(res.reservationPeriod) }}</span>
              </span>
              <span v-else>
                <span class="badge bg-success">Ongoing</span>
              </span>
            </td>
          </tr>
        </tbody>
      </table>


      <!-- EMPTY STATE -->
      <div v-if="reservations.length === 0" class="text-center py-5">
        <i class="fas fa-calendar-times fa-5x text-muted mb-4 opacity-50"></i>
        <h4 class="text-muted">No reservations found</h4>
        <p v-if="searchQuery" class="text-muted">
          No results for "<strong>{{ searchQuery }}</strong>"
        </p>
      </div>
    </div>

    <!-- PAGINATION -->
    <div class="mt-5">
      <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
        <div class="text-muted small">
          Showing {{ ((currentPage - 1) * perPage) + 1 }}–
          {{ Math.min(currentPage * perPage, totalItems) }} of {{ totalItems }} reservations
        </div>

        <div></div>
      </div>

      <div class="d-flex justify-content-start mt-3">
        <paginate v-if="totalPages > 1"
                  v-model="currentPage"
                  :page-count="totalPages"
                  :page-range="5"
                  :margin-pages="2"
                  :click-handler="onPageChange"
                  :prev-text="'Prev'"
                  :next-text="'Next'"
                  :container-class="'pagination pagination-md'"
                  :page-class="'page-item'"
                  :page-link-class="'page-link'"
                  :prev-class="'page-item'"
                  :next-class="'page-item'"
                  :active-class="'active'"
                  class="m-0" />
      </div>

      <div class="text-muted small mt-2">
        Page {{ currentPage }} of {{ totalPages || 1 }}
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, computed, onMounted } from 'vue'
  import axios from 'axios'
  import Paginate from 'vuejs-paginate-next'
  import { useConfirmWarning, successDialog, errorDialog } from '@/Component/Modals/ConfirmationModal'

  //State
  const reservations = ref([])
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
  const loading = ref(true)
  const error = ref('')

  // Search & Sort
  const searchQuery = ref('')
  const sortByField = ref('reservedDate')
  const sortDirection = ref('asc')

  const confirmDialog = useConfirmWarning().confirmDialog
  const API_BASE_URL = 'https://localhost:7037'

  const getToken = () => {
    return localStorage.getItem('token');
  };

  // Debounce search
  const debouncedSearch = debounce(() => {
    currentPage.value = 1
    loadReservations()            
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
    loadReservations()
  }

  function sortIcon(field) {
    if (sortByField.value !== field) return 'fas fa-sort ms-2 opacity-50'
    return sortDirection.value === 'asc'
      ? 'fas fa-sort-up ms-2 text-primary'
      : 'fas fa-sort-down ms-2 text-primary'
  }

  //Load
  const loadReservations = async () => {
    loading.value = true
    error.value = ''

    try {
      const token = localStorage.getItem('token')
      const response = await axios.get(`${API_BASE_URL}/api/Reservations`, {
        params: {
          page: currentPage.value,
          pageSize: perPage.value,
          search: searchQuery.value.trim(),
          sortBy: sortByField.value,
          sortDirection: sortDirection.value
        },
        headers: token ? { Authorization: `Bearer ${token}` } : {}
      })

      const data = response.data
      reservations.value = data.items || data
      totalItems.value = data.totalCount || data.length
      totalPages.value = Math.ceil(totalItems.value / perPage.value)
    } catch (err) {
      error.value = 'Failed to load reservations: ' + (err.response?.data || err.message)
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const onPageChange = (page) => {
    currentPage.value = page
    loadReservations()
  }

  // Debounce helper
  function debounce(fn, delay) {
    let timeout
    return (...args) => {
      clearTimeout(timeout)
      timeout = setTimeout(() => fn(...args), delay)
    }
  }

  const confirmRelease = async (reservation) => {
    const confirmed = await confirmDialog(
      'Release Book',
      `Do you want to release <strong>${reservation.bookTitle}</strong>?`,
      'Release'
    )
    if (confirmed) {
      await ReleaseBook(reservation)
    }
  }

  const ReleaseBook = async (reservation) => {
    const title = reservation.bookTitle
    const clientName = reservation.clientName
    const token = getToken();

    if (!token) {
      await errorDialog('Authentication token missing.', 'Authorization Failed 🔒');
      return;
    }

    try {
      const apiUrl = `${API_BASE_URL}/api/Reservations/Release?title=${encodeURIComponent(title)}&clientname=${encodeURIComponent(clientName)}`
      await axios.delete(apiUrl, {
        headers: { Authorization: `Bearer ${token}` }
      })
      await successDialog(`${reservation.bookTitle} has been released successfully!`)
    } catch (err) {
      let errorMessage = 'An unexpected error occurred during release.';
      if (err.response && err.response.data && err.response.data.title) {
        errorMessage = err.response.data.title;
      } else if (err.message) {
        errorMessage = err.message;
      }
      await errorDialog(errorMessage, 'Release Failed 🛑');
    }

    loadReservations()
  }

  const formatDate = (dateString) => {
    if (!dateString) return '-'
    const date = new Date(dateString)
    if (isNaN(date)) return dateString
    return date.toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    })
  }

  const formatPeriod = (timeSpanString) => {
    if (!timeSpanString) return 'N/A';

    const match = timeSpanString.match(/(?:(\d+)\.)?(\d{2}):(\d{2}):(\d{2})/)

    if (!match) {
      return 'Invalid Period';
    }

    const days = parseInt(match[1] || 0, 10);
    const hours = parseInt(match[2], 10);
    const minutes = parseInt(match[3], 10);
    const seconds = parseInt(match[4], 10);

    const parts = [];

    if (days > 0) {
      parts.push(`${days} dy`);
    }
    if (hours > 0) {
      parts.push(`${hours} hr`);
    }
    if (minutes > 0) {
      parts.push(`${minutes} min`);
    }

    if (parts.length === 0) {
      if (seconds > 0) {
        return `${seconds} sec`;
      }
      return '0 sec';
    }

    return parts.join(', ');
  }

  const clientInfoModal = async (reservation) => {
    const name = reservation.clientName
    let modalHtml = ''
    const clientData = await getClientInfo(name)
    if (clientData) {
      modalHtml = `
      <div class="p-3">
        <dl class="row mb-0 text-start">
          <dt class="col-sm-4">Name:</dt>
          <dd class="col-sm-8 text-center">${clientData.fullName}</dd>
          <dt class="col-sm-4">Phone:</dt>
          <dd class="col-sm-8 text-center">${clientData.phoneNo}</dd>
          <dt class="col-sm-4">Address:</dt>
          <dd class="col-sm-8 text-center">${clientData.address}</dd>
        </dl>
      </div>
    `;
    } else {
      modalHtml = `<div class="p-3 text-danger">Client information could not be loaded.</div>`
    }
    await Swal.fire({
      title: 'Client Information',
      html: modalHtml,
      showCancelButton: true,
      showConfirmButton: false,
      cancelButtonText: 'Close',
      cancelButtonColor: 'gray',
      showCloseButton: true
    })
  }

  const getClientInfo = async (name) => {
    if (!name) return null
    const token = getToken()
    if (!token) return null
    try {
      const response = await axios.get(`${API_BASE_URL}/api/Clients/${encodeURIComponent(name)}`, {
        headers: { Authorization: `Bearer ${token}` }
      })
      return response.data
    } catch (err) {
      console.error('Failed to get client info', err)
      return null
    }
  }


  onMounted(() => {
    loadReservations()
  })
</script>

<style scoped>
  .container {
    font-family: 'Segoe UI', sans-serif;
    background: #ffff;
  }

  h2 {
    font-size: 2.8rem;
    font-weight: 750;
    color: #46ba86;
    text-align: center;
    margin-bottom: 2rem;
  }

  /* Search Bar */
  .input-group {
    max-width: 500px;
    margin: 0 auto 1rem;
    display: flex;
    background: white;
    border-radius: 20px;
    overflow: hidden;
    box-shadow: 0 10px 30px rgba(70, 186, 134, 0.15);
    border: 1px solid rgba(70, 186, 134, 0.2);
  }

  .input-group-text {
    background: transparent !important;
    border: none;
    padding: 0 1.2rem;
  }

  .form-control {
    border: none !important;
    padding: 1.1rem 1rem;
    font-size: 1rem;
    box-shadow: none !important;
  }

    .form-control:focus {
      box-shadow: none !important;
    }

  .input-group button {
    border: none;
    background: transparent;
    padding: 0 1.2rem;
    color: #94a3b8;
  }
</style>
