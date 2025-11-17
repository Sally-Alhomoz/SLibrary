<template>
  <div class="container mt-5">
    <h2 class="mb-4">Book List</h2>

    <div v-if="canAdd" class="mb-3 d-flex justify-content-end">
      <button class="btn btn-primary" @click="AddBookModal">
        <i class="fas fa-plus-circle"></i> Add Book
      </button>
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
                      @click="confirm(book.id)">
                <i class="fas fa-trash"></i>
              </button>


              <button class="btn btn-sm text-primary"
                      title="Reserve Book"
                      @click="ReserveBookModal(book.title)">
                <i class="fas fa-bookmark"></i>
              </button>
              <!--<router-link :to="{ name: 'ReserveBook', params: { title: book.title } }"
                           class="btn btn-sm text-primary"
                           title="Reserve Book"
                           :disabled="book.available == 0">
                <i class="fas fa-bookmark"></i>
              </router-link>-->
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

  const client = ref({})
  const books = ref([])
  const error = ref('')
  const loading = ref(true)


  const API_BASE_URL = 'https://localhost:7037';

  const getToken = () => {
    return localStorage.getItem('token');
  };

  const confirm = async (id) => {
    const result = await Swal.fire({
      title: 'Delete Book?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Delete',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#D33',
      cancelButtonColor: 'gray',
      showCloseButton: true
    })

    if (result.isConfirmed) {
      await deleteBook(id)
    }
  }


  const read = async () => {
    loading.value = true
    error.value = ''

    const response = await axios.get(`${API_BASE_URL}/api/Books`)
    books.value = response.data

    loading.value = false
  }

  const deleteBook = async (id) => {
    await axios.delete(`${API_BASE_URL}/api/Books?id=${encodeURIComponent(id)}`)

    await Swal.fire({
      title: 'Success!',
      text: 'Book deleted.',
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    })

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

  const AddBookModal = async () => {
    const Modalhtml = `
        <div class="mb-3 text-start">
            <label for="swal-title" class="form-label text-dark">Title</label>
            <input id="swal-title" type="text" class="form-control" placeholder="Book Title" required>
        </div>
        <div class="mb-3 text-start">
            <label for="swal-author" class="form-label text-dark">Author</label>
            <input id="swal-author" type="text" class="form-control" placeholder="Author Name" required>
        </div>
    `;

    const result = await Swal.fire({
      title: 'Add New Book',
      html: Modalhtml,
      icon: 'info',
      showCancelButton: true,
      confirmButtonText: 'Add',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: 'gray',
      focusConfirm: false, 

      preConfirm: () => {
        const title = document.getElementById('swal-title').value.trim();
        const author = document.getElementById('swal-author').value.trim();

        if (!title || !author) {
          Swal.showValidationMessage('Title and Author are required!');
          return false;
        }
        return { title, author };
      }
    });
    if (result.isConfirmed) {
      const { title, author } = result.value;
      await AddBook(title, author);
    }
  };

  const AddBook = async (title, author) => {
    await axios.post(`${API_BASE_URL}/api/Books`, {
      Title: title,
      Author: author
    });

    await Swal.fire({
      title: 'Success!',
      text: `Book ${title} Added.`,
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    });

    await read();
  }

  const ReserveBookModal = async (title) => {
    const Modalhtml = `
        <div class="mb-3 text-start">
            <label for="swal-title" class="form-label text-dark">Book Title</label>
            <input id="swal-title" type="text" class="form-control" placeholder="Book Title" value="${title}" required>
        </div>
        <div class="mb-3 text-start">
            <label for="swal-phone" class="form-label text-dark">Phone Number</label>
            <input id="swal-phone" type="text" class="form-control" placeholder="Phone Number" required>
        </div>

        <div class="mb-3 text-start">
            <label for="swal-name" class="form-label text-dark">Client Name</label>
            <input id="swal-name" type="text" class="form-control" placeholder="Client Name" required>
        </div>

        <div class="mb-3 text-start">
            <label for="swal-address" class="form-label text-dark">Address</label>
            <input id="swal-address" type="text" class="form-control" placeholder="Address" required>
        </div>

    `;

    const result = await Swal.fire({
      title: 'Reserve Book',
      html: Modalhtml,
      icon: 'info',
      showCancelButton: true,
      confirmButtonText: 'Reserve',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: 'gray',
      focusConfirm: false,

      preConfirm: () => {
        const Title = document.getElementById('swal-title').value.trim();
        const phone = document.getElementById('swal-phone').value.trim();
        const name = document.getElementById('swal-name').value.trim();
        const address = document.getElementById('swal-address').value.trim();

        if (!Title || !phone || !name || !address) {
          Swal.showValidationMessage('All Information required !');
          return false;
        }
        return { Title, phone, name , address };
      }
    });
    if (result.isConfirmed) {
      const { Title, phone, name, address } = result.value;
      await ReserveBook(Title, phone, name, address);
    }
  };

  const ReserveBook = async (Title ,phone , name , address ) => {
    const token = getToken();
    if (!token) {
      error.value = 'You must be logged in to view client information.'
      loading.value = false
      return
    }

    const url = `${API_BASE_URL}/api/Reservations/Reserve`;
    const params = {
      title: Title,
      clientname: name,
      phoneNo: phone,
      address: address
    }

    const response = await axios.post(url, null, {
      params,
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    await Swal.fire({
      title: 'Success!',
      text: 'Book Reseved',
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    });

    await read();

  }

  onMounted(() => {
    read()
  })
</script>
