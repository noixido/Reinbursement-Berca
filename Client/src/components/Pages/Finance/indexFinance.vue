<template>
    <MainLayout>
        <div class="container mx-auto p-6">
            <h1 class="text-3xl font-semibold text-gray-800 mb-6">Dashboard Finance</h1>

            <!-- Dashboard Cards -->
            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                <!-- Total Reimbursement Requests -->
                <div class="bg-white p-6 rounded-lg shadow-md flex items-center hover:shadow-xl transition duration-300">
                    <div class="w-1/4">
                        <i class="fas fa-file-invoice text-4xl text-blue-500"></i>
                    </div>
                    <div class="w-3/4 pl-4">
                        <h3 class="text-xl font-semibold text-black">Total Requests</h3>
                        <p class="text-2xl font-bold text-blue-900">{{ totalRequests }}</p>
                    </div>
                </div>

                <!-- Approved Requests -->
                <div class="bg-white p-6 rounded-lg shadow-md flex items-center hover:shadow-xl transition duration-300">
                    <div class="w-1/4">
                        <i class="fas fa-check-circle text-4xl text-green-500"></i>
                    </div>
                    <div class="w-3/4 pl-4">
                        <h3 class="text-xl font-semibold text-black">Approved Status</h3>
                        <p class="text-2xl font-bold text-green-900">{{ approvedRequests }}</p>
                    </div>
                </div>

                <!-- Declined Requests -->
                <div class="bg-white p-6 rounded-lg shadow-md flex items-center hover:shadow-xl transition duration-300">
                    <div class="w-1/4">
                        <i class="fas fa-times-circle text-4xl text-red-500"></i>
                    </div>
                    <div class="w-3/4 pl-4">
                        <h3 class="text-xl font-semibold text-black">Declined Requests</h3>
                        <p class="text-2xl font-bold text-red-900">{{ declinedRequests }}</p>
                    </div>
                </div>

                <!-- Total Dana Bulan Ini -->
                <div class="bg-white p-6 rounded-lg shadow-md flex items-center hover:shadow-xl transition duration-300">
                    <div class="w-1/4">
                        <i class="fas fa-coins text-4xl text-yellow-500"></i>
                    </div>
                    <div class="w-3/4 pl-4">
                        <h3 class="text-xl font-semibold text-black">Approved Saldo</h3>
                        <p class="text-2xl font-bold text-black">{{ formatToRupiah(totalDanaBulanIni) }}</p>
                    </div>
                </div>
            </div>

            <!-- Chart Section -->
            <!-- <div class="mt-8 grid grid-cols-1 lg:grid-cols-2 gap-8">
                <div class="bg-white p-6 rounded-lg shadow-lg">
                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Jumlah Dana yang Dikeluarkan per Bulan</h3>
                    <line-chart :data="danaPerBulanData" :options="lineChartOptions"></line-chart>
                </div>
            </div> -->
        </div>
    </MainLayout>
</template>

<script>
import { defineComponent, ref, onMounted } from 'vue';
import axios from 'axios';
import { Line } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement } from 'chart.js';
import MainLayout from '../../layouts/MainLayout.vue';

ChartJS.register(Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement);

export default defineComponent({
    components: {
        'line-chart': Line,
        MainLayout
    },
    setup() {
        // Data Properties for Dashboard Cards
        const totalRequests = ref(0);
        const totalDanaBulanIni = ref(0);
        const approvedRequests = ref(0);
        const declinedRequests = ref(0);

        // Chart Data for Dana per Month
        const danaPerBulanData = ref({
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            datasets: [
                {
                    label: 'Dana Dikeluarkan (Rp)',
                    data: [1000000, 1500000, 2000000, 2500000, 3000000, 3500000, 4000000, 4500000, 5000000, 5500000, 6000000, 6500000],
                    borderColor: '#3b82f6',
                    backgroundColor: 'rgba(59, 130, 246, 0.2)',
                    fill: true,
                    tension: 0.4,
                },
            ],
        });

        // Chart Options
        const lineChartOptions = ref({
            responsive: true,
            plugins: {
                legend: {
                    display: true,
                    position: 'top',
                },
                tooltip: {
                    callbacks: {
                        label: (context) => `Rp ${context.raw.toLocaleString()}`,
                    },
                },
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: (value) => `Rp ${value.toLocaleString()}`,
                    },
                },
            },
        });

        // Helper function to format numbers to Rupiah
        const formatToRupiah = (value) => {
            return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR' }).format(value);
        };

        // Fetch Data from API
        const fetchData = async () => {
            try {
                const response = await axios.get('https://localhost:7102/api/Reimbursement', {
                    headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
                });

                const reimbursements = response.data.data;

                // Total requests
                totalRequests.value = reimbursements.length;

                // Total approved requests
                approvedRequests.value = reimbursements.filter(
                    (item) => item.status.toLowerCase() === 'approved'
                ).length;

                // Total declined requests
                declinedRequests.value = reimbursements.filter(
                    (item) => item.status.toLowerCase().includes('decline')
                ).length;

                // Total dana yang diapprove bulan ini
                const currentMonth = new Date().getMonth();
                const currentYear = new Date().getFullYear();
                totalDanaBulanIni.value = reimbursements.reduce((total, item) => {
                    const submitDate = new Date(item.submit_Date);
                    if (
                        submitDate.getMonth() === currentMonth &&
                        submitDate.getFullYear() === currentYear &&
                        item.status.toLowerCase() === 'approved'
                    ) {
                        return total + item.approve_Amount;
                    }
                    return total;
                }, 0);

            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        // OnMounted hook to fetch data when component is mounted
        onMounted(() => {
            fetchData();
        });

        // Return properties and methods for use in the template
        return {
            totalRequests,
            approvedRequests,
            declinedRequests,
            totalDanaBulanIni,
            danaPerBulanData,
            lineChartOptions,
            formatToRupiah,
        };
    },
});
</script>

<style lang="scss" scoped>
.container {
    max-width: 1200px;
}

.text-blue-500 {
    color: #3b82f6;
}

.text-green-500 {
    color: #10b981;
}

.text-red-500 {
    color: #ef4444;
}

.text-yellow-500 {
    color: #f59e0b;
}

.text-2xl {
    font-size: 1.5rem;
}

.text-xl {
    font-size: 1.25rem;
}

.text-gray-800 {
    color: #1f2937;
}

h3 {
    font-weight: 600;
}
</style>