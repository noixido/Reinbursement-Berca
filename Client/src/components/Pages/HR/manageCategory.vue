<template>
    <div>
      <MainLayout>
        <div class="overflow-x-auto">
          <h1 class="text-3xl font-bold mb-10">Manage Reimbursement Category</h1>
  
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
                    <button @click="openAddModal" class="btn bg-[#45aafd]" title="Add Data">
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
                        <h3 class="text-lg font-bold">{{ isEditing ? "Edit" : "Add" }} Reimbursement Category</h3>
                        <form id="titleForm" @submit.prevent="submitData" class="mt-5 flex flex-col gap-3">
                            <div class="flex flex-col form-control">
                                <label for="title" class="font-semibold ml-3 mb-2">Reimbursement Category Name</label>
                                <input
                                    type="text"
                                    id="category"
                                    v-model="state.category.category_Name"
                                    placeholder="Reimbursement Category Name"
                                    class="input input-bordered w-full"
                                    autofocus
                                />
                                <span v-if="v$.category.category_Name.$error" class="text-sm text-red-500">{{ v$.category.category_Name.$errors[0].$message }}</span>
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
                placeholder="Search reimbursement categories..."
                class="input input-bordered w-full max-w-xs"
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
          <table class="table w-full border-collapse">
            <!-- Table Head -->
            <thead>
              <tr>
                <th class="p-2 text-center font-bold">Id Reimbursement Category</th>
                <th class="p-2 text-center font-bold">Reimbursement Category Name</th>
                <th class="p-2 text-center font-bold">Action</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(category, index) in paginatedData" :key="category.id_Category">
                <td class="p-2 text-center">{{ category.id_Category }}</td>
                <td class="p-2 text-center">{{ category.category_Name }}</td>
                <td class="p-2 text-center">
                  <div class="flex gap-5 justify-center">
                    <button class="btn btn-warning" @click="editData(category)" title="Edit Data">
                      <i class="fas fa-pencil-alt"></i>
                    </button>
                    <button
                      class="btn btn-error"
                      @click="deleteData(category.id_Category)"
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
  import useVuelidate from '@vuelidate/core';
  import { required } from '@vuelidate/validators';
  import { reactive, computed } from "vue";
  
  export default {
    name: "manage-reimbursement-category",
    components: {
      MainLayout,
    },
    data() {
      return {
        categories: [],
        searchTerm: "",
        itemsPerPage: 10,
        currentPage: 1,
        isEditing: false,
      };
    },
    setup() {
        const token = localStorage.getItem("token");

        const state = reactive({
          category: {
            id_Category: "",
            category_Name: "",
          },
        });

        const rules = computed(()=>{
          return {
            category: {
            category_Name: { required },
            },
          };
        });

        const v$ = useVuelidate(rules, state);

        return {
            token,
            state,
            v$,
        };
    },
    computed: {
        filteredData() {
            return this.categories.filter((category) => {
                return (
                    category.id_Category.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                    category.category_Name.toLowerCase().includes(this.searchTerm.toLowerCase())
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
        const api = "https://localhost:7102/api/Category";
        return axios
          .get(api, {
            headers: {
              Authorization: `Bearer ${this.token}`,
            },
          })
          .then((response) => {
            this.categories = response.data.data;
          })
          .catch((error) => {
            console.error(error.response?.data?.message || error.message);
          });
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
        this.state.category = { 
            id_Category: "",
            category_Name: "",
        };
        this.v$.$reset();
      },
      submitData(){
        if(this.isEditing){
            this.updateData();
        }else{
            this.addData();
        }
      },
      addData(){
        this.v$.$validate();

        if(this.v$.$error){
          console.error("fix the error first!");
          return;
        }

        const api = "https://localhost:7102/api/Category";
        return axios
            .post(api, this.state.category, {
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
        const api = "https://localhost:7102/api/Category/" + id;
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
      editData(category){
        this.isEditing = true;
        this.getDataById(category.id_Category).then((data)=>{
            this.state.category = data;
            this.$refs.theModal.showModal();
        });
      },
      updateData(){
        this.v$.$validate();

        if(this.v$.$error){
          console.error("fix the error first!");
          return;
        }

        const api = "https://localhost:7102/api/Category/" + this.state.category.id_Category;
        return axios
            .put(api, this.state.category, {
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
        const api = "https://localhost:7102/api/Category/" + id;
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
.table tbody tr:nth-child(odd) {
    background-color: #f2f2f2; /* Light gray for odd rows */
  }

  .table tbody tr:nth-child(even) {
    background-color: #ffffff; /* White for even rows */
  }
  </style>
  