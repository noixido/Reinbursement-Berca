<template>
    <nav class="bg-base-300 px-4 py-2 flex justify-between items-center">
        <button @click="toggleSidebar" class="btn btn-square btn-ghost">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                class="inline-block h-5 w-5 stroke-current">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16">
                </path>
            </svg>
        </button>
        <!-- <button @click="logout" class="btn">Logout</button> -->
        <div class="dropdown dropdown-end">
            <div tabindex="0" role="button" class="btn btn-ghost btn-circle avatar">
                <div class="w-10 rounded-full">
                <img
                    alt="Tailwind CSS Navbar component"
                    src="https://img.daisyui.com/images/stock/photo-1534528741775-53994a69daeb.webp" />
                </div>
            </div>
            <ul
                tabindex="0"
                class="menu menu-sm dropdown-content bg-base-100 rounded-box z-[1] mt-3 w-52 p-2 shadow">
                <li><a>Profile</a></li>
                <li><a @click="logout">Logout</a></li>
            </ul>
        </div>
    </nav>
</template>

<script>
import { useRouter } from 'vue-router';

export default {
    data() {
        return {
            router: useRouter(),
        };
    },
    methods: {
        toggleSidebar() {
            this.$emit('toggleSidebar');
        },
        logout() {
            // Clear token from localStorage
            localStorage.removeItem("token");

            // Redirect to login page
            this.router.push("/");
        },
    },

    setup() {
        

        return{  }
    }
};
</script>

<style scoped>
/* Tambahkan gaya khusus untuk Navbar jika diperlukan */
</style>