<template>
    <MainLayout>
        <!-- Search Bar -->
        <h1 class="text-center text-2xl font-bold mb-4">Approval Reimbursement for Finance</h1>
        <div class="mb-4 flex justify-end">
            <input v-model="searchQuery" type="text" placeholder="Cari data..."
                class="input input-bordered w-full max-w-xs" />
        </div>

        <!-- Table -->
        <table class="table table-zebra">
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
                <tr v-for="(item, index) in paginatedData" :key="item.id_Reimbursement">
                    <td>{{ index + 1 }}</td>
                    <td>{{ item.id_Reimbursement }}</td>
                    <td>{{ item.category_Name }}</td>
                    <td>{{ new Date(item.submit_Date).toLocaleDateString('id-ID', {
                        year: 'numeric', month: 'long', day:
                            '2-digit'
                    }) }}</td>
                    <td>Rp. {{ formatCurrency(item.amount) }}</td>
                    <td>Rp. {{ item.approve_Amount ? formatCurrency(item.approve_Amount) : "-" }}</td>
                    <td>
                        <span :class="{
                            'badge badge-warning': item.status.includes('progress'),
                            'badge badge-success': item.status === 'approved',
                            'badge badge-error': item.status.includes('declined')
                        }">
                            {{ item.status }}
                        </span>
                    </td>
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
            </tbody>
        </table>

        <!-- Pagination Controls -->
        <div id="kontrol" class="flex justify-center items-center mt-4">
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

                    <div class="flex justify-between">
                        <span class="font-semibold">📁 Kategori:</span>
                        <span>{{ selectedReimbursement.category_Name }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📅 Tanggal Pengajuan:</span>
                        <span>{{ new Date(selectedReimbursement.submit_Date).toLocaleDateString('id-ID', {
                            year:
                                'numeric', month: 'long', day: '2-digit'
                        }) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">💰 Jumlah Dana:</span>
                        <span>Rp {{ formatCurrency(selectedReimbursement.amount) }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">✔️ Dana Disetujui:</span>
                        <span>Rp. {{ selectedReimbursement.approve_Amount ?
                            formatCurrency(selectedReimbursement.approve_Amount) : "-" }}</span>
                    </div>

                    <div class="flex justify-between">
                        <span class="font-semibold">📈 Status:</span>
                        <span :class="{
                            'text-green-600 font-semibold': selectedReimbursement.status === 'Approved',
                            'text-red-600 font-semibold': selectedReimbursement.status === 'Rejected',
                            'text-yellow-600 font-semibold': selectedReimbursement.status === 'Pending'
                        }">
                            {{ selectedReimbursement.status }}
                        </span>
                    </div>

                    <div class="">
                        <div class="font-semibold">📝 Catatan:</div>
                        <div>{{ selectedReimbursement.note || "Tidak ada catatan" }}</div>
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
import MainLayout from '../../layouts/MainLayout.vue';

export default {
    components: {
        MainLayout
    },
    data(){
        const token = localStorage.getItem('token');
        const payload = JSON.parse(atob(token.split(".")[1]));
        const id_Account = payload.id_account;
        const account_Name = payload.name;
        return {
            reimbursements: [],
            searchQuery: '',
            account_Name,
            currentPage: 1,
            itemsPerPage: 10, // Adjust this to change the number of items per page
            selectedReimbursement: {}, // to hold the selected reimbursement details
        }
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
        paginatedData() {
            // Slice data for current page
            const start = (this.currentPage - 1) * this.itemsPerPage;
            const end = start + this.itemsPerPage;
            return this.filteredData.slice(start, end);
        }
    },
    methods: {
        fetchReimbursements() {
            axios
                .get("https://localhost:7102/api/Reimbursement/finance", {
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('token')
                    }
                })
                .then((response) => {
                    this.reimbursements = response.data.data ? response.data.data : [];
                })
                .catch((error) => {
                    console.error('Error fetching data:', error);
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
    mounted() {
        this.fetchReimbursements();
    },
    watch: {
        currentPage(value) {
            // Reset to last page if we go beyond
            if (value > this.totalPages) this.currentPage = this.totalPages;
        }
    }
}
</script>

<style lang="scss" scoped>

</style>