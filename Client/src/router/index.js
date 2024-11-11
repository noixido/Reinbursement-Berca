import { createWebHistory, createRouter } from "vue-router";
import Login from "../components/Auth/Login.vue";
import MainLayout from "../components/layouts/MainLayout.vue";
import IndexEmployee from "../components/Pages/Employee/index-employee.vue";

const routes = [{
    path: '/',
    name: 'login',
    component: Login,
    props: true,
    meta: {
        guest: true,
    }
},{
    path: '/',
    component: MainLayout,
    children:[{
        path: 'employee',
        name: 'employee',
        component: IndexEmployee,
        meta: {
            requiresAuth: true,
            role: 'Employee',
        }
    }]
}]

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
            path: '/employee',
          });
        }else if(role === 'HR'){
          next({
            path: '/hr',
          });
        }else if(role === 'Finance'){
            next({
              path: '/finance',
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