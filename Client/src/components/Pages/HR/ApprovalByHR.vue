<template>
  <MainLayout>
    <!-- Search Bar -->
    <h1 class="text-center text-2xl font-bold mb-4">Approval Reimbursement for Human Resource</h1>

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
        <input v-model="searchQuery" type="text" placeholder="Search data..." class="input input-bordered w-full max-w-xs" />
      </div>
    </div>

    <!-- Tabel -->
    <div class="overflow-x-auto">
      <table class="table w-full">
        <thead>
          <tr>
            <th>Nomor</th>
            <th>ID Reimbursement</th>
            <th>Category</th>
            <th>Submission Date</th>
            <th>Total Funds</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in paginatedData" :key="item.id_Reimbursement">
            <td>{{ index + 1 }}</td>
            <td>{{ item.id_Reimbursement }}</td>
            <td>{{ item.category_Name }}</td>
            <td>{{ new Date(item.submit_Date).toLocaleDateString('id-ID', { year: 'numeric', month: 'long', day: '2-digit' }) }}</td>
            <td>Rp. {{ formatCurrency(item.amount) }}</td>
            <td>
              <span :class="{
                  'badge badge-warning text-xs px-2 py-1 rounded-lg': item.status.includes('progress'),
                  'badge badge-success text-xs px-2 py-1 rounded-lg': item.status === 'approved',
                  'badge badge-error text-xs px-2 py-1 rounded-lg': item.status.includes('declined')
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
            <td colspan="8" class="text-center py-4 text-red-500">No data found</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Pagination Controls -->
    <div class="flex justify-between mt-5">
      <div class="mb-4">
        <span class="text-sm">
          Showing {{ startItem }} to {{ endItem }} of {{ filteredData.length }} entries
        </span>
      </div>
      <div class="join mt-4">
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
        <button class="join-item btn" @click="currentPage = totalPages" :disabled="currentPage >= totalPages">
          Last
        </button>
      </div>
    </div>

    <!-- Modal -->
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
              <span class="font-semibold">👤 User Name:</span>
              <span>{{ selectedReimbursement.name }}</span>
            </div>
            <div class="flex justify-between">
              <span class="font-semibold">💳 Current Limit:</span>
              <span>Rp. {{ formatCurrency(selectedReimbursement.current_Limit) }}</span>
            </div>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">📁 Category:</span>
            <span>{{ selectedReimbursement.category_Name }}</span>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">📅 Submission Date:</span>
            <span>{{ new Date(selectedReimbursement.submit_Date).toLocaleDateString('id-ID', { year: 'numeric', month: 'long', day: '2-digit' }) }}</span>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">💰 Total Funds:</span>
            <span>Rp {{ formatCurrency(selectedReimbursement.amount) }}</span>
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

          <div class="flex justify-between">
            <span class="font-semibold">📝 Notes:</span>
            <div class="md:max-w-md mt-2 md:mt-0 font-semibold whitespace-normal break-words">
              {{ selectedReimbursement.note || 'Tidak ada catatan' }}
            </div>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">📎 Evidence:</span>
            <a :href="`https://localhost:7102/api/file/` + selectedReimbursement.evidence" target="_blank" rel="noopener noreferrer" class="text-blue-500 hover:underline">
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
           <div v-if="showApproveForm" class="mt-4 space-y-3">
            <label class="font-semibold">Notes:</label>
            <textarea v-model="selectedReimbursement.note" class="textarea textarea-bordered w-full" placeholder="Masukkan catatan persetujuan"></textarea>
            <button class="btn btn-primary mt-2 w-full" @click.prevent="approveReimbursement(selectedReimbursement.id_Reimbursement)">
              Submit Approval
            </button>
          </div>

          <div v-if="showDeclineForm" class="mt-4 space-y-3">
            <label class="font-semibold">Notes:</label>
            <textarea v-model="selectedReimbursement.note" class="textarea textarea-bordered w-full" placeholder="Masukkan alasan penolakan"></textarea>
            <button class="btn btn-primary mt-2 w-full" @click.prevent="declineReimbursement(selectedReimbursement.id_Reimbursement)">
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

export default {
  components: {
    MainLayout
  },
  data() {
    // Mengambil token dari localStorage dan memeriksa validitasnya
    const token = localStorage.getItem('token');
    let account_Name = '';
    let id_Account = null;

    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      id_Account = payload.id_account;
      account_Name = payload.name;
    } catch (error) {
      console.error("Token tidak valid atau tidak ditemukan:", error);
    }

    return {
      reimbursements: [],
      searchQuery: '',
      account_Name,
      currentPage: 1,
      itemsPerPage: 10,
      selectedReimbursement: {},
      showApproveForm: false,
      showDeclineForm: false,
      note: ''
    };
  },
  computed: {
    filteredData() {
      return this.reimbursements.filter(item =>
        item.category_Name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        item.status.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        item.id_Reimbursement.toLowerCase().includes(this.searchQuery)
      );
    },
    paginatedData() {
      const start = (this.currentPage - 1) * this.itemsPerPage;
      const end = start + this.itemsPerPage;
      return this.filteredData.slice(start, end);
    },
    totalPages() {
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
    }
  },
  methods: {
    fetchReimbursements() {
      axios
        .get("https://localhost:7102/api/Reimbursement/hr", {
          headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token')
          }
        })
        .then((response) => {
          this.reimbursements = response.data.data || [];
        })
        .catch((error) => {
          console.error('Error fetching data:', error);
        });
    },
    openModal(item) {
      this.selectedReimbursement = item;
      this.showApproveForm = false;
      this.showDeclineForm = false;
      this.$refs.reimbursementModal.showModal();
    },
    openApproveForm() {
      this.showApproveForm = true;
      this.showDeclineForm = false;
    },
    openDeclineForm() {
      this.showApproveForm = false;
      this.showDeclineForm = true;
    },
    formatCurrency(value) {
      if (!value) return '0';
      return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    },
    approveReimbursement(id) {
      if (!this.selectedReimbursement.note) {
        alert("Catatan persetujuan harus diisi.");
        return;
      }
      if (confirm("Anda yakin ingin menyetujui reimbursement ini?")) {
        axios
          .put(`https://localhost:7102/api/Reimbursement/approvehr/${id}`, { note: this.selectedReimbursement.note }, {
            headers: {
              'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
          })
          .then(() => {
            this.selectedReimbursement.status = 'Approved';
            alert("Reimbursement telah disetujui!");
            this.$refs.reimbursementModal.close();
            this.fetchReimbursements();
          })
          .catch((error) => {
            console.error('Error approving reimbursement:', error);
          });
      }
    },
    declineReimbursement(id) {
      if (!this.selectedReimbursement.note) {
        alert("Catatan penolakan harus diisi.");
        return;
      }
      if (confirm("Anda yakin ingin menolak reimbursement ini?")) {
        axios
          .put(`https://localhost:7102/api/Reimbursement/declinehr/${id}`, { note: this.selectedReimbursement.note }, {
            headers: {
              'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
          })
          .then(() => {
            this.selectedReimbursement.status = 'Rejected';
            alert("Reimbursement telah ditolak!");
            this.$refs.reimbursementModal.close();
            this.fetchReimbursements();
          })
          .catch((error) => {
            console.error('Error declining reimbursement:', error);
          });
      }
    }
  },
  mounted() {
    this.fetchReimbursements();
  }
}
</script>

<style lang="scss" scoped>
/* Sesuaikan style sesuai kebutuhan */
</style>
