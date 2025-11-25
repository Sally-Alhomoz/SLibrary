<template>
  <div class="container py-5 edit-profile-container">
    <h2 class="text-center mb-5 fw-bold profile-title">Profile Settings</h2>

    <div class="card profile-card shadow-lg border-0 rounded-xl">
      <div class="card-body p-4 p-md-5">
        <div class="row">
          <div class="col-lg-4 border-end pe-lg-4 mb-4 mb-lg-0">
            <div class="text-center mb-4">
              <div class="avatar-placeholder mb-3">
                <i class="fas fa-user-circle fa-2x"></i>
              </div>
            </div>
            <br /><br /><br />
            <div class="mt-4 pt-3 border-top text-center">
              <router-link to="/app/profile/changepassword" class="btn btn-outline-secondary me-2">
                <i class="fas fa-lock me-2"></i> Change Password
              </router-link>
            </div>
          </div>

          <div class="col-lg-8 ps-lg-4">
            <h5 class="fw-semibold text-dark mb-4">Account Information</h5>
            <form @submit.prevent="confirm">
              <div class="mb-4">
                <label for="username" class="form-label text-dark"><strong>Username</strong></label>
                <div class="input-group">
                  <input type="text" class="form-control" id="username" v-model="username" readonly />
                  <span class="input-group-text"><i class="fas fa-user text-muted"></i></span>
                </div>
              </div>

              <div class="mb-4">
                <label for="email" class="form-label text-dark"><strong>E-mail</strong></label>
                <div class="input-group">
                  <input type="email"
                         class="form-control"
                         id="email"
                         :value="isEmailEditing ? newEmail : email"
                         @input="event => newEmail = event.target.value"
                         :readonly="!isEmailEditing"
                         :class="{'is-editing': isEmailEditing}" />

                  <button class="btn btn-outline-secondary edit-toggle-btn"
                          type="button"
                          @click="toggleEmailEdit"
                          :title="isEmailEditing ? 'Cancel Editing' : 'Edit Email'">
                    <i :class="isEmailEditing ? 'fas fa-times' : 'fas fa-pen'"></i>
                  </button>
                </div>
              </div>

              <div class="d-flex justify-content-end mt-5">
                <button type="submit" class="btn btn-primary update-btn" v-if="isEmailEditing">
                  Update
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'
  import { useRouter } from 'vue-router'
  import { useConfirm, successDialog, errorDialog } from '@/Component/Modals/ConfirmationModal'
  import api from '@/Component/AuthServices/authAPI'

  const API_BASE_URL = 'https://localhost:7037';

  const username = ref('')
  const email = ref('')
  const newEmail = ref('')
  const confirmDialog = useConfirm().confirmDialog

  const isEmailEditing = ref(false)
  const router = useRouter(); 

  const toggleEmailEdit = () => {
    isEmailEditing.value = !isEmailEditing.value
    if (isEmailEditing.value) {
      newEmail.value = email.value
    } else {
      newEmail.value = email.value
    }
  }


  const confirm = async () => {
    if (newEmail.value === email.value) {
      await errorDialog('No changes detected.', 'Please change your email address before saving.');
      return;
    }

    const confirmed = await confirmDialog(
      'Save Changes?',
      'Are you sure you want to change your email address?',
      'Save'
    )
    if (confirmed) {
      await editEmail()
    }
  }

  const editEmail = async () => {
    try {
      const params = {
        NewEmail: newEmail.value
      };

      await api.patch(`${API_BASE_URL}/api/Account/EditEmail`, params)

      email.value = newEmail.value;
      isEmailEditing.value = false;

      await successDialog('Account information updated.', 'Your email address has been successfully changed.')

    } catch (error) {
      let errorMessage = 'There was an issue updating your profile.'
      if (error.response && error.response.data) {
        errorMessage = error.response.data;
      } else if (error.response) {
        errorMessage = `Error ${error.response.status}: Failed to update.`;
      }
      await errorDialog(errorMessage,'Update Failed')
    }

  }

  const getInfo = async () => {
    try {
      const response = await api.get(`${API_BASE_URL}/api/Account/GetAccountInfo`)
      username.value = response.data.username
      email.value = response.data.email
    } catch (err) {
      console.error("Error fetching info:", err)
    }
  }

  onMounted(() => {
    getInfo()
  })
</script>

<style scoped>

    .edit-profile-container {
        background-color: #ffff;
        min-height: 100vh;
        padding-bottom: 5rem !important;

  }

    .profile-title {
        color: #34495e;
        font-size: 2.5rem;
        margin-bottom: 3rem !important;

  }

    .profile-card {
        border-radius: 1rem;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
        background-color: #ffffff;

  }

    .avatar-placeholder {
        color: #adb5bd;
        font-size: 5rem;
        line-height: 1;
        margin: 0 auto 1.5rem;

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
        background-color: #fff; 

  }

    .input-group-text {
        background-color: #f8f9fa;
        border-left: none;
        border-radius: 0 0.5rem 0.5rem 0;
        border-color: #e0e0e0;
        padding: 0.75rem 1rem;
        color: #6c757d;

  }

    .edit-toggle-btn {
        background-color: #f8f9fa;
        border: 1px solid #e0e0e0;
        border-left: none;
        border-radius: 0 0.5rem 0.5rem 0;
        padding: 0.75rem 1rem;
        color: #6c757d;
        transition: all 0.2s ease;

  }

        .edit-toggle-btn:hover {
            background-color: #e9ecef;
            color: #34495e;

    }


    .form-control:read-only {
        background-color: #f8f9fa; 
        cursor: default;
        border-right: none;

  }

    .form-control.is-editing {
        background-color: #fff; 
        border-color: #46ba86; 
        border-right: 1px solid #46ba86; 

  }
      .form-control.is-editing + .edit-toggle-btn {
          border-color: #46ba86;
          background-color: #fff;
          color: #46ba86;
          box-shadow: 0 0 0 0.25rem rgba(70, 186, 134, 0.25);
          z-index: 3; 
  }

    .mb-4:first-child .input-group-text {
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

  }

    textarea.form-control {
        border-radius: 0.5rem;
        padding: 0.75rem 1rem;
        border-color: #e0e0e0;

  }

      textarea.form-control:focus {
          border-color: #46ba86;
          box-shadow: 0 0 0 0.25rem rgba(70, 186, 134, 0.25);

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

    @media (max-width: 991.98px) {
        .col-lg-4.border-end

  {
          border-right: none !important;
          border-bottom: 1px solid #e0e0e0 !important;
          padding-bottom: 2rem !important;
          margin-bottom: 2rem !important;

  }


  }
</style>
