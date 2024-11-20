<template>
    <div class="flex h-screen">
        <Sidebar :sidebarOpen="sidebarOpen" />
        <div class="flex-1 flex flex-col transition-transform"
            :style="{ marginLeft: sidebarOpen ? '16rem' : '0', transition: 'margin-left 0.3s ease' }">
            <Navbar @toggleSidebar="toggleSidebar" />
            <main class="flex-1 p-4">
                <!-- <Breadcrumb class="mb-4" :items="breadcrumbItems" /> -->
                <slot></slot>
                <!-- <router-view></router-view> -->
            </main>
            <Footer></Footer>
        </div>
    </div>
</template>

<script>
import Sidebar from './partials/sidebar.vue';
import Navbar from './partials/navbar.vue';
import Breadcrumb from './partials/Breadcrumb.vue';
import { useRoute } from 'vue-router';
import { computed, ref } from 'vue';
import Footer from './partials/Footer.vue';
import axios from 'axios';

export default {
    components: {
        Sidebar,
        Navbar,
        Breadcrumb,
        Footer
    },
    setup() {
        const sidebarOpen = ref(true); // Menggunakan ref untuk sidebarOpen

        const route = useRoute();

        // const breadcrumbItems = computed(() => {
        //     const items = [
        //         { name: 'Home', path: '/' },
        //     ];

        //     if (route.path === '/dashboard') {
        //         items.push({ name: 'Dashboard', path: '/dashboard' });
        //     } else if (route.path === '/helloworld') {
        //         items.push({ name: 'Hello World', path: '/helloworld' });
        //     }

        //     return items;
        // });

        return { sidebarOpen };
    },
    // mounted(){
    //     this.updateCurrentLimit();
    //     this.startInterval();
    // },
    methods: {
        toggleSidebar() {
            this.sidebarOpen = !this.sidebarOpen;
        },
        // updateCurrentLimit(){
        //     const api = "https://localhost:7102/api/Account/updateCurrentLimitPeriodically";
        //     return axios
        //         .post(api, {}, {
        //             headers: {
        //             Authorization: `Bearer ${this.token}`,
        //             },
        //         })
        //         .then((response)=>{
        //             console.log(response.data.message);
        //         })
        //         .catch((error)=>{
        //             console.error(error);
        //         });
        // },
        // startInterval(){
        //     setInterval(()=>{
        //         this.updateCurrentLimit();
        //     }, 86400000);
        // }
    },

};
</script>

<style scoped>
/* Tambahkan gaya khusus untuk MainLayout jika diperlukan */
.transition-transform {
    transition: transform 0.3s ease;
}
</style>