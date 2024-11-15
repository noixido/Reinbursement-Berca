<template>
    <MainLayout>
        <!-- Back Button -->
        <div class="mb-4">
            <button @click="goBack" class="btn btn-ghost text-gray-700">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"
                    class="inline-block h-5 w-5 stroke-current">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
                </svg>
                Back
            </button>
        </div>

        <!-- Profile Header -->
        <div v-if="user" class="bg-white p-6 shadow rounded-lg mb-6 flex justify-between">
            <div class="flex items-center">
                <!-- Profile Image -->
                <div class="w-24 h-24 rounded-full overflow-hidden mr-6">
                    <img alt="Profile Picture"
                        :src="user.profilePicture || 'https://img.daisyui.com/images/stock/photo-1534528741775-53994a69daeb.webp'"
                        class="w-full h-full object-cover" />
                </div>
                <!-- Profile Info -->
                <div>
                    <h2 class="text-2xl font-semibold text-gray-800">{{ user.name }}</h2>
                    <p class="text-gray-500">{{ user.role_Name }}</p>
                    <p class="text-gray-400">{{ user.email }}</p>
                </div>
            </div>
            <div class="h-full flex gap-2">
                <button @click="openEditModal" id="editProfile" class="text-gray-400 hover:bg-gray-100 p-1 px-2 rounded-xl cursor-pointer flex items-center text-sm">
                    <i class="fas fa-user mr-2 text-xs"></i>
                    Edit Profile
                </button>
                <button @click="openChangePasswordModal" id="editProfile" class="text-gray-400 hover:bg-gray-100 p-1 px-2 rounded-xl cursor-pointer flex items-center text-sm">
                    <i class="fas fa-key mr-2 text-xs"></i>
                    Change Password
                </button>
            </div>
        </div>

        <!-- Error Message -->
        <div v-if="error" class="text-red-500">
            <p>{{ error }}</p>
        </div>


        <!-- Profile Details Section -->
        <div v-if="user" class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- Left Side (Account Information) -->
            <div class="bg-white p-6 shadow rounded-lg">
                <h3 class="text-xl font-semibold text-gray-800 mb-4">Account Information</h3>
                <div class="space-y-4">
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">ID Account:</span>
                        <span class="text-gray-800">{{ user.id_Account }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Email:</span>
                        <span class="text-gray-800">{{ user.email }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Role Name:</span>
                        <span class="text-gray-800">{{ user.role_Name }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Title Name:</span>
                        <span class="text-gray-800">{{ user.title_Name }}</span>
                    </div>
                </div>
            </div>

            <!-- Right Side (Personal Information) -->
            <div class="bg-white p-6 shadow rounded-lg">
                <h3 class="text-xl font-semibold text-gray-800 mb-4">Personal Information</h3>
                <div class="space-y-4">
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Name:</span>
                        <span class="text-gray-800">{{ user.name }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Phone:</span>
                        <span class="text-gray-800">{{ user.phone }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Gender:</span>
                        <span class="text-gray-800">{{ user.gender }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Birth Date:</span>
                        <span class="text-gray-800">{{ formatTanggal(user.birth_Date) }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Join Date:</span>
                        <span class="text-gray-800">{{ formatTanggal(user.join_Date) }}</span>
                    </div>
                    <div class="flex justify-between">
                        <span class="font-medium text-gray-600">Current Limit:</span>
                        <span class="text-gray-800">{{ formatToRupiah(user.current_Limit) }}</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- Edit Profile Modal -->
        <div v-if="showEditModal" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
            <div class="bg-white p-6 rounded-lg shadow-lg w-96">
                <h2 class="text-xl font-semibold mb-4">Edit Profile</h2>

                <!-- Scrollable Form -->
                <form @submit.prevent="updateProfile">
                    <div class="modal-body space-y-4">
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Id</label>
                            <input type="text" v-model="editableUser.id_Account" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Email</label>
                            <input type="text" v-model="editableUser.email" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Role Name</label>
                            <input type="text" v-model="editableUser.role_Name" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Name</label>
                            <input type="text" v-model="editableUser.name" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Phone</label>
                            <input type="text" v-model="editableUser.phone" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Gender</label>
                            <input type="text" v-model="editableUser.gender" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Birth Date</label>
                            <input type="date" v-model="editableUser.birth_Date" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Join Date</label>
                            <input type="date" v-model="editableUser.join_Date" class="input input-bordered w-full" />
                        </div>
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Current Limit</label>
                            <input type="text" v-model="editableUser.current_Limit"
                                class="input input-bordered w-full" />
                        </div>

                        <!-- Title Selection -->
                        <div class="mb-4">
                            <label class="font-medium text-gray-600">Title</label>
                            <select v-model="editableUser.id_Title" class="input input-bordered w-full">
                                <option v-for="title in titleList" :key="title.id_Title" :value="title.id_Title">
                                    {{ title.title_Name }}
                                </option>
                            </select>
                        </div>
                    </div>

                    <div class="flex justify-end space-x-4">
                        <button type="button" @click="closeEditModal" class="btn btn-ghost">Cancel</button>
                        <button type="submit" class="btn btn-primary">Save</button>
                    </div>
                </form>
            </div>
        </div>


        <!-- Change Password Modal -->
        <div v-if="showChangePasswordModal"
            class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
            <div class="bg-white p-6 rounded-lg shadow-lg w-96">
                <h2 class="text-xl font-semibold mb-4">Change Password</h2>

                <form @submit.prevent="changePassword">
                    <div class="mb-4">
                        <label class="font-medium text-gray-600">Old Password</label>
                        <input type="password" v-model="changePasswordData.oldPassword"
                            class="input input-bordered w-full" required />
                    </div>
                    <div class="mb-4">
                        <label class="font-medium text-gray-600">New Password</label>
                        <input type="password" v-model="changePasswordData.newPassword"
                            class="input input-bordered w-full" required />
                    </div>
                    <div class="mb-4">
                        <label class="font-medium text-gray-600">Confirm New Password</label>
                        <input type="password" v-model="changePasswordData.confirmNewPassword"
                            class="input input-bordered w-full" required />
                    </div>
                    <div class="flex justify-end space-x-4">
                        <button type="button" @click="closeChangePasswordModal" class="btn btn-ghost">Cancel</button>
                        <button type="submit" class="btn btn-primary">Change</button>
                    </div>
                </form>
            </div>
        </div>
    </MainLayout>
</template>


<script>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import MainLayout from '../layouts/MainLayout.vue';
import Swal from 'sweetalert2';
import moment from 'moment';

export default {
    components: {
        MainLayout,
    },
    setup() {
        const user = ref(null);
        const titleList = ref([]);
        const editableUser = ref({
            id_Account: '',
            email: '',
            role_Name: '',
            name: '',
            phone: '',
            gender: '',
            birth_Date: '',
            join_Date: '',
            current_Limit: '',
            id_Title: ''
        });
        const error = ref(null);
        const showEditModal = ref(false);
        const showChangePasswordModal = ref(false);

        const token = localStorage.getItem("token");
        const payload = JSON.parse(atob(token.split(".")[1]));
        const emailFromPayload = payload.email;

        const changePasswordData = ref({
            email: emailFromPayload,
            oldPassword: '',
            newPassword: ''
        });

        const fetchUserData = async (email) => {
            try {
                const response = await axios.get(`https://localhost:7102/api/Account/${email}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
                user.value = response.data.data;
                // console.log(user);
            } catch (err) {
                error.value = "Error fetching user data: " + err.message;
            }
        };


        const changePassword = async () => {
            try {
                const response = await axios.put(
                    `https://localhost:7102/api/Account/ChangePassword/${user.value.email}`,
                    changePasswordData.value,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        },
                    }
                );
                // Menampilkan SweetAlert sukses setelah password berhasil diubah
                Swal.fire({
                    icon: 'success',
                    title: 'Password Changed Successfully!',
                    text: 'Your password has been updated.',
                    showConfirmButton: false, // Menghilangkan tombol OK
                    timer: 1500, // SweetAlert akan hilang otomatis setelah 1,5 detik
                });
                console.log("Password changed successfully:", response);
                closeChangePasswordModal();
            } catch (err) {
                // Menampilkan SweetAlert error jika terjadi kesalahan
                Swal.fire({
                    icon: 'error',
                    title: 'Error Changing Password',
                    text: err.response?.data?.message || err.message,
                    showConfirmButton: false, // Menghilangkan tombol OK pada error message juga
                    timer: 1500, // SweetAlert error juga hilang otomatis setelah 1,5 detik
                });
                console.error("Error changing password:", err);
                console.log("Error response:", err.response?.data); // Log the detailed error response
                error.value = "Error changing password: " + (err.response?.data?.message || err.message);
            }
        };


        const fetchTitleList = async () => {
            try {
                const response = await axios.get('https://localhost:7102/api/Titles', {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }
                });
                titleList.value = response.data.data;
                // console.log(titleList.value); // Pastikan data titleList berhasil diambil
            } catch (err) {
                console.error('Error fetching titles:', err);
            }
        };

        // Method to format date into yyyy-MM-dd
        const formatToDate = (date) => {
            const d = new Date(date);
            const year = d.getFullYear();
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const day = String(d.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        };


        const delay = (ms) => new Promise(resolve => setTimeout(resolve, ms));

        const updateProfile = async () => {
            try {
                // Format tanggal dan kirim data update
                // editableUser.value.birth_Date = formatToDate(editableUser.value.birth_Date);
                // editableUser.value.join_Date = formatToDate(editableUser.value.join_Date);

                const response = await axios.put(
                    `https://localhost:7102/api/Account/${user.value.email}`,
                    editableUser.value,
                    {
                        headers: {
                            Authorization: `Bearer ${token}`,
                        }
                    }
                );

                user.value = response.data.data;
                closeEditModal();

                // Menampilkan SweetAlert tanpa tombol 'OK'
                Swal.fire({
                    icon: 'success',
                    title: 'Profile Updated Successfully!',
                    text: 'Your profile information has been updated.',
                    showConfirmButton: false, // Menghilangkan tombol OK
                    timer: 1500, // Menampilkan SweetAlert selama 1,5 detik
                }).then(() => {
                    fetchUserData(emailFromPayload);
                    // window.location.reload(); // Redirect ke halaman profil setelah penundaan
                });

                // window.location.href = "/profile"; // Redirect ke halaman profil setelah penundaan
                // Menunggu SweetAlert selesai ditampilkan
                // await delay(1500); // Sesuaikan waktu delay jika perlu

                // window.location.href = "/profile"; // Redirect ke halaman profil setelah penundaan

            } catch (err) {
                console.error("Error updating profile:", err);
                error.value = "Error updating profile: " + (err.response?.data?.message || err.message);

                await Swal.fire({
                    icon: 'error',
                    title: 'Error Updating Profile',
                    text: err.response?.data?.message || err.message,
                    confirmButtonText: 'Try Again',
                });
            }
        };

        // Edit profile methods
        const openEditModal = () => {
            // Salin data user ke editableUser
            editableUser.value = {
                ...user.value,
                join_Date: formatToISO(user.value.join_Date),
                birth_Date: formatToISO(user.value.birth_Date),
            };

            // Jika `title_Name` ada di user, cari `id_Title` yang sesuai dari titleList
            const selectedTitle = titleList.value.find(title => title.title_Name === user.value.title_Name);
            if (selectedTitle) {
                editableUser.value.id_Title = selectedTitle.id_Title; // Set id_Title ke editableUser
            }

            // Format birth_Date dan join_Date menjadi yyyy-MM-dd
            // editableUser.value.birth_Date = formatToDate(editableUser.value.birth_Date);
            // editableUser.value.join_Date = formatToDate(editableUser.value.join_Date);

            showEditModal.value = true;
        };


        const closeEditModal = () => {
            showEditModal.value = false;
        };

        const openChangePasswordModal = () => {
            showChangePasswordModal.value = true;
        };

        const closeChangePasswordModal = () => {
            showChangePasswordModal.value = false;
        };
        // format tanggal dengan format 13-11-2024 ke 2024-11-13
        const formatToISO = (date) => {
            const [day, month, year] = date.split('-');
            return `${year}-${month}-${day}`;
        };

        onMounted(() => {
            fetchUserData(emailFromPayload);
            fetchTitleList();
        });

        return {
            user,
            editableUser,
            updateProfile,
            error,
            titleList,
            showEditModal,
            showChangePasswordModal,
            changePasswordData,
            openEditModal,
            closeEditModal,
            openChangePasswordModal,
            closeChangePasswordModal,
            changePassword,
            formatToDate
        };
    },
    methods: {
        // format tanggal dengan format 13-11-2024 ke 13 November 2024
        formatTanggal(date) {
            const [day, month, year] = date.split('-');
            const parsedDate = new Date(year, month - 1, day); // Bulan dikurangi 1 karena dimulai dari 0
            const options = { year: 'numeric', month: 'long', day: 'numeric' };
            return parsedDate.toLocaleDateString('id-ID', options);
        },
        formatDate(date) {
            const options = { year: 'numeric', month: 'long', day: 'numeric' };
            return new Date(date).toLocaleDateString(undefined, options);
        },
        goBack() {
            this.$router.go(-1);
        },
        formatToRupiah(number) {
            return new Intl.NumberFormat("id-ID", {
            style: "currency",
            currency: "IDR",
            }).format(number);
        },
    },
};
</script>

<style scoped>
.bg-white {
    background-color: #fff;
}

.p-6 {
    padding: 24px;
}

.shadow {
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.rounded-lg {
    border-radius: 10px;
}

.grid {
    display: grid;
    grid-template-columns: 1fr;
}

.lg\:grid-cols-2 {
    grid-template-columns: repeat(2, 1fr);
}

.gap-6 {
    gap: 24px;
}

.text-2xl {
    font-size: 1.5rem;
}

.text-xl {
    font-size: 1.25rem;
}

.font-semibold {
    font-weight: 600;
}

.text-gray-800 {
    color: #2d3748;
}

.text-gray-500 {
    color: #6b7280;
}

.text-gray-400 {
    color: #9ca3af;
}

.mb-4 {
    margin-bottom: 16px;
}

.font-medium {
    font-weight: 500;
}

.space-y-4>*+* {
    margin-top: 16px;
}

.w-24 {
    width: 6rem;
}

.h-24 {
    height: 6rem;
}

.object-cover {
    object-fit: cover;
}

/* Include the same styling as before, along with modal styling if needed */
.fixed {
    position: fixed;
}

.inset-0 {
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
}

.bg-opacity-50 {
    background-color: rgba(0, 0, 0, 0.5);
}

/* Add this to make the edit form scrollable */
.modal-body {
    max-height: 80vh;
    /* Adjust height to your needs */
    overflow-y: auto;
    /* Allow scrolling */
}

/* Add the main container for profile details to be scrollable */
.profile-container {
    max-height: 80vh;
    /* Set a max height */
    overflow-y: auto;
    /* Allow scrolling */
}
</style>