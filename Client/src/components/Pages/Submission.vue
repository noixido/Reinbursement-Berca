<template>
    <MainLayout>
        <form @submit.prevent="submitForm">
            <div class="space-y-6">
                <div class="flex flex-wrap justify-center items-center gap-8 mt-6">
                    <template v-for="(form, index) in forms" :key="index">
                        <div class="border-2 border-gray-600 p-6 rounded-lg shadow-sm w-full md:w-[48%]">
                            <div class="flex justify-between items-center mb-4">
                                <h2 class="text-lg font-semibold text-gray-800">Form {{ index + 1 }}</h2>

                                <button v-if="forms.length > 1" type="button" @click="removeForm(index)" class="text-red-500 hover:text-red-700">
                                    &#x2716;
                                </button>
                            </div>

                            <div class="mb-4">
                                <label for="inputCategory"
                                    class="block text-gray-700 font-semibold mb-2">Category</label>
                                <select v-model="form.id_Category"
                                    class="border border-gray-600 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500">
                                    <option value="" disabled>== Select Category ==</option>
                                    <option v-for="category in categories" :key="category.id_Category"
                                        :value="category.id_Category">
                                        {{ category.category_Name }}
                                    </option>
                                </select>
                                <p v-if="formErrors[index]?.category" class="text-red-500 text- mt-1">Category is
                                    required!
                                </p>
                            </div>

                            <div class="mb-4">
                                <label for="inputEvidence"
                                    class="block text-gray-700 font-semibold mb-2">Evidence</label>
                                <input type="file" @change="handleFileUpload($event, index)"
                                    class="border border-gray-600 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500"
                                    accept=".pdf" />
                                <p class="text-gray-600 text-sm mt-1">
                                    Please upload a file in PDF format. (max size: 5 MB)
                                </p>
                                <p v-if="formErrors[index]?.evidence" class="text-red-500 text-sm mt-1">Evidence is
                                    required!
                                </p>
                                <p v-if="formErrors[index]?.evidenceFileType" class="text-red-500 text-sm mt-1">
                                    Only PDF files are allowed!
                                </p>
                                <p v-if="formErrors[index]?.evidenceFileSize" class="text-red-500 text-sm mt-1">File
                                    size must not exceed 5 MB!</p>

                            </div>

                            <div class="mb-4">
                                <label for="inputAmount" class="block text-gray-700 font-semibold mb-2">Amount</label>
                                <input v-model="forms[index].formattedAmount" @input="formatAmount(index)" type="text"
                                    inputmode="numeric"
                                    class="border border-gray-600 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500" />
                                <!-- <input v-model="form.amount" type="number" min="0"
                                    class="border border-gray-300 rounded-lg p-2 w-full focus:outline-none focus:ring-2 focus:ring-indigo-500" /> -->
                                <p v-if="formErrors[index]?.amount" class="text-red-500 text-sm mt-1">Amount is required
                                    and the value cannot be 0!</p>
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

function formatAmount(index) {
    // Hapus semua titik (pemformatan lama)
    const rawValue = forms.value[index].formattedAmount.replace(/\./g, '');
    if (isNaN(rawValue)) {
        forms.value[index].formattedAmount = '';
        forms.value[index].amount = 0;
        return;
    }

    // Simpan nilai asli tanpa format
    forms.value[index].amount = parseInt(rawValue, 10);

    // Tambahkan titik pemisah ribuan
    forms.value[index].formattedAmount = forms.value[index].amount
        .toString()
        .replace(/\B(?=(\d{3})+(?!\d))/g, '.');
}


function addForm() {
    formErrors.value = []
    // forms.value.push({ id_Account: localAccount, id_Category: '', evidence: null, amount: 0 });
    forms.value.push({
        id_Account: localAccount,
        id_Category: '',
        evidence: null,
        amount: 0,
        formattedAmount: '', // Untuk menampilkan angka terformat
    });
}

function handleFileUpload(event, index) {
    const file = event.target.files[0];
    if (file) {
        const maxSize = 5 * 1024 * 1024; // 5 MB in bytes
        const fileExtension = file.name.split('.').pop().toLowerCase();

        // Reset error state for file size and type
        formErrors.value[index] = { ...formErrors.value[index], evidenceFileType: false, evidenceFileSize: false };

        if (fileExtension !== 'pdf') {
            // Invalid file type
            formErrors.value[index].evidenceFileType = true;
            forms.value[index].evidence = null;
        } else if (file.size > maxSize) {
            // Invalid file size
            formErrors.value[index].evidenceFileSize = true;
            forms.value[index].evidence = null;
        } else {
            // Valid file
            forms.value[index].evidence = file;
        }
    }
}

function removeForm(index) {
    forms.value.splice(index, 1);
}

function validateForm() {
    let valid = true;
    formErrors.value = [];  // Reset errors

    forms.value.forEach((form, index) => {
        const errors = { category: false, evidence: false, evidenceFileType: false, amount: false };
        if (!form.id_Category) {
            errors.category = true;
            valid = false;
        }
        if (!form.evidence) {
            errors.evidence = true;
            valid = false;
        } else {
            // Validate evidence file type (only PDF allowed)
            const fileExtension = form.evidence.name.split('.').pop().toLowerCase();
            if (fileExtension !== 'pdf') {
                errors.evidenceFileType = true;
                valid = false;
            }
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
            amount: form.amount, // Kirim angka asli tanpa format
            submit_Date: new Date().toISOString(),
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
            title: "Reimbursement Submitted",
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
