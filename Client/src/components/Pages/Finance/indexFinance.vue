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
                    <h3 class="text-xl font-semibold text-black">Total Reimbursement Requests</h3>
                    <p class="text-2xl font-bold text-blue-900">{{ totalRequests }}</p>
                </div>
            </div>

            <!-- Approved Requests -->
            <div class="bg-white p-6 rounded-lg shadow-md flex items-center hover:shadow-xl transition duration-300">
                <div class="w-1/4">
                    <i class="fas fa-check-circle text-4xl text-green-500"></i>
                </div>
                <div class="w-3/4 pl-4">
                    <h3 class="text-xl font-semibold text-black">Approved Requests</h3>
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
                    <h3 class="text-xl font-semibold text-black">Total Dana Bulan Ini</h3>
                    <p class="text-2xl font-bold text-black">Rp {{ totalDanaBulanIni }}</p>
                </div>
            </div>
        </div>
            <!-- Chart Section -->
            <div class="mt-8 grid grid-cols-1 lg:grid-cols-2 gap-8">
                <!-- Approval Status Tracking Chart -->
                <div class="bg-white p-6 rounded-lg shadow-lg">
                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Tracking Status per Bulan</h3>
                    <line-chart :data="approvalStatusData" :options="lineChartOptions"></line-chart>
                </div>

                <!-- Dana Per Bulan Tracking Chart -->
                <div class="bg-white p-6 rounded-lg shadow-lg">
                    <h3 class="text-xl font-semibold text-gray-800 mb-4">Jumlah Dana yang Dikeluarkan per Bulan</h3>
                    <line-chart :data="danaPerBulanData" :options="lineChartOptions"></line-chart>
                </div>
            </div>
        </div>
    </MainLayout>
</template>


<script>
import { defineComponent } from 'vue';
import { Line } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement } from 'chart.js';
import MainLayout from '../../layouts/MainLayout.vue';

ChartJS.register(Title, Tooltip, Legend, LineElement, CategoryScale, LinearScale, PointElement);

export default defineComponent({
    components: {
        'line-chart': Line,
        MainLayout
    },
    data() {
        return {
            totalRequests: 120,
            approvedRequests: 85,
            declinedRequests: 20,
            totalDanaBulanIni: 5000000,
            
            // Data for Approval Status Tracking Chart
            approvalStatusData: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [
                    {
                        label: 'Approved',
                        data: [10, 15, 30, 40, 50, 70, 85, 90, 95, 100, 110, 120],
                        borderColor: '#10b981',
                        backgroundColor: 'rgba(16, 185, 129, 0.2)',
                        fill: true,
                        tension: 0.4
                    },
                    {
                        label: 'Declined',
                        data: [5, 7, 10, 15, 18, 20, 22, 24, 26, 28, 30, 32],
                        borderColor: '#ef4444',
                        backgroundColor: 'rgba(239, 68, 68, 0.2)',
                        fill: true,
                        tension: 0.4
                    }
                ]
            },

            danaPerBulanData: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [
                    {
                        label: 'Dana Dikeluarkan (Rp)',
                        data: [1000000, 1500000, 2000000, 2500000, 3000000, 3500000, 4000000, 4500000, 5000000, 5500000, 6000000, 6500000],
                        borderColor: '#3b82f6',
                        backgroundColor: 'rgba(59, 130, 246, 0.2)',
                        fill: true,
                        tension: 0.4
                    }
                ]
            },

            // Chart options
            lineChartOptions: {
                responsive: true,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top'
                    },
                    tooltip: {
                        callbacks: {
                            label: (context) => `Rp ${context.raw.toLocaleString()}`
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: (value) => `Rp ${value.toLocaleString()}`
                        }
                    }
                }
            }
        };
    }
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