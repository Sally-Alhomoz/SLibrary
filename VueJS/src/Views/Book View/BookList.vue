<template>
  <div class="container mt-5">
    <h2 class="mb-4">Book List</h2>
    <br />

    <div class="row mb-4 align-items-center">
      <!-- Search  -->
      <div class="col-lg-6 col-md-7">
        <div class="input-group shadow-sm">
          <span class="input-group-text bg-light border-end-0">
            <i class="fas fa-search text-muted"></i>
          </span>
          <input v-model="searchQuery"
                 @input="debouncedSearch"
                 type="text"
                 class="form-control border-start-0"
                 placeholder="Search..."
                 style="font-size: 0.95rem;" />
          <button v-if="searchQuery"
                  @click="clearSearch"
                  class="btn btn-outline-secondary border-start-0"
                  type="button">
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>

      <div class="col-lg-6 col-md-5 text-end">
        <button v-if="canAdd" class="btn btn-primary px-4" @click="AddBookModal">
          <i class="fas fa-plus-circle me-2"></i>Add Book
        </button>
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
            <th @click="sortBy('title')" class="cursor-pointer user-select-none">
              Title <i :class="sortIcon('title')"></i>
            </th>
            <th @click="sortBy('author')" class="cursor-pointer user-select-none">
              Author <i :class="sortIcon('author')"></i>
            </th>
            <th @click="sortBy('reserved')" class="cursor-pointer user-select-none">
              Reserved <i :class="sortIcon('reserved')"></i>
            </th>
            <th @click="sortBy('available')" class="cursor-pointer user-select-none">
              Available <i :class="sortIcon('available')"></i>
            </th>
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
                        @click="confirmDelete(book)">
                  <i class="fas fa-trash"></i>
                </button>


                <button class="btn btn-sm text-primary"
                        title="Reserve Book"
                        @click="ReserveBookModal(book.title)">
                  <i class="fas fa-bookmark"></i>
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- EMPTY STATE -->
    <div v-if="books.length === 0" class="text-center py-5">
      <i class="fas fa-book-open fa-5x text-muted opacity-50"></i>
      <h4 class="text-muted">No books found</h4>
      <p v-if="searchQuery" class="text-muted">
        No results for "<strong>{{ searchQuery }}</strong>"
      </p>
    </div>
  </div>

  <!-- PAGINATION -->
  <div class="mt-5">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <div class="text-muted small">
        Showing {{ ((currentPage - 1) * perPage) + 1 }}–{{ Math.min(currentPage * perPage, totalItems) }} of {{ totalItems }} books
      </div>
    </div>

    <div class="d-flex justify-content-start">
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
                :active-class="'active'"
                class="m-0" />
    </div>

    <div class="text-muted">
      Page {{ currentPage }} of {{ totalPages || 1 }}
    </div>
  </div>

</template>



