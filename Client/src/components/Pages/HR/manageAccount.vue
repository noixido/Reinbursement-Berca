<template>
    <div>
      <MainLayout>
        <div class="overflow-x-auto">
          <h1 class="text-3xl font-bold mb-10">Manage Accounts</h1>
  
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
                        <h3 class="text-lg font-bold">{{ isEditing ? "Edit" : "Add" }} Account</h3>
                        <form @submit.prevent="submitAccount" id="accountForm" class="mt-5 flex flex-col gap-3">
                            <div class="flex flex-col form-control">
                                <label for="email" class="font-semibold ml-3 mb-2">Email</label>
                                <input
                                    type="text"
                                    id="email"
                                    v-model="state.account.email"
                                    placeholder="Email"
                                    class="input input-bordered w-full"
                                    autofocus
                                />
                                <span v-if="v$.account.email.$error" class="text-sm text-red-500">{{ v$.account.email.$errors[0].$message }}</span>
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="name" class="font-semibold ml-3 mb-2">Employee Name</label>
                                <input
                                    type="text"
                                    id="name"
                                    v-model="state.account.name"
                                    placeholder="Employee Name"
                                    class="input input-bordered w-full"
                                    autofocus
                                />
                                <span v-if="v$.account.name.$error" class="text-sm text-red-500">{{ v$.account.name.$errors[0].$message }}</span>
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="phone" class="font-semibold ml-3 mb-2">Phone</label>
                                <input
                                    type="text"
                                    id="phone"
                                    v-model="state.account.phone"
                                    placeholder="Phone"
                                    class="input input-bordered w-full"
                                    autofocus
                                />
                                <span v-if="v$.account.phone.$error" class="text-sm text-red-500">{{ v$.account.phone.$errors[0].$message }}</span>
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="gender" class="font-semibold ml-3 mb-2">Gender</label>
                                <select name="gender" id="gender" v-model="state.account.gender" class="select select-bordered w-full">
                                    <option value="" disabled>== Select Gender ==</option>
                                    <option value="Male">Male</option>
                                    <option value="Female">Female</option>
                                </select>
                                <span v-if="v$.account.gender.$error" class="text-sm text-red-500">{{ v$.account.gender.$errors[0].$message }}</span>
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="birth_Date" class="font-semibold ml-3 mb-2">Birth Date</label>
                                <input
                                    type="date"
                                    id="birth_Date"
                                    v-model="state.account.birth_Date"
                                    placeholder="Birth Date"
                                    class="input input-bordered w-full"
                                    autofocus
                                />
                                <span v-if="v$.account.birth_Date.$error" class="text-sm text-red-500">{{ v$.account.birth_Date.$errors[0].$message }}</span>
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="join_Date" class="font-semibold ml-3 mb-2">Join Date</label>
                                <input
                                    type="date"
                                    id="join_Date"
                                    v-model="state.account.join_Date"
                                    placeholder="Join Date"
                                    class="input input-bordered w-full"
                                    autofocus
                                />
                                <span v-if="v$.account.join_Date.$error" class="text-sm text-red-500">{{ v$.account.join_Date.$errors[0].$message }}</span>
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="title" class="font-semibold ml-3 mb-2">Title</label>
                                <select name="title" id="title" v-model="state.account.id_Title" class="select select-bordered w-full">
                                    <option value="" selected disabled>== Select Title ==</option>
                                    <option v-for="title in titles" :key="title.id_Title" :value="title.id_Title">{{ title.title_Name }}</option>
                                </select>
                                <span v-if="v$.account.id_Title.$error" class="text-sm text-red-500">{{ v$.account.id_Title.$errors[0].$message }}</span>
                            </div>
                            <div class="flex flex-col form-control">
                                <label for="role_Name" class="font-semibold ml-3 mb-2">Role</label>
                                <select name="role_Name" id="role_Name" v-model="state.account.role_Name" class="select select-bordered w-full">
                                    <option value="" selected disabled>== Select Role ==</option>
                                    <option value="Employee">Employee</option>
                                    <option value="HR">Human Resource Department</option>
                                    <option value="Finance">Finance</option>
                                </select>
                                <span v-if="v$.account.role_Name.$error" class="text-sm text-red-500">{{ v$.account.role_Name.$errors[0].$message }}</span>
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
                placeholder="Search accounts..."
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
                <th class="p-2 text-center font-bold">Id Account</th>
                <th class="p-2 text-center font-bold">Employee Name</th>
                <th class="p-2 text-center font-bold">Email</th>
                <th class="p-2 text-center font-bold">Phone</th>
                <th class="p-2 text-center font-bold">Gender</th>
                <th class="p-2 text-center font-bold">Birth Date</th>
                <th class="p-2 text-center font-bold">Join Date</th>
                <th class="p-2 text-center font-bold">Title</th>
                <th class="p-2 text-center font-bold">Reimbursement Limit</th>
                <th class="p-2 text-center font-bold">Role</th>
                <th class="p-2 text-center font-bold">Action</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(account, index) in paginatedAccounts" :key="account.id_Account">
                <td class="p-2 text-center">{{ account.id_Account }}</td>
                <td class="p-2 text-center">{{ account.name }}</td>
                <td class="p-2 text-center">{{ account.email }}</td>
                <td class="p-2 text-center">{{ account.phone }}</td>
                <td class="p-2 text-center">{{ account.gender }}</td>
                <td class="p-2 text-center">{{ account.birth_Date }}</td>
                <td class="p-2 text-center">{{ account.join_Date }}</td>
                <td class="p-2 text-center">{{ account.title_Name }}</td>
                <td class="p-2 text-center">{{ formatToRupiah(account.current_Limit) }}</td>
                <td class="p-2 text-center">{{ account.role_Name }}</td>
                <td class="p-2 text-center">
                  <div class="flex gap-5 justify-center">
                    <button class="btn btn-warning" @click="editAccount(account)" title="Edit Data">
                      <i class="fas fa-pencil-alt"></i>
                    </button>
                    <button
                      class="btn btn-error"
                      @click="deleteAccount(account.email)"
                      title="Delete Data"
                    >
                      <i class="fas fa-trash"></i>
                    </button>
                  </div>
                </td>
              </tr>
              <!-- Show message when no data is found -->
              <tr v-if="filteredAccounts.length === 0">
                <td colspan="11" class="text-center py-4 text-red-500">No data found</td>
              </tr>
            </tbody>
          </table>

          <div class="flex justify-between mt-5">
            <!-- Showing data info -->
          <div class="mb-4">
            <span class="text-sm">
              Showing {{ startItem }} to {{ endItem }} of {{ filteredAccounts.length }} entries
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
  import { required, email } from '@vuelidate/validators';
  import { reactive, computed } from "vue";
  
  export default {
    name: "manage-account",
    components: {
      MainLayout,
    },
    data() {
      return {
        accounts: [],
        titles: [],
        searchTerm: "",
        itemsPerPage: 10,
        currentPage: 1,
        isEditing: false,
      };
    },
    setup() {
        const token = localStorage.getItem("token");

        const state = reactive({
          account: {
            email: "",
            name: "",
            phone: "",
            gender: "",
            birth_Date: "",
            join_Date: "",
            id_Title: "",
            role_Name: "",
          },
        });

        const rules = computed(()=>{
          return {
            account: {
            email: { required, email },
            name: { required },
            phone: { required },
            gender: { required },
            birth_Date: { required },
            join_Date: { required },
            id_Title: { required },
            role_Name: { required },
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
      filteredAccounts() {
        return this.accounts.filter((account) => {
            return (
            account.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.email.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.phone.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.id_Account.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.gender.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.birth_Date.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.join_Date.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.title_Name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.current_Limit.toString().toLowerCase().includes(this.searchTerm.toLowerCase()) ||
            account.role_Name.toLowerCase().includes(this.searchTerm.toLowerCase())
          );
        });
      },
      paginatedAccounts() {
        const startIndex = (this.currentPage - 1) * this.itemsPerPage;
        return this.filteredAccounts.slice(startIndex, startIndex + this.itemsPerPage);
      },
      totalPages() {
        return Math.ceil(this.filteredAccounts.length / this.itemsPerPage);
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
        return Math.min(this.currentPage * this.itemsPerPage, this.filteredAccounts.length);
      },
    },
    mounted() {
      this.fetchData();
      this.fetchTitle();
    },
    methods: {
      fetchData() {
        const api = "https://localhost:7102/api/Account";
        return axios
          .get(api, {
            headers: {
              Authorization: `Bearer ${this.token}`,
            },
          })
          .then((response) => {
            this.accounts = response.data.data;
          })
          .catch((error) => {
            console.error(error.response?.data?.message || error.message);
          });
      },
      fetchTitle(){
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
      this.state.account = { 
        email: "",
        name: "",
        phone: "",
        gender: "",
        birth_Date: "",
        join_Date: "",
        id_Title: "",
        role_Name: "",
      };
      this.v$.$reset();
    },
      submitAccount(){
        if (this.isEditing) {
            this.updateAccount();
        } else {
            this.addAccount();
        }
      },
      addAccount() {
            this.v$.$validate();
            
            if(this.v$.$error){
              console.error("fix the error first!");
              return;
            }

            const api = "https://localhost:7102/api/Account";
            
            // Ensure date format (if required by your API)
            if (this.state.account.birth_Date) {
              this.state.account.birth_Date = new Date(this.state.account.birth_Date).toISOString().split("T")[0];
            }
            if (this.state.account.join_Date) {
              this.state.account.join_Date = new Date(this.state.account.join_Date).toISOString().split("T")[0];
            }

            return axios
                .post(api, this.state.account, {
                    headers: {
                        Authorization: `Bearer ${this.token}`,
                        "Content-Type": "application/json",
                    },
                })
                .then((response) => {
                    this.$refs.theModal.close();
                    alert(response.data.message);
                    this.fetchData();
                })
                .catch((error) => {
                    console.log(error)
                    console.error(error.response?.data?.message || error.message);
                    alert(error.response?.data?.message || "An error occurred.");
                });
        },
      getAccountByEmail(emailParams){
        const api = "https://localhost:7102/api/Account/getAccountByEmailForUpdate/" + emailParams;
        return axios
            .get(api, {
                headers: {
                    Authorization: `Bearer ${this.token}`,
                    "Content-Type": "application/json",
                },
            })
            .then((response)=>{
                const data = response.data.data;

                this.state.account.email = data.email;
                this.state.account.name = data.name;
                this.state.account.phone = data.phone;
                this.state.account.gender = data.gender;
                this.state.account.birth_Date = this.formatDate(data.birth_Date);
                this.state.account.join_Date = this.formatDate(data.join_Date);
                this.state.account.id_Title = data.id_Title;
                this.state.account.role_Name = data.role_Name;

                return this.state.account;
            })
            .catch((error)=>{
                console.error(error);
            });
      },
      editAccount(account) {
        this.isEditing = true;
        this.getAccountByEmail(account.email).then((data)=>{
            this.account = data;
            this.$refs.theModal.showModal();
        });
      },
      updateAccount(){
        const api = "https://localhost:7102/api/Account/" + this.account.email;
        return axios
          .put(api, this.account, {
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
      deleteAccount(email) {
        const api = "https://localhost:7102/api/Account/Delete/" + email;
        return axios
          .put(api, {}, {
            headers: {
              Authorization: `Bearer ${this.token}`,
              "Content-Type": "application/json",
            },
          })
          .then((response)=>{
            this.$refs.theModal.close();
            alert(response.data.message);        
            if (
              this.currentPage === this.totalPages && 
              this.paginatedData &&
              this.paginatedData.length === 1
            ) {
              // If true, go to the previous page
              this.currentPage-1
            }

            this.fetchData();
          })
          .catch((error)=>{
            console.error(error);
            alert(error);
          })
      },
      formatDate(dateStr) {
        const [day, month, year] = dateStr.split('-');
        return `${year}-${month}-${day}`; // Convert to YYYY-MM-DD
      },
    },
  };
  </script>
  
  <style scoped>
  /* Additional styling if needed */
  .table tbody tr:nth-child(odd) {
    background-color: #f2f2f2; /* Light gray for odd rows */
  }

  .table tbody tr:nth-child(even) {
    background-color: #ffffff; /* White for even rows */
  }
  </style>
