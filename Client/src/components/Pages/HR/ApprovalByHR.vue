<template>
  <MainLayout>
      <!-- Search Bar -->
      <h1 class="text-center text-2xl font-bold mb-4">Approval Reimbursement for Human Resouce</h1>

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

      <!-- Tabel -->
      <div class="overflow-x-auto">
        <table class="table w-full">
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
            <td>{{ item.note || '-' }}</td>
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
              <span>{{ new Date(selectedReimbursement.submit_Date).toLocaleDateString('id-ID', { year: 'numeric', month: 'long', day: '2-digit' }) }}</span>
            </div>

            <div class="flex justify-between">
              <span class="font-semibold">💰 Jumlah Dana:</span>
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

            <!-- Catatan Field: Moved below -->
            <div class="flex justify-between">
              <span class="font-semibold">📝 Catatan:</span>
            </div>
            <div class="flex justify-between">
              <textarea v-model="selectedReimbursement.note" class="input input-bordered w-full" placeholder="Tulis catatan..."></textarea>
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
    },
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
      approveReimbursement(id) {
        axios
          .put(`https://localhost:7102/api/Reimbursement/approvehr/${id}`, { note: this.selectedReimbursement.note }, {
            headers: {
              'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
          })
          .then((response) => {
            this.selectedReimbursement.status = 'Approved';
            alert("Reimbursement has been approved!");
            this.$refs.reimbursementModal.close();
            this.fetchReimbursements();
          })
          .catch((error) => {
            console.error('Error approving reimbursement:', error);
          });
      },

      declineReimbursement(id) {
        axios
          .put(`https://localhost:7102/api/Reimbursement/declinehr/${id}`, { note: this.selectedReimbursement.note }, {
            headers: {
              'Authorization': 'Bearer ' + localStorage.getItem('token')
            }
          })
          .then((response) => {
            this.selectedReimbursement.status = 'Rejected';
            alert("Reimbursement has been declined!");
            this.$refs.reimbursementModal.close();
            this.fetchReimbursements();
          })
          .catch((error) => {
            console.error('Error declining reimbursement:', error);
          });
      }
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