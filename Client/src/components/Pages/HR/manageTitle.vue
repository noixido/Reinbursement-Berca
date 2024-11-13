<template>
    <div>
      <MainLayout>
        <div class="overflow-x-auto">
          <h1 class="text-3xl font-bold mb-10">Manage Titles</h1>
  
          <div class="flex justify-between items-center mb-4">
            <div class="flex gap-5">
                <!-- Rows Per Page -->
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

                <div  class="flex items-center">
                    <button @click="openAddModal" class="btn btn-primary" title="Add Data">
                        <i class="fas fa-plus"></i>
                    </button>
                </div>

                <dialog ref="theModal" class="modal">
                    <div class="modal-box">
                        <form method="dialog">
                            <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">
                                ✕
                            </button>
                        </form>
                        <h3 class="text-lg font-bold">{{ isEditing ? "Edit" : "Add" }} Titles</h3>
                        <form id="titleForm" @submit.prevent="submitData" class="mt-5 flex flex-col gap-3">
                            <div class="flex flex-col form-control">
                                <label for="title" class="font-semibold ml-3 mb-2">Title Name</label>
                                <input
                                    type="text"
                                    id="title"
                                    v-model="title.title_Name"
                                    placeholder="Title Name"
                                    class="input input-bordered w-full"
                                    autofocus
                                />
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="reimburse_Limit" class="font-semibold ml-3 mb-2">Reimbursement Limit</label>
                                <input
                                    type="text"
                                    id="reimburse_Limit"
                                    v-model="title.reimburse_Limit"
                                    placeholder="Reimbursement Limit"
                                    class="input input-bordered w-full"
                                />
                            </div>
                            <button
                                type="submit"
                                :class="isEditing ? 'btn-warning' : 'btn-primary'"
                                class="btn btn-primary w-28"
                            >
                                {{ isEditing ? "Edit" : "Submit" }}
                            </button>
                        </form>
                    </div>
                    <form method="dialog" class="modal-backdrop">
                        <button>close</button>
                    </form>
                </dialog>

            </div>

            <!-- Live Search Input -->
            <div class="relative w-full max-w-xs">
              <input
                type="text"
                v-model="searchTerm"
                placeholder="Search titles..."
                class="bg-white text-black border border-gray-300 rounded p-2 w-full pr-8"
              />
              <!-- Clear button (shown when searchTerm is not empty) -->
              <button
                v-if="searchTerm"
                @click="clearSearch"
                class="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>
          </div>
  
          <!-- Table -->
          <table class="table">
            <!-- Table Head -->
            <thead>
              <tr>
                <th>Id Account</th>
                <th>Title Name</th>
                <th>Reimbursement Limit</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(title, index) in paginatedData" :key="title.id_Title">
                <td>{{ title.id_Title }}</td>
                <td>{{ title.title_Name }}</td>
                <td>{{ formatToRupiah(title.reimburse_Limit) }}</td>
                <td>
                  <div class="flex gap-5 justify-center">
                    <button class="btn btn-warning" @click="editData(title)" title="Edit Data">
                      <i class="fas fa-pencil-alt"></i>
                    </button>
                    <button
                      class="btn btn-error"
                      @click="deleteData(title.id_Title)"
                      title="Delete Data"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
              <!-- Show message when no data is found -->
              <tr v-if="filteredData.length === 0">
                <td colspan="11" class="text-center py-4 text-red-500">No data found</td>
              </tr>
            </tbody>
          </table>

          <div class="flex justify-between mt-5">
            <!-- Showing data info -->
          <div class="mb-4">
            <span class="text-sm">
              Showing {{ startItem }} to {{ endItem }} of {{ filteredData.length }} entries
            </span>
          </div>

          <!-- Pagination Controls (DaisyUI join component) -->
          <div class="join mt-4">
            <!-- Previous Page Button -->
            <button
              class="join-item btn"
              @click="currentPage = Math.max(1, currentPage - 1)"
              :disabled="currentPage <= 1"
            >
              Prev
            </button>
  
            <!-- Page Number Buttons -->
            <button
              v-for="page in pageNumbers"
              :key="page"
              class="join-item btn"
              @click="currentPage = page"
              :class="{'btn-active': currentPage === page}"
            >
              {{ page }}
            </button>
  
            <!-- Ellipsis Button for Middle Pages -->
            <button
              v-if="showEllipsis"
              class="join-item btn btn-disabled"
            >
              ...
            </button>
  
            <!-- Last Page Button -->
            <button
              class="join-item btn"
              @click="currentPage = totalPages"
              :disabled="currentPage >= totalPages"
            >
              Last
            </button>
          </div>
        </div>

        </div>
      </MainLayout>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  import MainLayout from "../../layouts/MainLayout.vue";
  
  export default {
    name: "manage-title",
    components: {
      MainLayout,
    },
    data() {
      return {
        titles: [],
        title: {
            id_Title: "",
            title_Name: "",
            reimburse_Limit: "",
        },
        searchTerm: "",
        itemsPerPage: 10,
        currentPage: 1,
        isEditing: false,
      };
    },
    setup() {
        const token = localStorage.getItem("token");
        return {
            token,
        };
    },
    computed: {
        filteredData() {
        return this.titles.filter((title) => {
            return (
            title.id_Title.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            title.title_Name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            title.reimburse_Limit.toString().toLowerCase().includes(this.searchTerm.toLowerCase()) 
          );
        });
      },
      paginatedData() {
        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        return this.filteredData.slice(startIndex, startIndex + this.itemsPerPage);
      },
      totalPages() {
        return Math.ceil(this.filteredData.length / this.itemsPerPage);
      },
      pageNumbers() {
        const pages = [];
        for (let i = 1; i <= this.totalPages; i++) {
          pages.push(i);
        }
        return pages;
      },
      showEllipsis() {
        return this.totalPages > 5 && (this.currentPage > 3 && this.currentPage < this.totalPages - 2);
      },
      startItem() {
        return (this.currentPage - 1) * this.itemsPerPage + 1;
      },
      endItem() {
        return Math.min(this.currentPage * this.itemsPerPage, this.filteredData.length);
      },
    },
    mounted() {
      this.fetchData();
    },
    methods: {
      fetchData() {
        const api = "https://localhost:7102/api/Titles";
        return axios
          .get(api, {
            headers: {
              Authorization: `Bearer ${this.token}`,
            },
          })
          .then((response) => {
            this.titles = response.data.data;
          })
          .catch((error) => {
            console.error(error.response?.data?.message || error.message);
          });
      },
      formatToRupiah(number) {
        return new Intl.NumberFormat("id-ID", {
          style: "currency",
          currency: "IDR",
        }).format(number);
      },
      clearSearch() {
        this.searchTerm = "";
      },
      openAddModal() {
        // alert("test");
        this.clearForm();
        this.isEditing = false;
        this.$refs.theModal.showModal();
      },
      clearForm() {
        this.title = { title_Name: "", reimburse_Limit: ""};
      },
      submitData(){
        if(this.isEditing){
            this.updateData();
        }else{
            this.addData();
        }
      },
      addData(){
        const api = "https://localhost:7102/api/Titles";
        return axios
            .post(api, this.title, {
                    headers: {
                        Authorization: `Bearer ${this.token}`,
                        "Content-Type": "application/json",
                    },
                })
                .then((response)=>{
                    this.$refs.theModal.close();
                    alert(response.data.message);
                    this.fetchData();
                })
                .catch((error) => {
                    console.error(error);
                    alert(error);
                });
      },
      getDataById(id){
        const api = "https://localhost:7102/api/Titles/" + id;
        return axios
            .get(api, {
                    headers: {
                        Authorization: `Bearer ${this.token}`,
                        "Content-Type": "application/json",
                    },
                })
                .then((response)=>{
                    return response.data.data;
                })
                .catch((error) => {
                    console.error(error);
                    alert(error);
                });
      },
      editData(title){
        this.isEditing = true;
        this.getDataById(title.id_Title).then((data)=>{
            this.title = data;
            this.$refs.theModal.showModal();
        });
      },
      updateData(){
        const api = "https://localhost:7102/api/Titles/" + this.title.id_Title;
        return axios
            .put(api, this.title, {
                    headers: {
                        Authorization: `Bearer ${this.token}`,
                        "Content-Type": "application/json",
                    },
                })
                .then((response)=>{
                    this.$refs.theModal.close();
                    alert(response.data.message);
                    this.fetchData();
                })
                .catch((error) => {
                    console.error(error);
                    alert(error);
                });
      },
      deleteData(id){
        const api = "https://localhost:7102/api/Titles/" + id;
        return axios
            .delete(api, {
                    headers: {
                        Authorization: `Bearer ${this.token}`,
                        "Content-Type": "application/json",
                    },
                })
                .then((response)=>{
                    alert(response.data.message);
                    if (
                        this.currentPage === this.totalPages &&
                        this.paginatedData.length === 1
                    ) {
                        // If true, go to the previous page
                        this.currentPage-1
                    }

                    this.fetchData();
                })
                .catch((error) => {
                    console.error(error);
                    alert(error);
                });
      }
    },
  };
  </script>
  
  <style scoped>

  </style>
  