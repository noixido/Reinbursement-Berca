<template>
    <div class="fixed top-0 left-0 h-full w-64 bg-[#0e162e] p-4 transition-transform z-50 text-white font-semibold"
        :class="{ 'translate-x-0': sidebarOpen, '-translate-x-full': !sidebarOpen }">
        <div class="flex items-center mb-4">
            <!-- <img src="https://picsum.photos/40" alt="User Avatar" class="rounded-full mr-3" />
            <div>
                <h2 class="text-lg font-bold">User Info</h2>
                <p class="text-sm text-gray-600">{{ payload.name }}</p>
            </div> -->
            <img src="../../../assets/images/logo.png" alt="logo" class="w-20 mx-auto">
        </div>

        <ul class="menu bg-inherit rounded-box w-56 mt-10">

            <!-- EMPLOYEE -->
            
            <!-- Dashboard Employee -->
            <li v-if="role === 'Employee' || role === 'HR' || role === 'Finance'">
                <RouterLink to="/dashboard" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Dashboard
                </RouterLink>
            </li>

            <!-- FINANCE -->

            <!-- Divider for Finance -->
            <div v-if="role === 'Finance'" class="divider divider-neutral">Manage Reimbursement</div>

            <!-- Dashboard Finance -->
            <li v-if="role === 'Finance'" class="mb-1">
                <RouterLink to="/finance/dashboard" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Finance Dashboard
                </RouterLink>
            </li>

            <!-- Approval Finance -->
            <li v-if="role === 'Finance'" class="mb-1">
                <RouterLink to="/finance/approval-fnc" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M5 13l4 4L19 7" />
                    </svg>
                    Approval
                </RouterLink>
            </li>

            <!-- HUMAN RESOURCE -->

            <!-- Divider for HR -->
            <div v-if="role === 'HR'" class="divider divider-neutral">Manage Data</div>

            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/manage-account" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M5.121 19.364A7.488 7.488 0 0112 15a7.488 7.488 0 016.879 4.364M15 10a3 3 0 11-6 0 3 3 0 016 0z" />
                    </svg>
                    Account
                </RouterLink>
            </li>

            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/manage-title" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M5 13l4 4L19 7M3 7h6l7 14l4-8l-7-6H5l-2 8z" />
                    </svg>
                    Employee Title
                </RouterLink>
            </li>

            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/manage-reimbursement-category" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M12 8c-1.38 0-2.5 1.12-2.5 2.5s1.12 2.5 2.5 2.5S14.5 11.88 14.5 10.5 13.38 8 12 8zm0 0V6m0 8v2m0-14a10 10 0 0110 10A10 10 0 0112 22A10 10 0 012 12A10 10 0 0112 2z" />
                    </svg>
                    Reimbursement Category
                </RouterLink>
            </li>


            <div v-if="role === 'HR'" class="divider divider-neutral">Manage Reimbursement</div>
            <!-- Dashboard HR -->
            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/dashboard" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24"
                        stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    HRD Dashboard
                </RouterLink>
            </li>

            <li v-if="role === 'HR'" class="mb-1">
                <RouterLink to="/hr/approval-hr" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M5 13l4 4L19 7" />
                    </svg>
                    Approval
                </RouterLink>
            </li>

            <div class="divider divider-neutral">Reimbursement</div>
            <li class="mb-1">
                <RouterLink to="/submission" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M12 20H6a2 2 0 01-2-2V6a2 2 0 012-2h6m0 0h6a2 2 0 012 2v12a2 2 0 01-2 2h-6m0-16v16" />
                    </svg>
                    Submission
                </RouterLink>
            </li>

            <li class="mb-1">
                <RouterLink to="/history" exact-active-class="active">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                            d="M12 8v4l3 3m-3-7a9 9 0 100 18 9 9 0 000-18z" />
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