<template>
    <MainLayout>
        <h2 class="text-xl font-semibold text-gray-800">Reimbursement Requests</h2>
        <br>
        <div class="mb-4 flex items-center justify-between">
            <label class="flex items-center gap-2">
                <span>Show</span>
                <select v-model="entriesPerPage" class="border border-gray-300 rounded p-1">
                    <option v-for="count in [5, 10, 20, 50]" :key="count" :value="count">{{ count }}</option>
                </select>
                <span>entries</span>
            </label>

            <input 
                v-model="searchQuery" 
                type="text" 
                placeholder="Search..." 
                class="border border-gray-500 rounded p-1"
            />
        </div>

        <div class="overflow-x-auto">
            <table class="table">
                <thead>
                    <tr>
                        <th>ID Reimbursement</th>
                        <th>Category Name</th>
                        <th>Submit Date</th>
                        <th>Status</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    <tr 
                        v-for="request in paginatedRequests" 
                        :key="request.id"
                    >
                        <td>{{ request.id }}</td>
                        <td>{{ request.category }}</td>
                        <td>{{ request.submitDate }}</td>
                        <td>{{ request.status || 'Pending' }}</td>
                        <td>
                            <button 
                                @click="viewRequest(request.id)" 
                                class="btn btn-info btn-xs mr-2 bg-[#14306e] hover:bg-[#14306e] focus:outline-none focus:ring-none text-white"
                            >
                                👁️
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>

            <!-- Pagination Controls -->
            <div class="join mt-4">
                <button
                    class="join-item btn"
                    @click="currentPage = Math.max(1, currentPage - 1)"
                    :disabled="currentPage <= 1"
                >
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

                <button
                    v-if="showEllipsis"
                    class="join-item btn btn-disabled"
                >
                    ...
                </button>

                <button
                    class="join-item btn"
                    @click="currentPage = totalPages"
                    :disabled="currentPage >= totalPages"
                >
                    Last
                </button>
            </div>
        </div>

        <!-- Modal for Approve/Decline -->
        <div v-if="showModal" class="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
            <div class="bg-white p-6 rounded-lg max-w-lg w-full relative">
                <!-- Close button -->
                <button @click="closeModal" class="absolute top-2 right-2 text-2xl text-gray-500 hover:text-gray-800">×</button>

                <h3 class="text-lg font-semibold mb-4">Approval Details</h3>

                <!-- Scrollable content for PDF -->
                <div class="overflow-y-auto max-h-80 mb-4">
                    <iframe :src="selectedPdf" class="w-full h-64" frameborder="0"></iframe>
                </div>

                <!-- Current Limit -->
                <p><strong>Current Limit:</strong> {{ selectedRequest.limit | currency }}</p> <!-- Displaying the current limit -->

                <!-- Approve Amount input -->
                <div class="mb-4">
                    <label for="approveAmount" class="block text-sm font-bold">Approve Amount</label>
                    <input type="number" id="approveAmount" v-model="approveAmount" class="mt-1 p-2 w-full border border-gray-300 rounded-lg" />
                </div>

                <!-- Notes input -->
                <div class="mb-4">
                    <label for="note" class="block text-sm font-bold">Catatan Finance</label>
                    <textarea id="note" v-model="note" rows="4" class="mt-1 p-2 w-full border border-gray-300 rounded-lg"></textarea>
                </div>

                <!-- Approve and Decline buttons -->
                <div class="flex justify-between mt-4">
                    <button @click="approveRequest(selectedRequest)" class="btn btn-success w-1/2 mr-2">Approve</button>
                    <button @click="declineRequest(selectedRequest)" class="btn btn-danger w-1/2">Decline</button>
                </div>
            </div>
        </div>
    </MainLayout>
</template>

<script>
import MainLayout from '../../layouts/MainLayout.vue';

export default {
    components: {
        MainLayout
    },
    data() {
        return {
            reimbursementRequests: [
                { id: 1, category: 'Kesehatan', submitDate: '2024-10-01', status: 'Approved', amount: 500000, limit: 2000000, pdf: 'file1.pdf' },
                { id: 2, category: 'Perjalanan Dinas', submitDate: '2024-10-05', status: 'Declined', amount: 1000000, limit: 2000000, pdf: 'file2.pdf' },
                { id: 3, category: 'Alat Tulis Kantor', submitDate: '2024-10-10', status: 'Pending', amount: 300000, limit: 2000000, pdf: 'file3.pdf' },
            ],
            showModal: false,
            selectedPdf: '',
            selectedRequest: {},
            note: '',
            approveAmount: 0,
            searchQuery: '',
            entriesPerPage: 5,
            currentPage: 1,
        };
    },
    methods: {
        viewRequest(id) {
            const request = this.reimbursementRequests.find(r => r.id === id);
            if (request) {
                this.selectedPdf = `path/to/pdfs/${request.pdf}`; // Update with correct PDF path
                this.selectedRequest = request;
                this.showModal = true;  // Open modal
            }
        },
        closeModal() {
            this.showModal = false;  // Close modal
            this.selectedPdf = '';
            this.selectedRequest = {};
            this.note = '';
            this.approveAmount = 0;
        },
        approveRequest(request) {
            // Logic to approve request (e.g., API call or state update)
            console.log('Approved:', request.id, 'Amount:', this.approveAmount, 'Note:', this.note);
            this.closeModal();
        },
        declineRequest(request) {
            // Logic to decline request (e.g., API call or state update)
            console.log('Declined:', request.id);
            this.closeModal();
        }
    },
    computed: {
        filteredRequests() {
            return this.reimbursementRequests.filter(request => 
                request.category.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
                request.status.toLowerCase().includes(this.searchQuery.toLowerCase())
            );
        },
        paginatedRequests() {
            const start = (this.currentPage - 1) * this.entriesPerPage;
            return this.filteredRequests.slice(start, start + this.entriesPerPage);
        },
        totalPages() {
            return Math.ceil(this.filteredRequests.length / this.entriesPerPage);
        },
        pageNumbers() {
            const pages = [];
            if (this.totalPages <= 5) {
                // Show all pages if total pages are less than or equal to 5
                for (let i = 1; i <= this.totalPages; i++) {
                    pages.push(i);
                }
            } else {
                // Show up to 5 pages with ellipsis
                if (this.currentPage <= 3) {
                    pages.push(1, 2, 3, 4, 5);
                } else if (this.currentPage >= this.totalPages - 2) {
                    pages.push(this.totalPages - 4, this.totalPages - 3, this.totalPages - 2, this.totalPages - 1, this.totalPages);
                } else {
                    pages.push(this.currentPage - 2, this.currentPage - 1, this.currentPage, this.currentPage + 1, this.currentPage + 2);
                }
            }
            return pages;
        },
        showEllipsis() {
            return this.totalPages > 5 && (this.currentPage > 3 && this.currentPage < this.totalPages - 2);
        },
    },
    watch: {
        entriesPerPage() {
            this.currentPage = 1;
        },
        searchQuery() {
            this.currentPage = 1;
        }
    }
};
</script>

<style scoped>
/* Styles for table and modal */
</style>
