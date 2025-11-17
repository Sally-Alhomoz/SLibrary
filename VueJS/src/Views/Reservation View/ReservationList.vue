<template>
  <div class="container mt-5">
    <h2 class="mb-4">Reservations List</h2>
    <table class="table table-striped table-bordered">
      <thead class="text-center">
        <tr>
          <th>Reserved By</th>
          <th>Client Name</th>
          <th>Book Title</th>
          <th>Reserve Date</th>
          <th>Release Date</th>
          <th>Reservation Period</th>
        </tr>
      </thead>
      <tbody class="text-center">
        <tr v-for="res in reservations" :key="res.id">
          <td>{{res.reservedBy}}</td>
          <td>
            <!--<router-link :to="{ name: 'ClientInfo', params: { name: res.clientName } }"
                         class="btn btn-sm text-primary"
                         title="View Client Info">
              <i class="fa fa-eye"></i>
            </router-link>-->
            <button class="btn btn-sm text-primary"
                    title="client info"
                    @click="clientInfoModal(res)">
              <i class="fa fa-eye"></i>
            </button>
            {{res.clientName}}
          </td>
          <td>{{res.bookTitle}}</td>
          <td>{{formatDate(res.reservedDate)}}</td>
          <td>
            <span v-if="res.releaseDate">
              {{formatDate(res.releaseDate)}}
            </span>

            <button v-else
                    type="button"
                    class="btn btn-link text-danger p-0 action-button"
                    title="Release Book"
                    @click="confirm(res)">
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
        <tr v-if="reservations.length === 0">
          <td colspan="5" class="text-center text-muted">
            {{ loading ? 'Loading reservations...' : error ? error : 'No reservations found.' }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>


<script setup>
  import { ref, onMounted } from 'vue'
  import axios from 'axios'

  const reservations = ref([])
  const error = ref('')
  const loading = ref(true)
  const client = ref({})

  const API_BASE_URL = 'https://localhost:7037';

  const getToken = () => {
    return localStorage.getItem('token');
  };

  const confirm = async (reservation) => {
    const result = await Swal.fire({
      text: `Do you want to release ${reservation.bookTitle} ? `,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Release',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: 'gray',
      showCloseButton: true
    })

    if (result.isConfirmed) {
      await ReleaseBook(reservation)
    }
  }

  const read = async () => {
    loading.value = true
    error.value = ''

    const response = await axios.get(`${API_BASE_URL}/api/Reservations`)
    reservations.value = response.data

    loading.value = false
  }

  const formatDate = (dateString) => {
    if (!dateString) return '';

    const date = new Date(dateString);

    return isNaN(date) ? dateString : date.toLocaleDateString();
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


  const ReleaseBook = async (reservation)=> {

    const title = reservation.bookTitle; 
    const clientName = reservation.clientName;

    const apiUrl = `${API_BASE_URL}/api/Reservations/Release?title=${encodeURIComponent(title)}&clientname=${encodeURIComponent(clientName)}`;
    const response = await axios.delete(apiUrl)

    await Swal.fire({
      title: 'Success!',
      text: `${reservation.bookTitle} released.`,
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    })

    await read()
  }


  const clientInfoModal = async (reservation) => {
    const name = reservation.clientName;
    let modalHtml = '';

    const clientData = await getClientInfo(name);
    client.value = clientData;

    if (clientData) {
      modalHtml = `
      <div class="p-3">
        <dl class="row mb-0 text-start">

            <dt class="col-sm-4">Name:</dt>
            <dd class="col-sm-7 text-center">${clientData.fullName}</dd> 
            
            <dt class="col-sm-4">Phone:</dt>
            <dd class="col-sm-8 text-center">${clientData.phoneNo}</dd>
            
            <dt class="col-sm-4">Address:</dt>
            <dd class="col-sm-8 text-center">${clientData.address}</dd>
            
            </dl>
      </div>
                    `;
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
    if (!name) {
      error.value = 'Client name is required.'
      loading.value = false
      return
    }

    const token = getToken();
    if (!token) {
      error.value = 'You must be logged in to view client information.'
      loading.value = false
      return
    }

    const response = await axios.get(`${API_BASE_URL}/api/Clients/${encodeURIComponent(name)}`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    return response.data

  }


  onMounted(() => {
    read()
  })
</script>


