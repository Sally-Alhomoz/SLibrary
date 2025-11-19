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
  import { useConfirm, successDialog, errorDialog } from '@/Component/Modals/ConfirmationModal'
  import api from '@/Component/AuthServices/authAPI'

  const newEmail = ref('')
  const confirmDialog= useConfirm().confirmDialog
  const router = useRouter()


  const API_BASE_URL = 'https://localhost:7037';

  const confirm = async () => {
    const confirmed = await confirmDialog(
      'Save Changes?',
      '',
      'Save'
    )
    if (confirmed) {
      await editEmail()
    }
  }


  const editEmail = async () => {
    const params = {
      NewEmail: newEmail.value
    };

    await api.patch(`${API_BASE_URL}/api/Account/EditEmail`, params)

    await successDialog('Account information updated.')
    router.push('/app/profile')

  }

</script>
