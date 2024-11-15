<template>
    <MainLayout>
        <!-- Search Bar -->
        <h1 class="text-center text-2xl font-bold mb-4">
            Approval Reimbursement for Finance
        </h1>
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
            <div class="flex justify-end">
                <input v-model="searchQuery" type="text" placeholder="Cari data..." class="input input-bordered w-full max-w-xs" />
            </div>
        </div>

        <!-- Table -->
        <div class="overflow-x-auto">
        <table class="table w-full">
                <thead>
                    <tr>
                        <th>Nomor</th>
                        <th>ID Reimbursement</th>
                        <th>Kategori</th>
                        <th>Tanggal Pengajuan</th>
                        <th>Jumlah Dana</th>
                        <th>Dana Disetujui</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in paginatedData" :key="item.id_Reimbursement" v-if="paginatedData.length > 0">
                        <td>{{ index + 1 }}</td>
                        <td>{{ item.id_Reimbursement }}</td>
                        <td>{{ item.category_Name }}</td>
                        <td>
                            {{
                                new Date(item.submit_Date).toLocaleDateString('id-ID', {
                                    year: 'numeric',
                                    month: 'long',
                                    day: '2-digit',
                                })
                            }}
                        </td>
                        <td>Rp. {{ formatCurrency(item.amount) }}</td>
                        <td>
                            Rp.
                            {{
                                item.approve_Amount ? formatCurrency(item.approve_Amount) : '-'
                            }}
                        </td>
                        <td>
                            <span :class="{
                                'badge badge-warning': item.status.includes('progress'),
                                'badge badge-success': item.status.includes('approved'),
                                'badge badge-error': item.status.includes('declined'),
                            }">
                                {{ item.status }}
                            </span>
                        </td>
                        <td>
                            <button
                                class="btn btn-info btn-xs mr-2 bg-[#45aafd] hover:bg-[#45aafd] focus:outline-none focus:ring-none text-white"
                                @click="openModal(item)"
                                title="View Details"
                            >
                                <i class="fas fa-eye"></i>
                            </button>
                        </td>
                    </tr>
                        <tr v-if="filteredData.length === 0">
                            <td colspan="7" class="text-center py-4 text-red-500">No data found</td>
                        </tr>
                    </tbody>
                </table>
            </div>

        <!-- Pagination Controls -->
        <div class="flex justify-between mt-5">
            <div>
                <span class="text-sm">
                Showing {{ startItem }} to {{ endItem }} of {{ filteredData.length }} entries
                </span>
            </div>
            <div class="join">
                <button class="join-item btn" @click="currentPage = Math.max(1, currentPage - 1)" :disabled="currentPage <= 1">
                Prev
                </button>
                <button
                v-for="page in pageNumbers"
                :key="page"
                class="join-item btn"
                @click="currentPage = page"
                :class="{'btn-active': currentPage === page}"
                >
                {{ page }}
                </button>
                <button class="join-item btn" @click="currentPage = Math.min(currentPage + 1, totalPages)" :disabled="currentPage >= totalPages">
                Next
                </button>
            </div>
        </div>

        <!-- Modal -->
        <dialog ref="reimbursementModal" class="modal">
            <div class="modal-box max-w-3xl">
                <form method="dialog">
                    <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">
                        ✕
                    </button>
                </form>
                <h3 class="text-center text-2xl font-bold mb-4">
                    Detail Reimbursement
                </h3>
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
                        <div class="flex justify-between">
                            <span class="font-semibold">💳 Current Limit:</span>
                            <span>Rp.
                                {{ formatCurrency(selectedReimbursement.current_Limit) }}</span>
                        </div>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📁 Kategori:</span>
                        <span>{{ selectedReimbursement.category_Name }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📅 Tanggal Pengajuan:</span>
                        <span>{{
                            new Date(selectedReimbursement.submit_Date).toLocaleDateString(
                                'id-ID',
                                {
                                    year: 'numeric',
                                    month: 'long',
                                    day: '2-digit',
                                }
                            )
                        }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">💰 Jumlah Dana:</span>
                        <span>Rp {{ formatCurrency(selectedReimbursement.amount) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">✔️ Dana Disetujui:</span>
                        <span>Rp. {{ selectedReimbursement.approve_Amount ? formatCurrency(selectedReimbursement.approve_Amount) : '-' }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📈 Status:</span>
                        <span :class="{
                            'text-green-600 font-semibold':
                                selectedReimbursement.status === 'Approved',
                            'text-red-600 font-semibold':
                                selectedReimbursement.status === 'Rejected',
                            'text-yellow-600 font-semibold':
                                selectedReimbursement.status === 'Pending',
                        }">
                            {{ selectedReimbursement.status }}
                        </span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📝 Catatan:</span>
                        <div class="md:max-w-md mt-2 md:mt-0 font-semibold whitespace-normal break-words">
                            {{ selectedReimbursement.note || 'Tidak ada catatan' }}
                        </div>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📎 Evidence:</span>
                        <a :href="`https://localhost:7102/api/file/` +
                            selectedReimbursement.evidence
                            " target="_blank" rel="noopener noreferrer" class="text-blue-500 hover:underline">
                            Lihat evidence
                        </a>
                    </div>
                </div>

                <div class="border border-yellow-500 rounded-lg p-4 bg-gray-200 mb-3">
                    <!-- Tombol Approve dan Decline -->
                    <div class="flex justify-center space-x-4">
                        <button class="btn btn-success" @click="openApproveForm">
                            Approve
                        </button>
                        <button class="btn btn-error" @click="openDeclineForm">
                            Decline
                        </button>
                    </div>
    
                    <!-- Form Approve atau Decline -->
                    <div v-if="showApproveForm" class="mt-4 space-y-3 flex flex-col">
                        <label class="font-semibold">Jumlah Dana Disetujui:</label>
                        <div class="flex justify-center space-x-2">
                            <span class="text-gray-700">{{ 0 }}</span>
                            <input v-model="approveAmount" type="range" 
                            :max="Math.min(selectedReimbursement.current_Limit, selectedReimbursement.amount)"
                            class="range range-primary w-2/3" />
                            <span class="text-gray-700">{{ Math.min(selectedReimbursement.current_Limit, selectedReimbursement.amount) }}</span>
                        </div>
                        <div class="flex">
                            <input v-model="approveAmount" type="number" 
                                :max="Math.min(selectedReimbursement.current_Limit, selectedReimbursement.amount)"
                                class="input input-bordered w-2/4 mb-2 mx-auto" placeholder="Masukkan jumlah dana disetujui" />
                        </div>
    
                        <label class="font-semibold">Catatan:</label>
                        <textarea v-model="note" class="textarea textarea-bordered w-full"
                            placeholder="Masukkan catatan"></textarea>
                        <button class="btn btn-primary mt-2 w-full" @click="submitApproval">
                            Submit Approval
                        </button>
                    </div>
    
                    <div v-if="showDeclineForm" class="mt-4 space-y-3">
                        <label class="font-semibold">Catatan:</label>
                        <textarea v-model="note" class="textarea textarea-bordered w-full"
                            placeholder="Masukkan alasan penolakan"></textarea>
                        <button class="btn btn-primary mt-2 w-full" @click="submitDecline">
                            Submit Decline
                        </button>
                    </div>
                </div>
            </div>
        </dialog>
    </MainLayout>
</template>

<script>
import axios from 'axios';
import MainLayout from '../../layouts/MainLayout.vue';
import Swal from 'sweetalert2';

export default {
    components: {
        MainLayout,
    },
    data() {
        const token = localStorage.getItem('token');
        const payload = JSON.parse(atob(token.split('.')[1]));
        const id_Account = payload.id_account;
        const account_Name = payload.name;
        return {
            reimbursements: [],
            searchQuery: '',
            account_Name,
            currentPage: 1,
            itemsPerPage: 10, // Adjust this to change the number of items per page
            selectedReimbursement: {}, // to hold the selected reimbursement details
            showApproveForm: false, // to toggle approve form visibility
            showDeclineForm: false, // to toggle decline form visibility
            approveAmount: 0, // amount to approve
            note: '', // note for approval or decline
        };
    },
    computed: {
        filteredData() {
            // Filter data based on search query
            return this.reimbursements.filter(
                (item) =>
                    item.category_Name
                        .toLowerCase()
                        .includes(this.searchQuery.toLowerCase()) ||
                    item.status
                        .toLowerCase()
                        .includes(this.searchQuery.toLowerCase()) ||
                    item.id_Reimbursement
                        .toLowerCase()
                        .toString()
                        .includes(this.searchQuery) ||
                    item.status.toLowerCase().toString().includes(this.searchQuery)
            );
        },
        totalPages() {
            // Calculate total pages
            return Math.ceil(this.filteredData.length / this.itemsPerPage);
        },
        paginatedData() {
            // Slice data for current page
            const start = (this.currentPage - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            return this.filteredData.slice(start, end);
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
    },
    methods: {
        fetchReimbursements() {
            axios
                .get('https://localhost:7102/api/Reimbursement/finance', {
                    headers: {
                        Authorization: 'Bearer ' + localStorage.getItem('token'),
                    },
                })
                .then((response) => {
                    this.reimbursements = response.data.data ? response.data.data : [];
                })
                .catch((error) => {
                    console.error('Error fetching data:', error);
                });
        },
        openModal(item) {
            this.showApproveForm = false;
            this.showDeclineForm = false;
            this.approveAmount = 0;
            this.note = '';

            this.selectedReimbursement = item; // set the selected reimbursement
            this.$refs.reimbursementModal.showModal(); // open the modal
        },
        formatCurrency(value) {
            if (!value) return '0';
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        },
        openApproveForm() {
            this.showApproveForm = true;
            this.showDeclineForm = false;
        },
        openDeclineForm() {
            this.showDeclineForm = true;
            this.showApproveForm = false;
        },
        submitApproval() {
            const approvalData = {
                approve_Amount: this.approveAmount,
                note: this.note,
                id_Reimbursement: this.selectedReimbursement.id_Reimbursement,
            };

            axios
                .put(
                    `https://localhost:7102/api/Reimbursement/approvefinance/${this.selectedReimbursement.id_Reimbursement}`,
                    approvalData,
                    {
                        headers: {
                            Authorization: 'Bearer ' + localStorage.getItem('token'),
                        },
                    }
                )
                .then((response) => {
                    // Handle success
                    this.fetchReimbursements(); // refresh the data
                    this.$refs.reimbursementModal.close(); // close modal
                    Swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Reimbursement Diterima',
                        showConfirmButton: false,
                        timer: 1500,
                    });
                })
                .catch((error) => {
                    // Handle error
                    console.error('Error approving reimbursement:', error);
                    Swal.fire({
                        position: 'center',
                        icon: 'error',
                        title: 'Operasi Gagal',
                        showConfirmButton: false,
                        timer: 1500,
                    });
                });
        },
        submitDecline() {
            const declineData = {
                note: this.note,
                id_Reimbursement: this.selectedReimbursement.id_Reimbursement,
            };

            axios
                .put(
                    `https://localhost:7102/api/Reimbursement/declinefinance/${this.selectedReimbursement.id_Reimbursement}`,
                    declineData,
                    {
                        headers: {
                            Authorization: 'Bearer ' + localStorage.getItem('token'),
                        },
                    }
                )
                .then((response) => {
                    // Handle success
                    this.fetchReimbursements(); // refresh the data
                    this.$refs.reimbursementModal.close(); // close modal
                    Swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Reimbursement Ditolak',
                        showConfirmButton: false,
                        timer: 1500,
                    });
                })
                .catch((error) => {
                    // Handle error
                    console.error('Error declining reimbursement:', error);
                    Swal.fire({
                        position: 'center',
                        icon: 'error',
                        title: 'Operasi Gagal',
                        showConfirmButton: false,
                        timer: 1500,
                    });
                });
        },
    },
    mounted() {
        this.fetchReimbursements();
    },
    watch: {
        currentPage(value) {
            // Reset to last page if we go beyond
            if (value > this.totalPages) this.currentPage = this.totalPages;
        },
    },
};
</script>

<style lang="scss" scoped>
.table {
    width: 100%;
    border-collapse: collapse;
}

.table th,
.table td {
    /* Hapus border */
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
    border: none;
    cursor: pointer;
}

#kontrol .btn:disabled {
    background-color: #ccc;
    cursor: not-allowed;
}
</style>
