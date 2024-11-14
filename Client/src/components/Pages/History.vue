<template>
    <MainLayout>
        <!-- Search Bar -->
        <h1 class="text-center text-2xl font-bold mb-4">History Reimbursement {{ account_Name }}</h1>
        <div class="mb-4 flex justify-end">
            <input v-model="searchQuery" type="text" placeholder="Cari data..."
                class="input input-bordered w-full max-w-xs" />
        </div>

        <!-- Table -->
        <table class="table">
            <thead>
                <tr>
                    <th>Nomor</th>
                    <th>ID Reimbursement</th>
                    <th>Kategori</th>
                    <th>Tanggal Pengajuan</th>
                    <th>Jumlah Dana</th>
                    <th>Status</th>
                    <th>Catatan</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in paginatedData" :key="item.id_Reimbursement" v-if="paginatedData.length > 0">
                    <td>{{ index + 1 }}</td>
                    <td>{{ item.id_Reimbursement }}</td>
                    <td>{{ item.category_Name }}</td>
                    <td>{{ new Date(item.submit_Date).toLocaleDateString('id-ID', { year: 'numeric', month: 'long', day: '2-digit' }) }}</td>
                    <td>Rp. {{ formatCurrency(item.amount) }}</td>
                    <td>
                        <span :class="{
                            'badge badge-warning': item.status.includes('progress'),
                            'badge badge-success': item.status.includes('approved'),
                            'badge badge-error': item.status.includes('declined')
                        }">
                            {{ item.status }}
                        </span>
                    </td>
                    <td>{{ item.note || '-' }}</td>
                    <td>
                        <button
                            class="btn btn-info btn-xs mr-2 bg-[#45aafd] hover:bg-[#45aafd] focus:outline-none focus:ring-none text-white"
                            @click="openModal(item)">
                            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 inline-block" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0zm1.73 4.73A8.94 8.94 0 0121 12a8.94 8.94 0 01-4.27 4.73m-8.73 0A8.94 8.94 0 013 12a8.94 8.94 0 014.27-4.73" />
                            </svg>
                        </button>
                    </td>
                </tr>

                <!-- Jika tidak ada data, tampilkan pesan "Data tidak ditemukan" -->
                <tr v-if="paginatedData.length === 0">
                    <td colspan="8" class="text-center text-gray-500 py-4">Data tidak ditemukan</td>
                </tr>
            </tbody>
        </table>

        <!-- Pagination Controls -->
        <div v-if="paginatedData.length > 0" id="kontrol" class="flex justify-center items-center mt-4">
            <button @click="currentPage--" :disabled="currentPage === 1" class="btn">Previous</button>
            <span class="mx-4">Page {{ currentPage }} of {{ totalPages }}</span>
            <button @click="currentPage++" :disabled="currentPage === totalPages" class="btn">Next</button>
        </div>

        <!-- Modal -->
        <dialog ref="reimbursementModal" class="modal">
            <div class="modal-box max-w-3xl">
                <form method="dialog">
                    <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
                </form>
                <h3 class="text-center text-2xl font-bold mb-4">Detail Reimbursement</h3>
                <div class="py-4 space-y-3 text-gray-800">

                    <div class="text-center">
                        <div class="font-semibold">📌 ID Reimbursement:</div>
                        <div>{{ selectedReimbursement.id_Reimbursement }}</div>
                    </div>
                    <div class="border border-blue-500 space-y-3 rounded-lg p-4 bg-gray-50 mb-3">
                        <div class="flex justify-between">
                            <span class="font-semibold">🆔 ID User:</span>
                            <span>{{ selectedReimbursement.id_Account }}</span>
                        </div>
                        <div class="flex justify-between">
                            <span class="font-semibold">👤 Nama User:</span>
                            <span>{{ selectedReimbursement.name }}</span>
                        </div>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📁 Kategori:</span>
                        <span>{{ selectedReimbursement.category_Name }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📅 Tanggal Pengajuan:</span>
                        <span>{{ new Date(selectedReimbursement.submit_Date).toLocaleDateString('id-ID', { year: 'numeric', month: 'long', day: '2-digit' }) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">💰 Jumlah Dana:</span>
                        <span>Rp {{ formatCurrency(selectedReimbursement.amount) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📈 Status:</span>
                        <span :class="{
                            'badge badge-warning': selectedReimbursement.status && selectedReimbursement.status.includes('progress'),
                            'badge badge-success': selectedReimbursement.status && selectedReimbursement.status.includes('approv'),
                            'badge badge-error': selectedReimbursement.status && selectedReimbursement.status.includes('declined')
                        }">
                            {{ selectedReimbursement.status }}
                        </span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📝 Catatan:</span>
                        <div class="md:max-w-md mt-2 md:mt-0 font-semibold whitespace-normal break-words">{{
                            selectedReimbursement.note || "Tidak ada catatan" }}</div>
                    </div>

                    <div class="flex justify-center space-x-4 mt-4">
                        <button class="btn btn-success" @click="approveReimbursement(selectedReimbursement.id_Reimbursement)">Approve</button>
                        <button class="btn btn-error" @click="declineReimbursement(selectedReimbursement.id_Reimbursement)">Decline</button>
                    </div>
                </div>
            </div>
        </dialog>

    </MainLayout>
</template>

<script>
import axios from 'axios';
import MainLayout from '../layouts/MainLayout.vue';

export default {
    components: {
        MainLayout
    },
    data() {
        const token = localStorage.getItem('token');
        const payload = JSON.parse(atob(token.split(".")[1]));
        const id_Account = payload.id_account;
        const account_Name = payload.name;
        return {
            reimbursements: [],
            searchQuery: '',
            id_Account,
            account_Name,
            currentPage: 1,
            itemsPerPage: 10, // Adjust this to change the number of items per page
            selectedReimbursement: {}, // to hold the selected reimbursement details
        };
    },
    computed: {
        filteredData() {
            return this.reimbursements.filter(item =>
                item.category_Name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                item.status.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                item.id_Reimbursement.toLowerCase().toString().includes(this.searchQuery) ||
                item.status.toLowerCase().toString().includes(this.searchQuery)
            );
        },
        totalPages() {
            return Math.ceil(this.filteredData.length / this.itemsPerPage);
        },
        paginatedData() {
            const start = (this.currentPage - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            return this.filteredData.slice(start, end);
        }
    },
    mounted() {
        this.fetchReimbursements();
    },
    methods: {
        fetchReimbursements() {
            axios
                .get("https://localhost:7102/api/Reimbursement/account/" + this.id_Account, {
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('token')
                    }
                })
                .then((response) => {
                    this.reimbursements = response.data.data;
                })
                .catch((error) => {
                    this.reimbursements = []
                });
        },
        openModal(item) {
            this.selectedReimbursement = item; // set the selected reimbursement
            this.$refs.reimbursementModal.showModal(); // open the modal
        },
        formatCurrency(value) {
            if (!value) return '0';
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        },
    },
    watch: {
        currentPage(value) {
            if (value > this.totalPages) this.currentPage = this.totalPages;
        }
    }
};
</script>

<style lang="scss" scoped>
.table {
    width: 100%;
    border-collapse: collapse; /* Remove table borders */
}

.table th,
.table td {
    padding: 8px;
    text-align: center;
}

.table th {
    background-color: #f8f8f8;
    font-weight: bold;
}

.table tr:nth-child(even) {
    background-color: #f2f2f2;
}

#kontrol .btn {
    background-color: #007bff;
    color: white;
}

#kontrol .btn:disabled {
    background-color: #e0e0e0;
}

#kontrol .btn:hover {
    background-color: #0056b3;
}

.modal-box {
    max-width: 600px;
    margin: 0 auto;
    padding: 20px;
}

.modal-box textarea {
    resize: vertical;
    min-height: 100px;
}
</style>
