<template>
    <MainLayout>
        <div class="container mx-auto p-6">
    <h1 class="text-2xl font-bold mb-4">Approval by HR</h1>
    <div class="bg-base-200 p-4 rounded-lg shadow-md">
        <div class="bg-white p-4 rounded-lg shadow-sm mb-4">
        <h2 class="text-xl font-semibold text-gray-800">Permintaan Reimbursement</h2>
        <div class="overflow-x-auto mt-4">
            <table class="table-auto w-full">
            <thead>
                <tr>
                <th class="px-4 py-2">No</th>
                <th class="px-4 py-2">Nama Karyawan</th>
                <th class="px-4 py-2">Jumlah</th>
                <th class="px-4 py-2">Status</th>
                <th class="px-4 py-2">Aksi</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(request, index) in reimbursementRequests" :key="request.id">
                <td class="border px-4 py-2">{{ index + 1 }}</td>
                <td class="border px-4 py-2">{{ request.employeeName }}</td>
                <td class="border px-4 py-2">{{ request.amount | currency }}</td>
                  <td class="border px-4 py-2">{{ request.status }}</td>
                  <td class="border px-4 py-2">
                    <button @click="approveRequest(request.id)" class="btn btn-success btn-sm">Approve</button>
                    <button @click="declineRequest(request.id)" class="btn btn-error btn-sm ml-2">Decline</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
    </MainLayout>
  </template>
  
  <script>
import MainLayout from '../layouts/MainLayout.vue';

  export default {
    components: {
        MainLayout
    },
    data() {
      return {
        // Contoh data permintaan reimbursement
        reimbursementRequests: [
          { id: 1, employeeName: 'John Doe', amount: 5000000, status: 'Pending' },
          { id: 2, employeeName: 'Jane Smith', amount: 3000000, status: 'Pending' },
          { id: 3, employeeName: 'Samuel Lee', amount: 1000000, status: 'Pending' },
        ],
      };
    },
    methods: {
      approveRequest(id) {
        const request = this.reimbursementRequests.find((r) => r.id === id);
        if (request) request.status = 'Approved';
      },
      declineRequest(id) {
        const request = this.reimbursementRequests.find((r) => r.id === id);
        if (request) request.status = 'Declined';
      },
    },
  };
  </script>
  
  <style scoped>
  /* Tampilan tabel */
  .table-auto {
    width: 100%;
    border-collapse: collapse;
  }
  .table-auto th,
  .table-auto td {
    border: 1px solid #ddd;
    padding: 8px;
    text-align: left;
  }
  </style>
  