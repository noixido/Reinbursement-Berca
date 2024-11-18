<template>
    <MainLayout>
        <div class="container mx-auto p-6 grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- Left Section: Cards -->
            <div class="grid grid-cols-2 gap-4">
                <!-- Total Reimbursement Requests -->
                <div class="bg-white p-4 rounded-lg shadow-md flex items-center hover:shadow-lg transition duration-300">
                    <i class="fas fa-file-invoice text-4xl text-blue-500 mr-4"></i>
                    <div>
                        <h3 class="text-lg font-semibold text-black">Total Requests</h3>
                        <p class="text-2xl font-bold text-blue-900">{{ totalRequests }}</p>
                    </div>
                </div>

                <!-- Approved Requests -->
                <div class="bg-white p-4 rounded-lg shadow-md flex items-center hover:shadow-lg transition duration-300">
                    <i class="fas fa-check-circle text-4xl text-green-500 mr-4"></i>
                    <div>
                        <h3 class="text-lg font-semibold text-black">Approved Status</h3>
                        <p class="text-2xl font-bold text-green-900">{{ approvedRequests }}</p>
                    </div>
                </div>

                <!-- Declined Requests -->
                <div class="bg-white p-4 rounded-lg shadow-md flex items-center hover:shadow-lg transition duration-300">
                    <i class="fas fa-times-circle text-4xl text-red-500 mr-4"></i>
                    <div>
                        <h3 class="text-lg font-semibold text-black">Declined Status</h3>
                        <p class="text-2xl font-bold text-red-900">{{ declinedRequests }}</p>
                    </div>
                </div>

                <!-- Total Dana Bulan Ini -->
                <div class="bg-white p-4 rounded-lg shadow-md flex items-center hover:shadow-lg transition duration-300">
                    <i class="fas fa-coins text-4xl text-yellow-500 mr-4"></i>
                    <div>
                        <h3 class="text-lg font-semibold text-black">Approved Funds</h3>
                        <p class="text-2xl font-bold text-black">{{ formatToRupiah(totalDanaBulanIni) }}</p>
                    </div>
                </div>
            </div>

            <!-- Right Section: Line Chart -->
            <div class="bg-white p-6 rounded-lg shadow-lg">
                <h3 class="text-xl font-semibold text-gray-800 mb-4">Jumlah Dana yang Disetujui per Bulan</h3>
                <line-chart :data="danaPerBulanData" :options="lineChartOptions"></line-chart>
            </div>
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
        MainLayout,
    },
    setup() {
        const totalRequests = ref(0);
        const totalDanaBulanIni = ref(0);
        const approvedRequests = ref(0);
        const declinedRequests = ref(0);

        const danaPerBulanData = ref({
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            datasets: [
                {
                    label: 'Dana Disetujui (Rp)',
                    data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    borderColor: '#3b82f6',
                    backgroundColor: 'rgba(59, 130, 246, 0.2)',
                    fill: true,
                    tension: 0.4,
                },
            ],
        });

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

        const formatToRupiah = (value) => {
            return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR' }).format(value);
        };

        const fetchData = async () => {
            try {
                const response = await axios.get('https://localhost:7102/api/Reimbursement', {
                    headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
                });

                const reimbursements = response.data.data;

                totalRequests.value = reimbursements.length;
                approvedRequests.value = reimbursements.filter(
                    (item) => item.status.toLowerCase() === 'approved'
                ).length;
                declinedRequests.value = reimbursements.filter(
                    (item) => item.status.toLowerCase().includes('decline')
                ).length;

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

                const monthlyDana = Array(12).fill(0);
                reimbursements.forEach((item) => {
                    const submitDate = new Date(item.submit_Date);
                    const month = submitDate.getMonth();
                    if (item.status.toLowerCase() === 'approved') {
                        monthlyDana[month] += item.approve_Amount;
                    }
                });

                danaPerBulanData.value.datasets[0].data = monthlyDana;
            } catch (error) {
                console.error('Error fetching data:', error);
            }
        };

        onMounted(() => {
            fetchData();
        });

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

<style scoped>
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

.text-lg {
    font-size: 1.125rem;
}

.text-sm {
    font-size: 0.875rem;
}

.text-gray-800 {
    color: #1f2937;
}
</style>
