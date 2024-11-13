import { createWebHistory, createRouter } from "vue-router";
import Login from "../components/Auth/Login.vue";
import IndexHr from "../components/Pages/HR/indexHr.vue";
import ManageAccount from "../components/Pages/HR/manageAccount.vue";
import ManageTitle from "../components/Pages/HR/manageTitle.vue";
import ManageCategory from "../components/Pages/HR/manageCategory.vue";

const routes = [{
        path: '/',
        name: 'login',
        component: Login,
        props: true,
        meta: {
            guest: true,
        }
    },{
        path: '/hr/dashboard',
        name: 'hr-dashboard',
        component: IndexHr,
        props: true,
        meta: {
            requiresAuth: true,
            role: 'HR',
        },
    },{
        path: '/hr/manage-account',
        name: 'manage-account',
        component: ManageAccount,
        props: true,
        meta: {
            requiresAuth: true,
            role: 'HR',
        },
    },{
        path: '/hr/manage-title',
        name: 'manage-title',
        component: ManageTitle,
        props: true,
        meta: {
            requiresAuth: true,
            role: 'HR',
        },
    },{
        path: '/hr/manage-reimbursement-category',
        name: 'manage-reimbursement-category',
        component: ManageCategory,
        props: true,
        meta: {
            requiresAuth: true,
            role: 'HR',
        },
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

router.beforeEach((to, from, next) => {
    const token = localStorage.getItem('token');
    const isAuthenticated = !!token;
  
    if(to.matched.some((record) => record.meta.requiresAuth)){
      if(!isAuthenticated){
        next({
          path: '/',
          query: {
            redirect: to.fullPath,
          },
        });
      }else{
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = payload.role;
  
        if(to.meta.role && to.meta.role !== role){
          next({
            path: '/',
          });
        }else{
          next();
        }
      }
    }else if(to.matched.some((record) => record.meta.guest)){
      if(isAuthenticated){
        const payload = JSON.parse(atob(token.split('.')[1]));
        const role = payload.role;
  
        if(role === 'Employee'){
          next({
            path: '/employee/dashboard',
          });
        }else if(role === 'HR'){
          next({
            path: '/hr/dashboard',
          });
        }else if(role === 'Finance'){
            next({
              path: '/finance/dashboard',
            });
          }
      }else{
        next();
      }
    }else{
      next();
    }
  });

export default router;