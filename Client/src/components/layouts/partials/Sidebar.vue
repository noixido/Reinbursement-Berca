<template>
    <div class="fixed top-0 left-0 h-full w-64 bg-base-200 p-4 transition-transform z-50"
        :class="{ 'translate-x-0': sidebarOpen, '-translate-x-full': !sidebarOpen }">
        <div class="flex items-center mb-4">
            <img src="https://picsum.photos/40" alt="User Avatar" class="rounded-full mr-3" />
            <div>
                <h2 class="text-lg font-bold">User Info</h2>
                <p class="text-sm text-gray-600">{{ payload.name }}</p>
            </div>
        </div>
        <ul class="menu bg-base-200 rounded-box w-56">

            <!-- EMPLOYEE -->
            
            <!-- Dashboard Employee -->
            <li v-if="role === 'Employee'">
                <RouterLink to="/employee/dashboard" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Dashboard
                </RouterLink>
            </li>

            <!-- FINANCE -->

            <!-- Dashboard Finance -->
            <li v-if="role === 'Finance'" class="mb-1">
                <RouterLink to="/finance/dashboard" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Dashboard
                </RouterLink>
            </li>

            <!-- Divider for Finance -->
            <div v-if="role === 'Finance'" class="divider">Manage Reimbursement</div>

            <!-- Approval Finance -->
            <li v-if="role === 'Finance'" class="mb-1">
                <RouterLink to="/finance/approval-fnc" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Approval
                </RouterLink>
            </li>

            <!-- HUMAN RESOURCE -->

            <!-- Dashboard HR -->
            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/dashboard" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Dashboard
                </RouterLink>
            </li>

            <!-- Divider for HR -->
            <div v-if="role === 'HR'" class="divider">Manage Data</div>

            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/manage-account" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Account
                </RouterLink>
            </li>
            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/manage-title" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Employee Title
                </RouterLink>
            </li>
            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/manage-reimbursement-category" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Reimbursement Category
                </RouterLink>
            </li>

            <div v-if="role === 'HR'" class="divider">Manage Reimbursement</div>
            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/approval-hr" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                    </svg>
                    Approval
                </RouterLink>
            </li>

            <div class="divider">Reimbursement</div>
            <li class="mb-1">
                <RouterLink to="/submission" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                    </svg>
                    Submission
                </RouterLink>
            </li>
            <li class="mb-1">
                <RouterLink to="/history" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                    </svg>
                    History
                </RouterLink>
            </li>
        </ul>
    </div>
</template>

<script>
import { RouterLink } from 'vue-router';

export default {
    props: {
        sidebarOpen: {
            type: Boolean,
            required: true,
        },
    },
    components: {
        RouterLink
    },
    data(){
        return {
            role: null,
            payload: null,
        };
    },
    created(){
        this.getUserRole();
        this.getPayload();
    },
    methods: {
        getUserRole(){
            const token = localStorage.getItem("token");
            if(token){
                try{
                    const payload = JSON.parse(atob(token.split(".")[1]));
                    this.role = payload.role;
                }catch(err){
                    console.error("Invalid token!", err);
                }
            }
            
        },
        getPayload(){
            const token = localStorage.getItem("token");
            if(token){
                try{
                    this.payload = JSON.parse(atob(token.split(".")[1]));
                }catch(err){
                    console.error("Invalid token!", err);
                }
            }
        }
    }
};
</script>

<style scoped>
/* Tambahkan gaya khusus untuk Sidebar jika diperlukan */
</style>