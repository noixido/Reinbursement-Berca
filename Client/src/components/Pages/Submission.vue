<template>
    <MainLayout>
        <form @submit.prevent="submitForm">
            <div class="space-y-6">
                <div class="flex flex-wrap justify-center items-center gap-8 mt-6">
                    <template v-for="(form, index) in forms" :key="index">
                        <div class="border border-gray-300 p-6 rounded-lg shadow-sm w-full md:w-[48%]">
                            <div class="flex justify-between items-center mb-4">
                                <h2 class="text-lg font-semibold text-gray-800">Form {{ index + 1 }}</h2>
    
                                <button type="button" @click="removeForm(index)" class="text-red-500 hover:text-red-700">
                                    &#x2716;
                                </button>
                            </div>
    
                            <div class="mb-4">
                                <label for="inputCategory" class="block text-gray-700 font-semibold mb-2">Kategori</label>
                                <select v-model="form.id_Category"
                                    class="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500">
                                    <option value="" disabled>-- Pilih Kategori --</option>
                                    <option v-for="category in categories" :key="category.id_Category"
                                        :value="category.id_Category">
                                        {{ category.category_Name }}
                                    </option>
                                </select>
                                <p v-if="formErrors[index]?.category" class="text-red-500 text-xs mt-1">Kategori harus
                                    diisi.
                                </p>
                            </div>
    
                            <div class="mb-4">
                                <label for="inputEvidence" class="block text-gray-700 font-semibold mb-2">Evidence</label>
                                <input type="file" @change="handleFileUpload($event, index)"
                                    class="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500" />
                                <p v-if="formErrors[index]?.evidence" class="text-red-500 text-xs mt-1">Evidence harus
                                    diisi.
                                </p>
                            </div>
    
                            <div class="mb-4">
                                <label for="inputAmount" class="block text-gray-700 font-semibold mb-2">Amount</label>
                                <input v-model="form.amount" type="number" min="0"
                                    class="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500" />
                                <p v-if="formErrors[index]?.amount" class="text-red-500 text-xs mt-1">Amount harus diisi dan
                                    lebih dari 0</p>
                            </div>
                        </div>
                    </template>
                </div>

                <div class="flex justify-between mt-6">
                    <button type="button" @click="addForm"
                        class="w-1/2 mr-2 flex items-center justify-center rounded-md border border-gray-300 bg-white py-2 text-sm font-semibold text-gray-700 shadow-sm hover:bg-gray-50">
                        Add another form
                    </button>

                    <button type="submit"
                        class="w-1/2 ml-2 flex items-center justify-center rounded-md bg-indigo-600 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500">
                        Submit
                    </button>
                </div>
            </div>
        </form>
    </MainLayout>
</template>


<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import Swal from 'sweetalert2';
import router from '../../router';
import MainLayout from '../layouts/MainLayout.vue';

const token = localStorage.getItem('token');
const payload = JSON.parse(atob(token.split(".")[1]));
const localAccount = payload.id_account;

const forms = ref([{ id_Account: localAccount, id_Category: '', evidence: null, amount: 0 }]);
const categories = ref([]);
const formErrors = ref([]);  // Untuk menyimpan error per form


onMounted(async () => {
    try {
        const response = await axios.get('https://localhost:7102/api/Category', {
            headers: {
                'Authorization': `Bearer ${localStorage.getItem("token")}`
            }
        });
        categories.value = response.data.data;
    } catch (error) {
        console.error("Failed to fetch categories:", error);
    }
});

function addForm() {
    forms.value.push({ id_Account: localAccount, id_Category: '', evidence: null, amount: 0 });
}

function handleFileUpload(event, index) {
    const file = event.target.files[0];
    forms.value[index].evidence = file;
}

function removeForm(index) {
    forms.value.splice(index, 1);
}

function validateForm() {
    let valid = true;
    formErrors.value = [];  // Reset errors

    forms.value.forEach((form, index) => {
        const errors = { category: false, evidence: false, amount: false };
        if (!form.id_Category) {
            errors.category = true;
            valid = false;
        }
        if (!form.evidence) {
            errors.evidence = true;
            valid = false;
        }
        if (form.amount <= 0) {
            errors.amount = true;
            valid = false;
        }
        formErrors.value.push(errors);
    });

    return valid;
}

async function submitForm() {
    if (!validateForm()) {
        return;  // Hentikan submit jika ada validasi yang gagal
    }

    const formData = forms.value.map((form, index) => {
        return {
            ...form,
            submit_Date: new Date().toISOString(),
            // evidence: "ini evidence"
            index: index + 1  // Tambahkan indeks untuk identifikasi
        };
    });

    Swal.fire({
        title: 'Submitting Forms...',
        html: '<ul id="form-status-list"></ul>',
        showConfirmButton: false,
        allowOutsideClick: false,
        didOpen: () => {
            const statusList = document.getElementById("form-status-list");
            formData.forEach((_, index) => {
                const listItem = document.createElement("li");
                listItem.innerHTML = `Form ${index + 1} <span id="status-${index + 1}">⏳ Processing...</span>`;
                statusList.appendChild(listItem);
            });
        }
    });

    try {
        for (const data of formData) {
            try {
                const response = await axios.post('https://localhost:7102/api/reimbursement', data, {
                    headers: {
                        'Authorization': 'Bearer ' + localStorage.getItem('token'),
                        'Content-Type': 'multipart/form-data'
                    }
                });

                // Update status ke sukses untuk form ini
                document.getElementById(`status-${data.index}`).innerText = "✔️ Success";
            } catch (error) {
                console.log(error);
                document.getElementById(`status-${data.index}`).innerText = "❌ Failed";
            }
        }

        Swal.fire({
            position: "center",
            icon: "success",
            title: "All forms processed!",
            showConfirmButton: false,
            timer: 1500
        }).then(() => {
            router.push("/history");  // Mengarahkan ke halaman yang diinginkan setelah submit
        });

    } catch (error) {
        Swal.fire({
            position: "center",
            icon: "error",
            title: 'Submission failed',
            text: "An error occurred while submitting.",
            showConfirmButton: true,
        });
    }
}


</script>

<style scoped>
/* Custom styles */
</style>
