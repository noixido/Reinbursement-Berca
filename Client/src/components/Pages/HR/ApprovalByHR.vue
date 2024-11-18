<template>
  <MainLayout>
    <!-- Search Bar -->
    <h1 class="text-3xl font-bold mb-10">Approval Reimbursement for Human Resource</h1>

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

    <!-- Tabel -->
    <div class="overflow-x-auto">
      <table class="table w-full border-collapse">
        <thead>
          <tr>
            <th class="p-2 text-center font-bold">Nomor</th>
            <th class="p-2 text-center font-bold">ID Reimbursement</th>
            <th class="p-2 text-center font-bold">Kategori</th>
            <th class="p-2 text-center font-bold">Tanggal Pengajuan</th>
            <th class="p-2 text-center font-bold">Jumlah Dana</th>
            <th class="p-2 text-center font-bold">Status</th>
            <th class="p-2 text-center font-bold">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in paginatedData" :key="item.id_Reimbursement">
            <td class="p-2 text-center">{{ index + 1 }}</td>
            <td class="p-2 text-center">{{ item.id_Reimbursement }}</td>
            <td class="p-2 text-center">{{ item.category_Name }}</td>
            <td class="p-2 text-center">{{ new Date(item.submit_Date).toLocaleDateString('id-ID', { year: 'numeric', month: 'long', day: '2-digit' }) }}</td>
            <td class="p-2 text-center">Rp. {{ formatCurrency(item.amount) }}</td>
            <td>
                            <span :class="{
                                'badge badge-warning': item.status.includes('progress'),
                                'badge badge-success': item.status.includes('approved'),
                                'badge badge-error': item.status.includes('declined')
                            }" class="badge-status">
                                {{ item.status }}
                            </span>
                        </td>
            <td class="p-2 text-center">
              <button
                class="btn btn-info mr-2 bg-[#45aafd] focus:outline-none focus:ring-none text-white"
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

        <div class="border-4 border-yellow-500 rounded-lg p-4 mb-3">
          <!-- Tombol Approve dan Decline -->
          <div class="flex justify-center space-x-4">
            <button class="btn bg-blue-500" @click="openApproveForm">
              Approve
            </button>
            <button class="btn bg-red-500" @click="openDeclineForm">
              Decline
            </button>
          </div>

           <!-- Form Approve atau Decline -->
           <div v-if="showApproveForm" class="mt-4 space-y-3">
            <label class="font-semibold">Notes:</label>
            <textarea v-model="selectedReimbursement.note" class="textarea textarea-bordered w-full" placeholder="Masukkan catatan persetujuan"></textarea>
            <button class="btn bg-blue-500 mt-2 w-full" @click.prevent="approveReimbursement(selectedReimbursement.id_Reimbursement)">
              Submit Approval
            </button>
          </div>

          <div v-if="showDeclineForm" class="mt-4 space-y-3">
            <label class="font-semibold">Notes:</label>
            <textarea v-model="selectedReimbursement.note" class="textarea textarea-bordered w-full" placeholder="Masukkan alasan penolakan"></textarea>
            <button class="btn bg-red-500 mt-2 w-full" @click.prevent="declineReimbursement(selectedReimbursement.id_Reimbursement)">
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
import { toast } from 'vue3-toastify';
import 'vue3-toastify/dist/index.css';

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
          toast.error(error.response.data.message, {
                        autoClose: 1000,
                    });
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
        // alert("Catatan persetujuan harus diisi.");
        toast.error("Catatan persetujuan harus diisi.", {
                        autoClose: 1000,
                    });
        return;
      }
      if (confirm("Anda yakin ingin menyetujui reimbursement ini?")) {
        axios
          .put(`https://localhost:7102/api/Reimbursement/approvehr/${id}`, { note: this.selectedReimbursement.note }, {
            headers: {
              'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
          })
          .then((response) => {
            this.selectedReimbursement.status = 'Approved';
            // alert("Reimbursement telah disetujui!");
            toast.success(response.data.message, {
                        autoClose: 1000,
                    });
            this.$refs.reimbursementModal.close();
            this.fetchReimbursements();
          })
          .catch((error) => {
            console.error('Error approving reimbursement:', error);
            toast.error(error.response.data.message, {
                        autoClose: 1000,
                    });
          });
      }
    },
    declineReimbursement(id) {
      if (!this.selectedReimbursement.note) {
        // alert("Catatan penolakan harus diisi.");
        toast.error("Catatan penolakan harus diisi.", {
                        autoClose: 1000,
                    });
        return;
      }
      if (confirm("Anda yakin ingin menolak reimbursement ini?")) {
        axios
          .put(`https://localhost:7102/api/Reimbursement/declinehr/${id}`, { note: this.selectedReimbursement.note }, {
            headers: {
              'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
          })
          .then((response) => {
            this.selectedReimbursement.status = 'Rejected';
            // alert("Reimbursement telah ditolak!");
            toast.success(response.data.message, {
                        autoClose: 1000,
                    });
            this.$refs.reimbursementModal.close();
            this.fetchReimbursements();
          })
          .catch((error) => {
            console.error('Error declining reimbursement:', error);
            toast.error(error.response.data.message, {
                        autoClose: 1000,
                    });
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
.table tbody tr:nth-child(odd) {
    background-color: #f2f2f2; /* Light gray for odd rows */
  }

  .table tbody tr:nth-child(even) {
    background-color: #ffffff; /* White for even rows */
  }
  .badge-status {
    display: inline-block; /* Membuat elemen berbentuk inline-block */
    font-size: 0.75rem; /* Ukuran font kecil */
    font-weight: normal; /* Tidak bold */
    white-space: nowrap; /* Menghindari teks terpotong ke bawah */
    overflow: hidden; /* Menyembunyikan teks yang melebihi area */
    text-align: center; /* Memastikan teks selalu rata tengah */
}
/* Sesuaikan style sesuai kebutuhan */
</style>
