import { createWebHistory, createRouter } from "vue-router";
import Login from "../components/Auth/Login.vue";
import Submission from "../components/pages/Submission.vue";
import History from "../components/pages/History.vue";
import ManageAccount from "../components/Pages/HR/manageAccount.vue";
import ManageTitle from "../components/Pages/HR/manageTitle.vue";
import ManageCategory from "../components/Pages/HR/manageCategory.vue";
import ApprovalByHR from "../components/pages/HR/ApprovalByHR.vue";
import ApprovalFinance from "../components/pages/Finance/ApprovalFinance.vue";
import Profile from "../components/pages/Profile.vue";
import IndexEmployee from "../components/Pages/Employee/indexEmployee.vue";
import IndexHr from "../components/pages/HR/indexHr.vue";
import NotFound from "../components/pages/NotFound.vue";
import IndexFinance from "../components/pages/Finance/indexFinance.vue";

const routes = [{
        path: '/',
        name: 'login',
        component: Login,
        props: true,
        meta: {
            guest: true,
        }
    },{
      path: '/dashboard',
      name: 'employee-dashboard',
      component: IndexEmployee,
      props: true,
      meta: {
        requiresAuth: true,
    },
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
    },{
      path: '/hr/approval-hr',
      name: 'approval-hr',
      component: ApprovalByHR,
      props: true,
      meta: {
          requiresAuth: true,
          role: 'HR',
      }
    },{
      path: '/finance/approval-fnc',
      name: 'approval-fnc',
      component: ApprovalFinance,
      props: true,
      meta: {
          requiresAuth: true,
          role: 'Finance',
      }
    },{
      path: '/finance/dashboard',
      name: 'dashboard-fnc',
      component: IndexFinance,
      props: true,
      meta: {
          requiresAuth: true,
          role: 'Finance',
      }
    },{
      path: '/profile',
      name: 'Profile',
      component: Profile,
      meta: {
        requiresAuth: true,
    }
    },{
      path: '/submission',
      name: 'submission',
      component: Submission,
      props: true,
      meta: {
          requiresAuth: true,
      }
    },{
      path: '/history',
      name: 'history',
      component: History,
      props: true,
      meta: {
          requiresAuth: true,
      }
    },{
      path: '/:pathMatch(.*)*',  // wildcard untuk URL yang tidak ditemukan
      name: 'not-found',
      component: NotFound,
      meta: {
        requiresAuth: true,
    }
    },
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
            path: '/dashboard',
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