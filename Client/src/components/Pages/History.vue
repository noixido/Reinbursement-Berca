<template>
    <MainLayout>
        <!-- Search Bar -->
        <h1 class="text-3xl font-bold mb-10">History Reimbursement {{ account_Name }}</h1>
        <!-- <div class="mb-4 flex justify-end">
            <input v-model="searchQuery" type="text" placeholder="Cari data..."
                class="input input-bordered w-full max-w-xs" />
        </div> -->
        <!-- Search Bar and Show Entries -->
        <div class="mb-4 flex justify-between items-center">
        <div class="flex items-center">
            <label class="mr-2">Show</label>
            <select
            v-model="itemsPerPage"
            @change="currentPage = 1"
            class="border border-gray-300 rounded p-2 bg-white"
            >
            <option v-for="option in [10, 25, 50, 100]" :key="option" :value="option">
                {{ option }}
            </option>
            </select>
            <span class="ml-2">entries</span>
        </div>
        <div class="relative w-full max-w-xs">
            <input v-model="searchQuery" type="text" placeholder="Search..." class="input input-bordered w-full max-w-xs" />
        </div>
        </div>

        <!-- Table -->
        <table class="table w-full border-collapse">
            <thead>
                <tr>
                    <th class="p-2 text-center font-bold">No</th>
                    <th class="p-2 text-center font-bold">Username</th>
                    <th class="p-2 text-center font-bold">Kategori</th>
                    <th class="p-2 text-center font-bold">Tanggal Pengajuan</th>
                    <th class="p-2 text-center font-bold">Jumlah Dana</th>
                    <th class="p-2 text-center font-bold">Dana Disetujui</th>
                    <th class="p-2 text-center font-bold">Status</th>
                    <th class="p-2 text-center font-bold">Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in paginatedData" :key="item.id_Reimbursement" v-if="paginatedData.length > 0">
                    <td class="p-2 text-center">{{ index + 1 }}</td>
                    <td class="p-2 text-center">{{ item.name }}</td>
                    <td class="p-2 text-center">{{ item.category_Name }}</td>
                    <td class="p-2 text-center">{{ new Date(item.submit_Date).toLocaleDateString('id-ID', {
                        year: 'numeric', month: 'long', day:
                            '2-digit'
                    }) }}</td>
                    <td class="p-2 text-center">Rp. {{ formatCurrency(item.amount) }}</td>
                    <td class="p-2 text-center">Rp. {{ item.approve_Amount ? formatCurrency(item.approve_Amount) : "-" }}</td>
                    <td class="p-2 text-center">
                        <span :class="{
                            'badge badge-warning': item.status.includes('Progress'),
                            'badge badge-success': item.status.includes('Approved'),
                            'badge badge-error': item.status.includes('Declined')
                        }" class="badge-status">
                            {{ item.status }}
                        </span>
                    </td>
                    <td class="p-2 text-center">
                        <button
                            class="btn btn-info mr-2 bg-[#45aafd] focus:outline-none focus:ring-none text-white relative group"
                            @click="openModal(item)"
                            title="View Details"
                        >
                            <i class="fas fa-eye"></i>
                            <!-- Tooltip -->
                            <div
                            class="absolute bottom-full left-1/2 transform -translate-x-1/2 mb-2 hidden group-hover:block bg-gray-800 text-white text-sm py-1 px-2 rounded shadow-md"
                            >
                            View Data
                            </div>
                        </button>
                    </td>
                </tr>

                <!-- Jika tidak ada data, tampilkan pesan "Data tidak ditemukan" -->
                <tr v-if="paginatedData.length === 0">
                    <td colspan="8" class="text-center py-4 text-red-500">No data found</td>
                </tr>
            </tbody>
        </table>

        <!-- Pagination Controls -->
        <div class="flex justify-between mt-5">
            <div class="mb-4">
                <span class="text-sm">
                    Showing {{ startItem }} to {{ endItem }} of {{ filteredData.length }} entries
                </span>
            </div>
            <div class="join mt-4">
                <button class="join-item btn" @click="currentPage = Math.max(1, currentPage - 1)"
                    :disabled="currentPage <= 1">
                    Prev
                </button>
                <button v-for="page in pageNumbers" :key="page" class="join-item btn" @click="currentPage = page"
                    :class="{ 'btn-active': currentPage === page }">
                    {{ page }}
                </button>
                <button class="join-item btn" @click="currentPage = totalPages" :disabled="currentPage >= totalPages">
                    Last
                </button>
            </div>
        </div>
        <!-- Modal -->
        <dialog ref="reimbursementModal" class="modal">
            <div class="modal-box max-w-3xl text-center">
                <form method="dialog">
                    <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
                </form>
                <h3 class="text-center text-2xl font-bold mb-4">Detail Reimbursement</h3>

                <div class="inline-block border-4" :class="{
                        'border-blue-500': selectedStatus.includes('approved'),
                        'border-red-500': selectedStatus.includes('decline'),
                        'border-yellow-500': selectedStatus.includes('progress'),
                    }">
                    <div :class="{
                        'bg-blue-500': selectedStatus.includes('approved'),
                        'bg-red-500': selectedStatus.includes('decline'),
                        'bg-yellow-500': selectedStatus.includes('progress'),
                    }" class="inline-block p-2 border-4 w-auto text-white font-bold border-white">
                        <span v-if="selectedStatus === 'approved'">
                            Reimbursement akan diterima pada bulan {{ selectedDateReceived }}
                        </span>
                        <span v-else-if="selectedStatus.includes('decline')">
                            Reimbursement ditolak.
                        </span>
                        <span v-else>
                            Reimbursement sedang dalam proses.
                        </span>
                    </div>
                </div>

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
                            <span class="font-semibold">👤 User Name:</span>
                            <span>{{ selectedReimbursement.name }}</span>
                        </div>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📁 Category:</span>
                        <span>{{ selectedReimbursement.category_Name }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📅 Submission Date:</span>
                        <span>{{ new Date(selectedReimbursement.submit_Date).toLocaleDateString('id-ID', {
                            year:
                                'numeric', month: 'long', day: '2-digit'
                        }) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">💰 Total Funds:</span>
                        <span>Rp {{ formatCurrency(selectedReimbursement.amount) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">✔️ Approved Funds:</span>
                        <span>Rp. {{ selectedReimbursement.approve_Amount ?
                            formatCurrency(selectedReimbursement.approve_Amount) : "-" }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📈 Status:</span>
                        <span :class="{
                            'badge badge-warning': selectedReimbursement.status && selectedReimbursement.status.includes('Progress'),
                            'badge badge-success': selectedReimbursement.status && selectedReimbursement.status.includes('Approv'),
                            'badge badge-error': selectedReimbursement.status && selectedReimbursement.status.includes('Declined')
                        }">
                            {{ selectedReimbursement.status }}
                        </span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📝 Notes:</span>
                        <div class="md:max-w-md mt-2 md:mt-0 font-semibold whitespace-normal break-words">{{
                            selectedReimbursement.note || "Tidak ada catatan" }}</div>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📎 Evidence:</span>
                        <a :href="`https://localhost:7102/api/file/` + selectedReimbursement.evidence" target="_blank"
                            rel="noopener noreferrer" class="text-blue-500 hover:underline">
                            Lihat evidence
                        </a>
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

            selectedStatus: '',
            selectedDateReceived: '',
        };
    },
    computed: {
        filteredData() {
            // Filter data based on search query
            return this.reimbursements.filter(item =>
                item.category_Name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                item.status.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                item.id_Reimbursement.toLowerCase().toString().includes(this.searchQuery) ||
                item.status.toLowerCase().toString().includes(this.searchQuery)
            );
        },
        totalPages() {
            // Calculate total pages
            return Math.ceil(this.filteredData.length / this.itemsPerPage);
        },
        pageNumbers() {
            return Array.from({ length: this.totalPages }, (_, i) => i + 1);
        },
        startItem() {
            return (this.currentPage - 1) * this.itemsPerPage + 1;
        },
        endItem() {
            return Math.min(this.currentPage * this.itemsPerPage, this.filteredData.length);
        },
        paginatedData() {
            // Slice data for current page
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


            const status = item.status.toLowerCase();
            const submitDate = new Date(item.submit_Date);

            const isBefore15 = submitDate.getDate() <= 15;

            const month = submitDate.getMonth() + (isBefore15 ? 0 : 1);
            const year = submitDate.getFullYear();
            const dateReceived = new Date(year, month, 1).toLocaleDateString('id-ID', { month: 'long', year: 'numeric' });

            this.selectedStatus = status;
            this.selectedDateReceived = dateReceived;

            this.$refs.reimbursementModal.showModal(); // open the modal
            console.log(this.selectedReimbursement)
        },
        formatCurrency(value) {
            if (!value) return '0';
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
        },
    },
    watch: {
        currentPage(value) {
            // Reset to last page if we go beyond
            if (value > this.totalPages) this.currentPage = this.totalPages;
        }
    }
};
</script>

<style lang="scss" scoped>
.table {
    width: 100%;
    border-collapse: collapse;
    border: 1px solid #000; /* Border hitam pada tabel */
}

.table th,
.table td {
    padding: 8px;
    text-align: center;
    border: 1px solid #000; /* Border hitam pada setiap sel */
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
    border: none;
    cursor: pointer;
}

#kontrol .btn:disabled {
    background-color: #ccc;
    cursor: not-allowed;
}

.badge-status {
    display: inline-block; /* Membuat elemen berbentuk inline-block */
    font-size: 0.75rem; /* Ukuran font kecil */
    font-weight: normal; /* Tidak bold */
    white-space: nowrap; /* Menghindari teks terpotong ke bawah */
    overflow: hidden; /* Menyembunyikan teks yang melebihi area */
    text-align: center; /* Memastikan teks selalu rata tengah */
}


.table tr:nth-child(even) {
    background-color: #f2f2f2;
}

.table tr:nth-child(odd) {
    background-color: #ffffff;
}

</style>
