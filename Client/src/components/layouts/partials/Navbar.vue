<template>
    <nav class="bg-base-300 px-4 py-2 flex justify-between items-center">
        <button @click="toggleSidebar" class="btn btn-square btn-ghost">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                class="inline-block h-5 w-5 stroke-current">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16">
                </path>
            </svg>
        </button>
        <button @click="logout" class="btn">Logout</button>
    </nav>
</template>

<script>
import { useRouter } from 'vue-router';

export default {
    methods: {
        toggleSidebar() {
            this.$emit('toggleSidebar');
        },
    },

    setup() {
        const router = useRouter()

        const logout = () => {
            localStorage.clear();
            router.push("/");
        };

        return{ logout }
    }
};
</script>

<style scoped>
/* Tambahkan gaya khusus untuk Navbar jika diperlukan */
</style>