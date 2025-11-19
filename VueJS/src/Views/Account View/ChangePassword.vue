<template>
  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <h2 class="text-center mb-5 text-dark fw-bold">Change Password</h2>

        <div class="card shadow-lg border-0 rounded-xl">
          <div class="card-body p-5">
            <form @submit.prevent="confirm">
              <div class="mb-3">
                <input v-model="oldPassword"
                       placeholder="Old Password"
                       type="password"
                       required
                       class="form-control" />
              </div>

              <div class="mb-3">
                <input v-model="newPassword"
                       placeholder="New Password"
                       type="password"
                       required
                       class="form-control" />
              </div>

              <div class="mb-3">
                <input v-model="confirmPassword"
                       placeholder="Confirm Password"
                       type="password"
                       required
                       class="form-control" />
              </div>

              <button type="submit" class="btn btn-primary w-100">Change</button>

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
  import { useConfirmWarning, successDialog, errorDialog } from '@/Component/Modals/ConfirmationModal'
  import api from '@/Component/AuthServices/authAPI'
  import { useAuth } from '@/Component/AuthServices/Authentication'

  const error = ref('')
  const oldPassword = ref('')
  const newPassword = ref('')
  const confirmPassword = ref('')
  const confirmDialogWarning = useConfirmWarning().confirmDialog

  const router = useRouter()
  const { currentUsername, logout: authLogout } = useAuth()

  const API_BASE_URL = 'https://localhost:7037';


  const confirm = async () => {
    if (newPassword.value !== confirmPassword.value) {
      await errorDialog('Passwords do not match')
      return
    }
    const confirmed = await confirmDialogWarning(
      'Change Password',
      'You will be logged out after this.',
      'Change'
    )

  if (confirmed) {
    await changePassword()  }
  }

  const changePassword = async () => {
    const Params = {
      OldPassword: oldPassword.value,
      NewPassword: newPassword.value,
      ConfirmPassword: confirmPassword.value
    };

    try {
      await api.patch(`${API_BASE_URL}/api/Account`,Params)
      await successDialog('Password changed. Logging out...')
      await authLogout()
      router.push('/')
    }
    catch(error) {
      let errorMessage = 'An unexpected error occurred.';

      if (error.response && error.response.data) {
        errorMessage = error.response.data;
      } else if (error.response) {
        errorMessage = `Error ${error.response.status}: Failed to change password`;
      }
      await errorDialog(errorMessage)
      return
    }
  }
</script>
