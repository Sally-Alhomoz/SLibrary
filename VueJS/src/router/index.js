import { createRouter, createWebHistory } from 'vue-router'

import AppLayout from '../Layouts/AppLayout.vue'
import Login from '../Views/Login.vue'
import Dashboard from '../Views/Dashboard.vue'
import BookList from '../Views/Book View/BookList.vue'
import ReservationList from '../Views/Reservation View/ReservationList.vue'
import Register from '../Views/Account View/Register.vue'
import UserList from '../Views/Account View/UserList.vue'
import Profile from '../Views/Account View/Profile.vue'
import ChangePassword from '../Views/Account View/ChangePassword.vue'
import EditAccount from '../Views/Account View/EditAccount.vue'

const routes = [
  {
    path: '/',
    name: 'Login',
    component: Login
  },
  {
    path: '/register',
    name: 'Register',
    component: Register
  },
  {
    path: '/app', 
    component: AppLayout,

    children: [
      {
        path: 'dashboard', 
        name: 'Dashboard',
        component: Dashboard
      },
      {
        path: 'books',
        name: 'BookList',
        component: BookList
      },
      {
        path: 'reservations',
        name: 'ReservationList',
        component: ReservationList
      },
      {
        path: 'users',
        name: 'UserList',
        component: UserList
      },
      {
        path: 'profile',
        name: 'Profile',
        component: Profile
      },
      {
        path: 'profile/changepassword',
        name: 'ChangePassword',
        component: ChangePassword
      },
      {
        path: 'profile/editaccount',
        name: 'EditAccount',
        component: EditAccount
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})



router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')

  const publicRoutes = ['/', '/register']
  const isPublic = publicRoutes.includes(to.path)

  if (!isPublic && !token) {
    next('/')  
  } else {
    next()    
  }
})

export default router