<script setup>
  import { ref, onMounted, computed } from 'vue'
  import Paginate from 'vuejs-paginate-next'
  import { useConfirm, useConfirmWarning, useInputDialog, successDialog, errorDialog } from '@/Component/Modals/ConfirmationModal'
  import api from '@/Component/AuthServices/authAPI'
  import { useAuth } from '@/Component/AuthServices/Authentication'

  //state
  const client = ref({})
  const books = ref([])
  const error = ref('')
  const loading = ref(true)
  const confirmDialogWarning = useConfirmWarning().confirmDialog
  const confirmDialog = useConfirm().confirmDialog
  const inputDialog = useInputDialog().inputDialog
  const currentPage = ref(1)
  const totalPages = ref(1)
  const totalItems = ref(0)
  const perPage = ref(10)
  const { isAdmin, canAdd } = useAuth()

  // Search & Sort
  const searchQuery = ref('')
  const sortByField = ref('title')
  const sortDirection = ref('asc')

  const API_BASE_URL = 'https://localhost:7037';


  function debounce(fn, delay) {
    let timeout
    return (...args) => {
      clearTimeout(timeout)
      timeout = setTimeout(() => fn(...args), delay)
    }
  }

  // Debounce search
  const debouncedSearch = debounce(() => {
    currentPage.value = 1
    loadBooks()
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
    loadBooks()
  }

  function sortIcon(field) {
    if (sortByField.value !== field) return 'fas fa-sort ms-2 opacity-50'
    return sortDirection.value === 'asc'
      ? 'fas fa-sort-up ms-2 text-primary'
      : 'fas fa-sort-down ms-2 text-primary'
  }

  //Load Books
  const loadBooks = async () => {
    loading.value = true
    error.value = ''

    try {
      const response = await api.get(`${API_BASE_URL}/api/Books`, {
        params: {
          page: currentPage.value,
          pageSize: perPage.value,
          search: searchQuery.value.trim(),
          sortBy: sortByField.value,
          sortDirection: sortDirection.value
        }
      })

      const data = response.data
      books.value = data.items || data
      totalItems.value = data.totalCount || data.length
      totalPages.value = Math.ceil(totalItems.value / perPage.value)
    } catch (err) {
      error.value = 'Failed to load books: ' + (err.response?.data || err.message)
      console.error(err)
    } finally {
      loading.value = false
    }
  }

  const onPageChange = (page) => {
    currentPage.value = page
    loadBooks()
  }

  const confirmDelete = async (book) => {
    const confirmed = await confirmDialogWarning(
      'Delete Book',
      '',
      'Delete'
    )

    if (confirmed) {
      await deleteBook(book)
    }
  }

  const deleteBook = async (book) => {
    try {
      await api.delete(`${API_BASE_URL}/api/Books?id=${encodeURIComponent(book.id)}`);

      await successDialog(`${book.title} has been released successfully!`)
    } catch (error) {
      let errorMessage = 'An unexpected error occurred during deletion.';

      if (error.response && error.response.data) {
        errorMessage = error.response.data;
      } else if (error.response) {
        errorMessage = `Error ${error.response.status}: Failed to delete the book.`;
      }

      await errorDialog(errorMessage, 'Deletion Failed 🛑')
    }
    await loadBooks();
  }




  const AddBookModal = async () => {
    const html = `
        <div class="mb-3 text-start">
            <label for="swal-title" class="form-label text-dark">Title</label>
            <input id="swal-title" type="text" class="form-control" placeholder="Book Title" required>
        </div>
        <div class="mb-3 text-start">
            <label for="swal-author" class="form-label text-dark">Author</label>
            <input id="swal-author" type="text" class="form-control" placeholder="Author Name" required>
        </div>
    `;

    const preConfirmFn = () => {
      const title = document.getElementById('swal-title').value.trim();
      const author = document.getElementById('swal-author').value.trim();

      if (!title || !author) {
        Swal.showValidationMessage('Title and Author are required!');
        return false;
      }
      return { title, author }; 
    };

    const data = await inputDialog('Add New Book', html, 'Add', preConfirmFn)
    if (data) {
      const { title, author } = data;
      await AddBook(title, author);
    }
        
  }

  const AddBook = async (title, author) => {
    await api.post(`${API_BASE_URL}/api/Books`, {
      Title: title,
      Author: author
    });

    await successDialog(`Book ${title} Added successfully!`)

    await loadBooks();
  }

  const ReserveBookModal = async (title) => {
    const html = `
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
    const preConfirmFn = () => {
      const Title = document.getElementById('swal-title').value.trim();
      const phone = document.getElementById('swal-phone').value.trim();
      const name = document.getElementById('swal-name').value.trim();
      const address = document.getElementById('swal-address').value.trim();

      if (!Title || !phone || !name || !address) {
        Swal.showValidationMessage('All Information required !');
        return false;
      }
      return { Title, phone, name, address }
    };

    const data = await inputDialog('Reserve Book', html, 'Reserve', preConfirmFn)
    if (data) {
      const { Title, phone, name, address } = data;
      await ReserveBook(Title, phone, name, address);
    }
  }

  const ReserveBook = async (Title ,phone , name , address ) => {
    const url = `${API_BASE_URL}/api/Reservations/Reserve`;
    const params = {
      title: Title,
      clientname: name,
      phoneNo: phone,
      address: address
    }
    const response = await api.post(url, null, {
      params
    })
    await successDialog(`Book ${Title} reserved successfully !`)

    await loadBooks();
  }

  onMounted(() => {
    loadBooks()
  })
</script>
