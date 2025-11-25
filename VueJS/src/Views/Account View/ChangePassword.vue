<template>
  <div class="container py-5 change-password-container">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <h2 class="text-center mb-5 fw-bold profile-title">Change Password</h2>

        <div class="card profile-card shadow-lg border-0 rounded-xl">
          <div class="card-body p-5">
            <form @submit.prevent="confirm">
              <div class="mb-4">
                <label for="oldPassword" class="form-label text-dark"><strong>Old Password</strong></label>
                <div class="input-group">
                  <input v-model="oldPassword"
                                     placeholder="Enter old password"
                                     type="password"
                                     required
                                     class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-lock text-muted"></i></span>
                </div>
              </div>

              <div class="mb-4">
                <label for="newPassword" class="form-label text-dark"><strong>New Password</strong></label>
                <div class="input-group">
                  <input v-model="newPassword"
                                     placeholder="Enter new password"
                                     type="password"
                                     required
                                     class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-key text-muted"></i></span>
                </div>
              </div>

              <div class="mb-5">
                <label for="confirmPassword" class="form-label text-dark"><strong>Confirm New Password</strong></label>
                <div class="input-group">
                  <input v-model="confirmPassword"
                                     placeholder="Confirm new password"
                                     type="password"
                                     required
                                     class="form-control password-input" />
                  <span class="input-group-text"><i class="fas fa-check-circle text-muted"></i></span>
                </div>
              </div>

              <button type="submit" class="btn update-btn w-100 shadow-sm">Change Password</button>

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
    if (newPassword.value === oldPassword.value) {
      await errorDialog('Your new password cannot be the same as your old password.')
      return
    }
    const confirmed = await confirmDialogWarning(
      'Change Password',
      'You will be logged out after this.',
      'Change'
    )

    if (confirmed) {
      await changePassword()
    }
  }

  const changePassword = async () => {
    const Params = {
      OldPassword: oldPassword.value,
      NewPassword: newPassword.value,
      ConfirmPassword: confirmPassword.value
    };

    try {
      await api.patch(`${API_BASE_URL}/api/Account`, Params)
      await successDialog('Password changed. Logging out...')
      await authLogout()
      router.push('/')
    }
    catch (error) {
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

<style scoped>

  .change-password-container {
    background-color: #f9fafb;
    min-height: 100vh;
  }

  .profile-title {
    color: #34495e;
    font-size: 2.5rem;
  }

  .profile-card {
    border-radius: 1rem;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
    background-color: #ffffff;
  }

  .form-label {
    font-weight: 500;
    color: #6c757d;
    margin-bottom: 0.5rem;
  }
  .input-group .form-control {
    border-radius: 0.5rem 0 0 0.5rem;
    padding: 0.75rem 1rem;
    border-color: #e0e0e0;
    z-index: 2;
  }

  .input-group .input-group-text {
    background-color: #f8f9fa;
    border-left: none;
    border-radius: 0 0.5rem 0.5rem 0;
    border-color: #e0e0e0;
    padding: 0.75rem 1rem;
    color: #6c757d;
  }

  .form-control:focus {
    border-color: #46ba86;
    box-shadow: 0 0 0 0.25rem rgba(70, 186, 134, 0.25);
    z-index: 3;
  }

    .form-control:focus + .input-group-text {
      border-color: #46ba86;
      box-shadow: none;
    }

  .update-btn {
    background-color: #46ba86;
    border-color: #46ba86;
    color: white;
    padding: 0.75rem 2rem;
    border-radius: 0.5rem;
    font-weight: 600;
    transition: all 0.2s ease;
  }

    .update-btn:hover {
      background-color: #46ba86;
      border-color: #46ba86;
      transform: translateY(-3px);
      box-shadow: 0 10px 20px rgba(70, 186, 134, 0.4);
    }
</style>
