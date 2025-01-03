<template>
    <MainLayout>
        <!-- Search Bar -->
        <h1 class="text-3xl font-bold mb-10">
            Approval Reimbursement for Finance
        </h1>
        <!-- Search Bar and Show Entries -->
        <div class="mb-4 flex justify-between items-center">
            <div class="flex items-center">
                <label class="mr-2">Show</label>
                <select v-model="itemsPerPage" @change="currentPage = 1"
                    class="border border-gray-300 rounded p-2 bg-white">
                    <option v-for="option in [10, 25, 50, 100]" :key="option" :value="option">
                        {{ option }}
                    </option>
                </select>
                <span class="ml-2">entries</span>
            </div>
            <div class="relative w-full max-w-xs">
                <input v-model="searchQuery" type="text" placeholder="Search..."
                    class="input input-bordered w-full max-w-xs" />
            </div>
        </div>

        <!-- Table -->
        <div class="overflow-x-auto">
            <table class="table w-full">
                <thead>
                    <tr>
                        <th>No</th>
                        <th>Name</th>
                        <th>Category</th>
                        <th>Submit Date</th>
                        <th>Total Funds</th>
                        <th>Approved Funds</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in paginatedData" :key="item.id_Reimbursement"
                        v-if="paginatedData.length > 0">
                        <td>{{ index + 1 }}</td>
                        <td>{{ item.name }}</td>
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
                                'badge badge-warning': item.status.includes('Progress'),
                                'badge badge-success': item.status.includes('Approved'),
                                'badge badge-error': item.status.includes('Declined')
                            }" class="badge-status text-white">
                                {{ item.status }}
                            </span>
                        </td>
                        <td class="p-2 text-center">
                            <button
                                class="btn btn-info mr-2 bg-[#45aafd] focus:outline-none focus:ring-none text-white relative group"
                                @click="openModal(item)" title="View Details">
                                <i class="fas fa-eye"></i>
                                <!-- Tooltip -->
                                <div
                                    class="absolute bottom-full left-1/2 transform -translate-x-1/2 mb-2 hidden group-hover:block bg-gray-800 text-white text-sm py-1 px-2 rounded shadow-md">
                                    View Data
                                </div>
                            </button>
                        </td>
                    </tr>
                    <tr v-if="filteredData.length === 0">
                        <td colspan="8" class="text-center py-4 text-red-500">No data found</td>
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
                <button class="join-item btn" @click="currentPage = Math.max(1, currentPage - 1)"
                    :disabled="currentPage <= 1">
                    Prev
                </button>
                <button v-for="page in pageNumbers" :key="page" class="join-item btn" @click="currentPage = page"
                    :class="{ 'btn-active': currentPage === page }">
                    {{ page }}
                </button>
                <button class="join-item btn" @click="currentPage = Math.min(currentPage + 1, totalPages)"
                    :disabled="currentPage >= totalPages">
                    Next
                </button>
            </div>
        </div>

        <!-- Modal -->
        <dialog ref="reimbursementModal" class="modal">
            <div class="modal-box max-w-3xl py-0">
                <div class="bg-white sticky top-0 p-3">
                    <form method="dialog">
                        <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">
                            ✕
                        </button>
                    </form>
                    <h3 class="text-center text-2xl font-bold mb-4">
                        Detail Reimbursement
                    </h3>
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
                            <span class="font-semibold">👤 Name:</span>
                            <span>{{ selectedReimbursement.name }}</span>
                        </div>
                        <div class="flex justify-between">
                            <span class="font-semibold">💳 Current Limit:</span>
                            <span>Rp.
                                {{ formatCurrency(selectedReimbursement.current_Limit) }}</span>
                        </div>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📁 Category:</span>
                        <span>{{ selectedReimbursement.category_Name }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📅 Submit Date:</span>
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
                        <span class="font-semibold">💰 Total Funds:</span>
                        <span>Rp {{ formatCurrency(selectedReimbursement.amount) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">✔️ Approved Funds:</span>
                        <span>Rp. {{ selectedReimbursement.approve_Amount ?
                            formatCurrency(selectedReimbursement.approve_Amount) : '-' }}</span>
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
                        <span class="font-semibold">📝 Notes:</span>
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

                <div class="border border-blue-500 bg-gray-50 rounded-lg p-4 mb-3">
                    <!-- Tombol Approve dan Decline -->
                    <div class="flex justify-center space-x-4 text-white font-semibold">
                        <button class="btn bg-blue-500 hover:bg-blue-900 hover:text-white text-white" @click="openApproveForm">
                            Approve
                        </button>
                        <button class="btn bg-red-500 hover:bg-red-900  hover:text-white text-white" @click="openDeclineForm">
                            Decline
                        </button>
                    </div>

                    <!-- Form Approve atau Decline -->
                    <div v-if="showApproveForm" class="mt-4 space-y-3 flex flex-col">
                        <label class="font-semibold">Jumlah Dana Disetujui:</label>
                        <!-- <div class="flex justify-center space-x-2">
                            <span class="text-gray-700">{{ 0 }}</span>
                            <input v-model="approveAmount" type="range"
                                :max="Math.min(selectedReimbursement.current_Limit, selectedReimbursement.amount)"
                                class="range range-info w-2/3" />
                            <span class="text-gray-700">{{ Math.min(selectedReimbursement.current_Limit,
                                selectedReimbursement.amount) }}</span>
                        </div> -->
                        <input
                            type="text"
                            readonly
                            :value="formatCurrency(selectedReimbursement.amount)" 
                            placeholder="Masukkan Dana Disetujui"
                            class="input input-bordered border border-gray-600"
                        />

                        <p v-if="!isApproveAmountValid && showValidation" class="text-red-500 text-sm">
                            Jumlah harus lebih besar dari 0 dan tidak melebihi {{
                                formatCurrency(Math.min(selectedReimbursement.current_Limit, selectedReimbursement.amount))
                            }}.
                        </p>

                        <label class="font-semibold">Catatan:</label>
                        <textarea v-model="note" class="textarea textarea-bordered w-full border border-gray-600"
                            placeholder="Masukkan catatan"></textarea>
                        <p v-if="!isNoteValid && showValidation" class="text-red-500 text-sm">
                            Catatan harus diisi.
                        </p>
                        <button class="btn bg-blue-500 mt-2 w-full  hover:bg-blue-900 hover:text-white text-white"
                            @click="submitApproval">
                            Submit Approval
                        </button>
                    </div>

                    <div v-if="showDeclineForm" class="mt-4 space-y-3">
                        <label class="font-semibold">Catatan:</label>
                        <textarea v-model="note" class="textarea textarea-bordered w-full border border-gray-600"
                            placeholder="Masukkan alasan penolakan"></textarea>
                        <p v-if="!isNoteValid && showValidation" class="text-red-500 text-sm">
                            Catatan harus diisi.
                        </p>
                        <button class="btn bg-red-500 mt-2 w-full  hover:bg-red-900 hover:text-white text-white" @click="submitDecline">
                            Submit Decline
                        </button>
                    </div>
                </div>
                <div class="bg-white sticky bottom-0 p-3">
                </div>
            </div>
            <form method="dialog" class="modal-backdrop">
                <button>close</button>
            </form>
        </dialog>
    </MainLayout>
</template>

<script>
import axios from 'axios';
import MainLayout from '../../layouts/MainLayout.vue';
import Swal from 'sweetalert2';
import { toast } from 'vue3-toastify';
import 'vue3-toastify/dist/index.css';

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
            showValidation: false,

            formattedAmount: '', // Untuk menampilkan angka terformat
        };
    },
    computed: {
        filteredData() {
            // Filter data based on search query
            return this.reimbursements
                .filter(
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
                )
                .filter(
                    item => item.name !== this.account_Name
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

        // validasi
        isApproveAmountValid() {
            return this.approveAmount > 0 &&
            this.approveAmount <= Math.min(this.selectedReimbursement.current_Limit, 
            this.selectedReimbursement.amount);
        },
        isNoteValid() {
            return this.note.trim().length > 0;
        },
        isApproveFormValid() {
            return this.isApproveAmountValid && this.isNoteValid;
        },
        isDeclineFormValid() {
            return this.isNoteValid;
        }
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
                    // toast.error(error.response.data.message, {
                    //         autoClose: 1000,
                    //     });
                });
        },
        openModal(item) {
            this.showApproveForm = false;
            this.showDeclineForm = false;
            this.approveAmount = item.approve_Amount || item.amount; // Gunakan dana yang disetujui oleh HR atau total dana diajukan
            this.note = '';
            this.formattedAmount = this.formatCurrency(this.approveAmount);

            this.selectedReimbursement = item; // set the selected reimbursement
            this.$refs.reimbursementModal.showModal(); // open the modal
        },
        formatCurrency(value) {
            if (!value) return '0';
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        },
        formatAmount() {
            // Hapus semua titik (pemformatan lama)
            const rawValue = this.formattedAmount.replace(/\./g, '');
            if (isNaN(rawValue)) {
                this.formattedAmount = '';
                this.approveAmount = 0;
                return;
            }

            // Simpan nilai asli tanpa format
            this.approveAmount = parseInt(rawValue, 10);

            // Tambahkan titik pemisah ribuan
            this.formattedAmount = this.approveAmount
                .toString()
                .replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        },
        openApproveForm() {
            this.showValidation = false;
            this.showApproveForm = true;
            this.showDeclineForm = false;
            this.formattedAmount = ''
            this.amount = 0
            this.note = ''
        },
        openDeclineForm() {
            this.showValidation = false;
            this.showDeclineForm = true;
            this.showApproveForm = false;
            this.formattedAmount = ''
            this.amount = 0
            this.note = ''
        },
        submitApproval() {
            this.showValidation = true;

            this.$refs.reimbursementModal.close();
            Swal.fire({
                title: "Are you sure?",
                text: "You will approve this reimbursement!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes!",
                customClass: {
                    popup: 'z-[99999999]' // Tambahkan kelas dengan z-index tinggi
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    const approvalData = {
                        approve_Amount: this.selectedReimbursement.amount, // Gunakan nilai pengajuan langsung
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
                            // Refresh data
                            this.fetchReimbursements();
                            toast.success(response.data.message, {
                                autoClose: 1000,
                            });
                        })
                        .catch((error) => {
                            console.error('Error approving reimbursement:', error);
                            toast.error(error.response.data.message, {
                                autoClose: 1000,
                            });
                        });
                }
            });
        },

        //     this.showValidation = true;
        //     if (!this.isApproveFormValid) {
        //         return
        //     }

        //     this.$refs.reimbursementModal.close();
        //     Swal.fire({
        //         title: "Are you sure?",
        //         text: "You will approve this reimbursement!",
        //         icon: "warning",
        //         showCancelButton: true,
        //         confirmButtonColor: "#3085d6",
        //         cancelButtonColor: "#d33",
        //         confirmButtonText: "Yes!",
        //         customClass: {
        //             popup: 'z-[99999999]' // Tambahkan kelas dengan z-index tinggi
        //         }
        //     }).then((result) => {
        //         if (result.isConfirmed) {
        //             const approvalData = {
        //                 approve_Amount: this.approveAmount,
        //                 note: this.note,
        //                 id_Reimbursement: this.selectedReimbursement.id_Reimbursement,
        //             };

        //             axios
        //                 .put(
        //                     `https://localhost:7102/api/Reimbursement/approvefinance/${this.selectedReimbursement.id_Reimbursement}`,
        //                     approvalData,
        //                     {
        //                         headers: {
        //                             Authorization: 'Bearer ' + localStorage.getItem('token'),
        //                         },
        //                     }
        //                 )
        //                 .then((response) => {
        //                     // Handle success
        //                     this.fetchReimbursements(); // refresh the data
        //                     this.$refs.reimbursementModal.close(); // close modal
        //                     // Swal.fire({
        //                     //     position: 'center',
        //                     //     icon: 'success',
        //                     //     title: 'Reimbursement Diterima',
        //                     //     showConfirmButton: false,
        //                     //     timer: 1500,
        //                     // });
        //                     toast.success(response.data.message, {
        //                         autoClose: 1000,
        //                     });
        //                 })
        //                 .catch((error) => {
        //                     // Handle error
        //                     console.error('Error approving reimbursement:', error);
        //                     toast.error(error.response.data.message, {
        //                         autoClose: 1000,
        //                     });
        //                 });
        //         }
        //     });

        // },
        submitDecline() {
            this.showValidation = true
            if (!this.isDeclineFormValid) {
                return
            }

            this.$refs.reimbursementModal.close();
            Swal.fire({
                title: "Are you sure?",
                text: "You will approve this reimbursement!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes!",
                customClass: {
                    popup: 'z-[99999999]' // Tambahkan kelas dengan z-index tinggi
                }
            }).then((result) => {
                if (result.isConfirmed) {
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
                            // Swal.fire({
                            //     position: 'center',
                            //     icon: 'success',
                            //     title: 'Reimbursement Ditolak',
                            //     showConfirmButton: false,
                            //     timer: 1500,
                            // });
                            toast.success(response.data.message, {
                                autoClose: 1000,
                            });
                        })
                        .catch((error) => {
                            // Handle error
                            console.error('Error declining reimbursement:', error);
                            // Swal.fire({
                            //     position: 'center',
                            //     icon: 'error',
                            //     title: 'Operasi Gagal',
                            //     showConfirmButton: false,
                            //     timer: 1500,
                            // });
                            toast.error(error.response.data.message, {
                                autoClose: 1000,
                            });
                        });
                }
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
    border: 1px solid #000;
    /* Border hitam pada tabel */
}

.table th,
.table td {
    padding: 8px;
    text-align: center;
    border: 1px solid #000;
    /* Border hitam pada setiap sel */
}

.table th {
    background-color: #f8f8f8;
    font-weight: bold;
}

.table tr:nth-child(even) {
    background-color: #f2f2f2;
}

.table tr:nth-child(odd) {
    background-color: #ffffff;
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
    display: inline-block;
    /* Membuat elemen berbentuk inline-block */
    font-size: 0.75rem;
    /* Ukuran font kecil */
    font-weight: bold;
    /* Tidak bold */
    white-space: nowrap;
    /* Menghindari teks terpotong ke bawah */
    overflow: hidden;
    /* Menyembunyikan teks yang melebihi area */
    text-align: center;
    /* Memastikan teks selalu rata tengah */
}
</style>
