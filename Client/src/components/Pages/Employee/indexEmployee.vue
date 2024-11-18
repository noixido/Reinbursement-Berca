<template>
    <MainLayout>
        <div class="container mx-auto p-6">
            <h1 class="text-3xl font-semibold text-gray-800 mb-6">Dashboard Employee</h1>

            <div class="flex gap-6">
                <!-- Left Column (Cards Column) -->
                <div class="flex flex-col gap-6 w-1/3">
                    <!-- Max Saldo Card -->
                    <div class="bg-white p-6 rounded-lg shadow-md flex items-center">
                        <div class="w-1/4">
                            <i class="fas fa-wallet text-4xl text-blue-500"></i>
                        </div>
                        <div class="w-3/4 pl-4">
                            <h3 class="text-xl font-semibold">Max Saldo</h3>
                            <p class="text-2xl font-bold">Rp. {{ formatCurrency(maxSaldo) }},00</p>
                        </div>
                    </div>

                    <!-- Saldo Card -->
                    <div class="bg-white p-6 rounded-lg shadow-md flex items-center">
                        <div class="w-1/4">
                            <i class="fas fa-piggy-bank text-4xl text-purple-500"></i>
                        </div>
                        <div class="w-3/4 pl-4">
                            <h3 class="text-xl font-semibold">Current Saldo</h3>
                            <p class="text-2xl font-bold">Rp. {{ formatCurrency(Math.floor(accountDetail.current_Limit))
                                }},00</p>
                        </div>
                    </div>

                    <!-- Approved Card -->
                    <div class="bg-white p-6 rounded-lg shadow-md flex items-center">
                        <div class="w-1/4">
                            <i class="fas fa-thumbs-up text-4xl text-green-500"></i>
                        </div>
                        <div class="w-3/4 pl-4">
                            <h3 class="text-xl font-semibold">Approved Saldo</h3>
                            <p class="text-2xl font-bold">Rp. {{ formatCurrency(approvedAmountTotal) }},00</p>
                        </div>
                    </div>
                </div>

                <!-- Right Column (Reimbursement Card) -->
                <div class="w-2/3 bg-white p-6 rounded-lg shadow-md flex flex-col justify-center">
                    <div>
                        <h3 class="text-xl font-semibold text-center">Reimbursement Status</h3>
                        <h3 class="font-semibold mb-4 text-center">{{ startPeriode }} - {{ endPeriode }}</h3>
                    </div>

                    <!-- Approved Progress Bar -->
                    <div class="mb-4">
                        <div class="flex justify-between mb-1">
                            <span class="text-sm font-medium text-green-700">Approved</span>
                            <span class="text-sm font-medium text-green-700">{{ approvedCount }}</span>
                        </div>
                        <div class="w-full bg-gray-200 rounded-full h-4">
                            <div :style="{ width: approvedPercentage + '%' }" class="bg-green-500 h-4 rounded-full">
                            </div>
                        </div>
                    </div>

                    <!-- In Progress Progress Bar -->
                    <div class="mb-4">
                        <div class="flex justify-between mb-1">
                            <span class="text-sm font-medium text-yellow-700">In Progress</span>
                            <span class="text-sm font-medium text-yellow-700">{{ inProgressCount }}</span>
                        </div>
                        <div class="w-full bg-gray-200 rounded-full h-4">
                            <div :style="{ width: inProgressPercentage + '%' }" class="bg-yellow-500 h-4 rounded-full">
                            </div>
                        </div>
                    </div>

                    <!-- Declined Progress Bar -->
                    <div class="mb-4">
                        <div class="flex justify-between mb-1">
                            <span class="text-sm font-medium text-red-700">Declined</span>
                            <span class="text-sm font-medium text-red-700">{{ declinedCount }}</span>
                        </div>
                        <div class="w-full bg-gray-200 rounded-full h-4">
                            <div :style="{ width: declinedPercentage + '%' }" class="bg-red-500 h-4 rounded-full"></div>
                        </div>
                    </div>

                    <!-- Total Progress Bar -->
                    <div class="mb-4">
                        <div class="flex justify-between mb-1">
                            <span class="text-sm font-medium text-blue-700">Total Reimbursement</span>
                            <span class="text-sm font-medium text-blue-700">{{ totalReimbursements }}</span>
                        </div>
                        <div class="w-full bg-gray-200 rounded-full h-4">
                            <div class="bg-blue-500 h-4 rounded-full" :class="totalReimbursements ? 'w-full' : 'w-0'">
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

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
        const token = localStorage.getItem('token');
        const payload = JSON.parse(atob(token.split('.')[1]));
        const email_Account = payload.email
        const role_Account = payload.role
        const id_Account = payload.id_account;
        const name_Account = payload.name;

        // menghitung max saldo
        // date join - date now
        // ambil limit di title -> reimburse limit. title.title_Name = account.title_Name
        // lebih 1 tahun : ambil limit di title
        // kurang 1 tahun : prorate (bulan/12 * title.reimbursel_limit)
        return {
            id_Account,
            email_Account,
            accountDetail: {},
            reimbursements: [],
            titles: [],
            maxSaldo: 0,

            startPeriode: "",
            endPeriode: "",
        };
    },
    computed: {
        totalReimbursements() {
            return this.reimbursements.length;
        },
        inProgressCount() {
            return this.reimbursements.filter(item => item.status.toLowerCase().includes("progress")).length;
        },
        approvedCount() {
            return this.reimbursements.filter(item => item.status.toLowerCase().includes("approved")).length;
        },
        declinedCount() {
            return this.reimbursements.filter(item => item.status.toLowerCase().includes("declined")).length;
        },
        inProgressPercentage() {
            return (this.inProgressCount ? (this.inProgressCount / this.totalReimbursements) * 100 : 0).toFixed(2);
        },
        approvedPercentage() {
            return (this.approvedCount ? (this.approvedCount / this.totalReimbursements) * 100 : 0).toFixed(2);
        },
        declinedPercentage() {
            return (this.declinedCount ? (this.declinedCount / this.totalReimbursements) * 100 : 0).toFixed(2);
        },
        approvedAmountTotal() {
            return this.reimbursements
                .filter(item => item.status.toLowerCase().includes("approved"))
                .reduce((total, item) => total + (item.approve_Amount || 0), 0);
        }
    },
    methods: {
        async fetchAccountDetail() {
            await axios
                .get(`https://localhost:7102/api/Account/${this.email_Account}`, {
                    headers: {
                        Authorization: 'Bearer ' + localStorage.getItem('token'),
                    },
                })
                .then((response) => {
                    this.accountDetail = response.data.data ? response.data.data : {};
                    console.log(this.accountDetail)
                })
                .catch((error) => {
                    console.error('Error fetching data:', error);
                });
        },
        async fetchReimbursementsAccount() {
            // ambil tanggal join-> misal tanggal join 13
            // if now kurang dari 13, ambil periode 13 bulan sebelumnya - hari ini
            // if now lebih dari 13, ambil periode tanggal 13 bulan ini - hari ini

            const [day, month, year] = this.accountDetail.join_Date.split('-');
            const joinDate = new Date(year, month - 1, day);

            const today = new Date();
            const joinDay = joinDate.getDate();
            let startDate, endDate;

            if (today.getDate() < joinDay) {
                // Jika hari ini kurang dari tanggal join, ambil periode dari tanggal join bulan sebelumnya - hari ini
                startDate = new Date(today.getFullYear(), today.getMonth() - 1, joinDay);
            } else {
                // Jika hari ini lebih dari atau sama dengan tanggal join, ambil periode dari tanggal join bulan ini - hari ini
                startDate = new Date(today.getFullYear(), today.getMonth(), joinDay);
            }

            endDate = new Date(startDate.getFullYear(), startDate.getMonth() + 1, joinDay)

            await axios
                .get(`https://localhost:7102/api/reimbursement/account/${this.id_Account}`, {
                    headers: {
                        Authorization: 'Bearer ' + localStorage.getItem('token'),
                    },
                })
                .then((response) => {
                    // this.reimbursements = response.data.data ? response.data.data : [];

                    // Filter reimbursements berdasarkan periode
                    this.reimbursements = response.data.data.filter(item => {
                        const reimbursementDate = new Date(item.submit_Date); // Pastikan field `date` benar
                        return reimbursementDate >= startDate && reimbursementDate <= endDate;
                    });

                    this.startPeriode = new Date(startDate).toLocaleDateString('id-ID', {
                        year: 'numeric', month: 'long', day:
                            '2-digit'
                    })

                    this.startPeriode = new Date(startDate).toLocaleDateString('id-ID', {
                        year: 'numeric', month: 'long', day:
                            '2-digit'
                    })
                    this.endPeriode = new Date(endDate).toLocaleDateString('id-ID', {
                        year: 'numeric', month: 'long', day:
                            '2-digit'
                    })

                    console.log(this.reimbursements)
                })
                .catch((error) => {
                    this.reimbursements = [];
                    this.startPeriode = new Date(startDate).toLocaleDateString('id-ID', {
                        year: 'numeric', month: 'long', day:
                            '2-digit'
                    })
                    this.endPeriode = new Date(endDate).toLocaleDateString('id-ID', {
                        year: 'numeric', month: 'long', day:
                            '2-digit'
                    })
                    console.log(this.reimbursements)
                    console.error('Error fetching data:', error);
                });
        },
        async fetchTitles() {
            await axios
                .get(`https://localhost:7102/api/titles`, {
                    headers: {
                        Authorization: 'Bearer ' + localStorage.getItem('token'),
                    },
                })
                .then((response) => {
                    this.titles = response.data.data ? response.data.data : [];
                    console.log(this.titles)
                })
                .catch((error) => {
                    this.titles = [];
                    console.log(this.reimbursements)
                    console.error('Error fetching data:', error);
                });
        },
        calculateMaxSaldo() {
            const dateNow = new Date();

            const [day, month, year] = this.accountDetail.join_Date.split('-');
            const joinDate = new Date(year, month - 1, day);

            const monthsDifference = (dateNow.getFullYear() - joinDate.getFullYear()) * 12 + (dateNow.getMonth() - joinDate.getMonth());

            const reimburseLimit = this.titles.find(t => t.title_Name === this.accountDetail.title_Name).reimburse_Limit;

            if (monthsDifference >= 12) {
                this.maxSaldo = reimburseLimit;
            }

            else {
                this.maxSaldo = Math.floor((monthsDifference / 12) * reimburseLimit);
            }
        },
        formatCurrency(value) {
            if (!value) return '0';
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        },
    },
    async mounted() {
        try {
            await this.fetchAccountDetail(),

                await Promise.all([
                    this.fetchReimbursementsAccount(),
                    this.fetchTitles()
                ]);

            this.calculateMaxSaldo();
        } catch (error) {
            console.error("Error fetching data:", error);
        }
    },
}
</script>

<style lang="scss" scoped></style>