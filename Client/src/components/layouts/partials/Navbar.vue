<template>
    <nav class="bg-base-300 px-4 py-2 flex justify-between items-center">
        <!-- Sidebar Toggle -->
        <button @click="toggleSidebar" class="btn btn-square btn-ghost">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                class="inline-block h-5 w-5 stroke-current">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
            </svg>
        </button>

        <!-- Profile Section -->
        <div class="flex items-center space-x-2">
            <!-- Nama Akun -->
            <span class="text-sm text-gray-600 font-semibold">{{ accountName }}</span>

            <!-- Dropdown -->
            <div class="dropdown dropdown-end">
                <div tabindex="0" role="button" class="btn btn-ghost btn-circle avatar">
                    <div class="w-10 rounded-full">
                        <!-- If no profile picture, show silhouette -->
                        <svg v-if="!userProfilePicture" xmlns="http://www.w3.org/2000/svg" class="w-5 h-10 text-gray-500 mx-auto" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 14c3.866 0 7 3.134 7 7H5c0-3.866 3.134-7 7-7zM12 2a4 4 0 110 8 4 4 0 010-8z" />
                        </svg>
                        <!-- If profile picture exists, show the image -->
                        <img v-else :src="userProfilePicture" alt="Profile Image" class="w-full h-full object-cover" />
                    </div>
                </div>
                <!-- Dropdown Menu -->
                <ul
                    tabindex="0"
                    class="menu menu-sm dropdown-content bg-base-100 rounded-box z-[1] mt-3 w-52 p-2 shadow">
                    <li><a @click="navigateToProfile">Profile</a></li>
                    <li><a @click="logout">Logout</a></li>
                </ul>
            </div>
        </div>
    </nav>
</template>

<script>
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

export default {
    setup(props, { emit }) {
        const router = useRouter();
        const accountName = ref('Guest');
        const userProfilePicture = ref(null);
        const token = localStorage.getItem("token");

        // Decode JWT untuk mendapatkan email
        const decodeToken = (token) => {
            try {
                const payload = JSON.parse(atob(token.split('.')[1]));
                return payload.email; // Ambil email dari payload
            } catch (error) {
                console.error('Invalid token:', error.message);
                return null;
            }
        };

        const emailFromPayload = decodeToken(token);

        const fetchAccountName = async () => {
            if (!emailFromPayload) {
                console.warn('Email not found in token payload.');
                accountName.value = 'Guest'; // Default jika email tidak ada
                return;
            }

            try {
                const response = await axios.get(`https://localhost:7102/api/Account/${emailFromPayload}`, {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                });

                // Ambil nama dan gambar profil dari respons API
                if (response.data?.data?.name) {
                    accountName.value = response.data.data.name;
                } else {
                    console.warn('Name not found in API response.');
                    accountName.value = 'Guest'; 
                }

                // Ambil URL gambar profil jika ada
                userProfilePicture.value = response.data?.data?.profilePicture || null; // null jika tidak ada gambar

            } catch (error) {
                console.error('Failed to fetch account name:', error.response?.data?.message || error.message);
                accountName.value = 'Guest'; // Default jika API gagal
                userProfilePicture.value = null; // Tidak ada gambar profil
            }
        };

        // Fungsi untuk mengontrol sidebar
        const toggleSidebar = () => {
            emit('toggleSidebar');
        };

        // Fungsi untuk navigasi ke profil
        const navigateToProfile = () => {
            router.push('/profile');
        };

        // Fungsi untuk logout
        const logout = () => {
            localStorage.removeItem('token'); 
            router.push('/');
        };

        onMounted(() => {
            fetchAccountName(); 
        });

        return {
            toggleSidebar,
            navigateToProfile,
            logout,
            accountName,
            userProfilePicture,
        };
    }
};
</script>

<style scoped>
/* Tambahkan gaya khusus untuk Navbar jika diperlukan */
</style>
