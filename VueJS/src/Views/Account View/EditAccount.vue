<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <h2 class="text-center mb-5 text-dark fw-bold">Edit Account</h2>

        <div class="card shadow-lg border-0 rounded-xl">
          <div class="card-body p-5">
            <form @submit.prevent="confirm">
              <div class="mb-3">
                <input v-model="newEmail"
                       placeholder="New email"
                       type="email"
                       required
                       class="form-control" />
              </div>

              <button type="submit" class="btn btn-primary w-100">Update</button>

            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import axios from 'axios'

  const newEmail = ref('')

  const router = useRouter()

  const API_BASE_URL = 'https://localhost:7037';

  const getToken = () => {
    return localStorage.getItem('token');
  };


  const confirm = async () => {
    const result = await Swal.fire({
      title: 'Save Change?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Save',
      cancelButtonText: 'Cancel',
      confirmButtonColor: '#3085d6',
      cancelButtonColor: 'gray',
      showCloseButton: true
    })

    if (result.isConfirmed) {
      await editEmail()
    }
  }


  const editEmail = async () => {
    const token = getToken();
    if (!token) {
      error.value = 'You must be logged in to view client information.'
      return
    }

    const params = {
      NewEmail: newEmail.value
    };

    await axios.patch(`${API_BASE_URL}/api/Account/EditEmail`, params, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    await Swal.fire({
      title: 'Success!',
      text: 'Account information updated.',
      icon: 'success',
      timer: 2000,
      showConfirmButton: false
    })

    router.push('/app/profile')

  }

</script>
