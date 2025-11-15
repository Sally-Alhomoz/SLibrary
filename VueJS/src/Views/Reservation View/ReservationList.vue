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
            <router-link :to="{ name: 'ClientInfo', params: { name: res.clientName } }"
                         class="btn btn-sm text-primary"
                         title="View Client Info">
              <i class="fa fa-eye"></i>
            </router-link>
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

  const API_BASE_URL = 'https://localhost:7037';


  const confirm = async (reservation) => {
    const result = await Swal.fire({
      title: 'Relase Book?',
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
      text: 'Book released.',
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    })

    await read()
  }


  onMounted(() => {
    read()
  })
</script>


