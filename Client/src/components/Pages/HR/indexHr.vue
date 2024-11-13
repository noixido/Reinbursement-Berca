<template>
    <MainLayout>
        <div class="container mx-auto p-6">
        <h1 class="text-3xl font-semibold text-gray-800 mb-6">Dashboard Human Resource</h1>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            <!-- Total Requests Card -->
            <div class="bg-white p-6 rounded-lg shadow-md flex items-center">
                <div class="w-1/4">
                    <i class="fas fa-file-invoice text-4xl text-blue-500"></i>
                </div>
                <div class="w-3/4 pl-4">
                    <h3 class="text-xl font-semibold">Total Reimbursement Requests</h3>
                    <p class="text-2xl font-bold">{{ totalRequests }}</p>
                </div>
            </div>

            <!-- Approved Requests Card -->
            <div class="bg-white p-6 rounded-lg shadow-md flex items-center">
                <div class="w-1/4">
                    <i class="fas fa-check-circle text-4xl text-green-500"></i>
                </div>
                <div class="w-3/4 pl-4">
                    <h3 class="text-xl font-semibold">Approved Requests</h3>
                    <p class="text-2xl font-bold">{{ approvedRequests }}</p>
                </div>
            </div>

            <!-- Declined Requests Card -->
            <div class="bg-white p-6 rounded-lg shadow-md flex items-center">
                <div class="w-1/4">
                    <i class="fas fa-times-circle text-4xl text-red-500"></i>
                </div>
                <div class="w-3/4 pl-4">
                    <h3 class="text-xl font-semibold">Declined Requests</h3>
                    <p class="text-2xl font-bold">{{ declinedRequests }}</p>
                </div>
            </div>
        </div>

        <!-- Progress Bar -->
        <div class="mt-8">
            <h3 class="text-2xl font-semibold mb-4">Approval Progress</h3>
            <div class="bg-gray-200 rounded-full h-2.5 w-full">
                <div :style="{ width: approvalProgress + '%' }" class="bg-blue-500 h-2.5 rounded-full"></div>
            </div>
            <p class="mt-2 text-center text-sm">{{ approvalProgress }}% Approval Completed</p>
        </div>

        <!-- Chart Section -->
        <!-- <div class="mt-8">
            <h3 class="text-2xl font-semibold mb-4">Approval Status Distribution</h3>
            <pie-chart :data="chartData" :options="chartOptions"></pie-chart>
        </div> -->
    </div>
    </MainLayout>
</template>

<script>
// Importing the necessary chart components
import { defineComponent } from 'vue';
import { Pie } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, ArcElement, CategoryScale, LinearScale } from 'chart.js';
import MainLayout from '../../layouts/MainLayout.vue';

ChartJS.register(Title, Tooltip, Legend, ArcElement, CategoryScale, LinearScale);

export default defineComponent({
    components: {
        'pie-chart': Pie,
        MainLayout
    },
    data() {
        return {
            // Card data
            totalRequests: 120,
            approvedRequests: 85,
            declinedRequests: 20,

            // Approval progress (in percentage)
            approvalProgress: 70,

            // Chart data
            chartData: {
                labels: ['Approved', 'Pending', 'Declined'],
                datasets: [{
                    data: [85, 30, 20],
                    backgroundColor: ['#34D399', '#F59E0B', '#F87171'],
                    hoverBackgroundColor: ['#16A34A', '#D97706', '#DC2626'],
                }]
            },
            chartOptions: {
                responsive: true,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    tooltip: {
                        callbacks: {
                            label: (context) => {
                                return `${context.label}: ${context.raw}%`;
                            }
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

.bg-white {
    background-color: white;
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

.bg-gray-200 {
    background-color: #e5e7eb;
}

.bg-blue-500 {
    background-color: #3b82f6;
}

.text-2xl {
    font-size: 1.5rem;
}

.text-xl {
    font-size: 1.25rem;
}

.text-sm {
    font-size: 0.875rem;
}

.text-gray-800 {
    color: #1f2937;
}

h3 {
    font-weight: 600;
}
</style>
