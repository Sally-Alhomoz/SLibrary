import { createRouter, createWebHistory } from 'vue-router'

import AppLayout from '../Layouts/AppLayout.vue'
import Login from '../Views/Login.vue'
import Dashboard from '../Views/Dashboard.vue'
import BookList from '../Views/Book View/BookList.vue'
import ReservationList from '../Views/Reservation View/ReservationList.vue'
import Register from '../Views/Account View/Register.vue'
import AddBook from '../Views/Book View/AddBook.vue'
import UserList from '../Views/Account View/UserList.vue'

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
        path: 'books/add',
        name: 'AddBook',
        component: AddBook
      },
      {
        path: 'users',
        name: 'UserList',
        component: UserList
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
  const authRequired = to.path !== '/'

  if (authRequired && !token) {
    next('/') 
  } else {
    next() 
  }
})

export default router
