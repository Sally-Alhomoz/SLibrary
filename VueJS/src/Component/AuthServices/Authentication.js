import { ref, computed } from 'vue'
import api from '@/Component/AuthServices/authAPI'

const currentUser = ref(null)

const decodeToken = (token) => {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]))
    return {
      username: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
        payload['unique_name'] || payload['name'],
      role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        payload['role']
    }
  } catch {
    return null
  }
}


export const initAuth = () => {
  const token = localStorage.getItem('token')
  if (token) {
    const user = decodeToken(token)
    if (user) currentUser.value = user
    else localStorage.removeItem('token')
  }
}

export const useAuth = () => {
  const login = (token) => {
    localStorage.setItem('token', token)
    const user = decodeToken(token)
    if (user) currentUser.value = user
  }

  const logout = async () => {       
    try {
      await api.post('/api/Account/logout')
      console.log('Backend logout successful → user set to inactive')
    } catch (err) {
      console.warn('Backend logout failed (still clearing locally):', err.message)
    }

    // NOW safely remove token and state
    localStorage.removeItem('token')
    currentUser.value = null
  }

  return {
    currentUsername: computed(() => currentUser.value?.username || ''),
    isAdmin: computed(() => ['Admin', '1', 1].includes(currentUser.value?.role)),
    isAuditor: computed(() => ['Auditor', '2', 2].includes(currentUser.value?.role)),
    canAdd: computed(() =>
      ['Admin', '1', 1, 'Auditor', '2', 2].includes(currentUser.value?.role)
    ),
    login,
    logout
  }
}
