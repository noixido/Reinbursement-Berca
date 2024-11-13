<template>
    <MainLayout>
        <div class="mb-4 flex justify-between">
            <div class="">
                <button class="btn btn-info" @click="addUniversity">
                    Tambah Data
                </button>
                <dialog id="my_modal_3" class="modal">
                    <div class="modal-box">
                        <form method="dialog">
                            <button class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2">✕</button>
                        </form>
                        <form @submit.prevent="submitUniversity" id="universityForm" class="m-3">
                            <div class="mb-4">
                                <input v-model="university.id" type="text" id="inputID" placeholder="ID Universitas"
                                    class="hidden">
                            </div>
                            <div class="mb-4">
                                <label for="inputNama" class="block text-gray-700 font-semibold mb-2">Nama
                                    Universitas</label>
                                <input name="inputNama" v-model="university.name" type="text" id="inputNama"
                                    placeholder="Nama Universitas"
                                    class="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-blue-500">
                            </div>
                            <div class="flex justify-end">
                                <button type="submit" class="btn" :class="isEditing ? 'btn-warning' : 'btn-primary'">
                                    {{ isEditing ? 'Edit' : 'Submit' }}
                                </button>
                            </div>
                        </form>
                    </div>
                </dialog>
            </div>

            <input v-model="searchQuery" @input="filterUniversities" type="text" placeholder="Cari Universitas"
                class="border border-gray-300 rounded-lg p-2 w-56 focus:outline-none focus:ring-2 focus:ring-blue-500">
        </div>

        <GenericTable
            :items="filteredUniversities"
            :fields="['id', 'name']"
            :headers="['ID', 'Name']"
            @edit="editUniversity"
            @delete="deleteUniversity"
        />
        
        <div class="pagination-controls mt-2">
            <button @click="prevPage" :disabled="pageNumber === 1" class="btn btn-sm">Previous</button>
            <span>Page {{ pageNumber }} of {{ totalPages }}</span>
            <button @click="nextPage" :disabled="pageNumber === totalPages" class="btn btn-sm">Next</button>
        </div>
    </MainLayout>
</template>

<script>
import axios from 'axios';
import MainLayout from '../layouts/MainLayout.vue';
import Swal from 'sweetalert2';
import GenericTable from '../tables/GenericTable.vue';

export default {
    components: {
        MainLayout,
        GenericTable
    },
    data() {
        return {
            universities: [],
            university: {
                id: '',
                name: ''
            },
            isEditing: false,
            searchQuery: '',
            pageNumber: 1,
            pageSize: 5,
            totalPages: 1,
            filteredUniversities: []
        };
    },
    mounted() {
        this.fetchUniversities();
    },
    methods: {
        fetchUniversities() {
            axios
                .get(`https://localhost:7180/universities/paginated?pageNumber=${this.pageNumber}&pageSize=${this.pageSize}`, {
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('userToken')
                    }
                })
                .then((response) => {
                    this.universities = response.data.data.data;
                    this.totalPages = response.data.data.totalPages;
                    this.filterUniversities();
                })
                .catch((error) => {
                    console.error('Error fetching data:', error);
                });
        },
        filterUniversities() {
            this.filteredUniversities = this.universities.filter(university => {
                return university.name.toLowerCase().includes(this.searchQuery.toLowerCase());
            });
        },
        addUniversity() {
            this.university.id = '';
            this.university.name = '';
            this.isEditing = false;
            document.getElementById('my_modal_3').showModal();
        },
        editUniversity(id) {
            const selectedUniversity = this.universities.find(university => university.id === id);
            if (selectedUniversity) {
                this.university.id = selectedUniversity.id;
                this.university.name = selectedUniversity.name;
                this.isEditing = true;
                document.getElementById('my_modal_3').showModal();
            }
        },
        submitUniversity() {
            if (!this.isEditing) {
                // add university
                axios
                    .post('https://localhost:7180/universities', this.university, {
                        headers: {
                            'Authorization': 'Bearer ' + localStorage.getItem('userToken'),
                            'Content-Type': 'application/json'
                        }
                    })
                    .then((response) => {
                        this.universities.push(response.data.data);
                        this.filterUniversities();
                        Swal.fire({
                            position: "center",
                            icon: "success",
                            title: response.data.message,
                            showConfirmButton: false,
                            timer: 1500
                        });
                        document.getElementById('my_modal_3').close();
                    })
                    .catch((error) => {
                        console.error('Error adding university:', error);
                    });
            } else {
                // edit university
                const updatedUniversity = {
                    id: this.university.id,
                    name: this.university.name
                };

                axios
                    .put(`https://localhost:7180/universities/${updatedUniversity.id}`, updatedUniversity, {
                        headers: {
                            'Authorization': 'Bearer ' + localStorage.getItem('userToken'),
                            'Content-Type': 'application/json'
                        }
                    })
                    .then((response) => {
                        const index = this.universities.findIndex(university => university.id === updatedUniversity.id);
                        if (index !== -1) {
                            this.universities.splice(index, 1, response.data.data);
                            this.filterUniversities();
                        }
                        Swal.fire({
                            position: "center",
                            icon: "success",
                            title: response.data.message,
                            showConfirmButton: false,
                            timer: 1500
                        });
                        document.getElementById('my_modal_3').close();
                    })
                    .catch((error) => {
                        console.error('Error updating university:', error);
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: error.response.data.message || "An error occurred while updating.",
                        });
                    });
            }
        },
        deleteUniversity(id) {
            Swal.fire({
                title: "Yakin mau hapus?",
                text: "Data akan dihapus permanen!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, Hapus!"
            }).then((result) => {
                if (result.isConfirmed) {
                    axios
                        .delete(`https://localhost:7180/universities/${id}`, {
                            headers: {
                                "Authorization": "Bearer " + localStorage.getItem("userToken")
                            }
                        })
                        .then((response) => {
                            // Remove the deleted university from the list
                            this.universities = this.universities.filter(university => university.id !== id);
                            this.filterUniversities();
                            Swal.fire({
                                title: "Deleted!",
                                text: response.data.message,
                                showConfirmButton: false,
                                icon: "success",
                                timer: 1500
                            });
                        })
                        .catch((error) => {
                            console.error('Error deleting university:', error);
                            Swal.fire({
                                title: "Error!",
                                text: "There was an error deleting the university.",
                                icon: "error"
                            });
                        });
                }
            });
        },
        prevPage() {
            if (this.pageNumber > 1) {
                this.pageNumber--;
                this.fetchUniversities();
            }
        },
        nextPage() {
            if (this.pageNumber < this.totalPages) {
                this.pageNumber++;
                this.fetchUniversities();
            }
        }
    }
}
</script>

<style lang="scss" scoped>
.table {
    width: 100%;
    border-collapse: collapse; // Ensures borders are merged for a cleaner look
}

.table th,
.table td {
    border: 1px solid #ccc; // Border color and style
    padding: 8px; // Add some padding for better spacing
    text-align: left; // Align text to the left
}

.table th {
    background-color: #f8f8f8; // Light background for headers
    font-weight: bold; // Bold font for headers
}

.table tr:nth-child(even) {
    background-color: #f2f2f2; // Zebra striping for better readability
}
</style>