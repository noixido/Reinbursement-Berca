<template>
    <MainLayout>
        <div class="container mx-auto p-6 bg-gray-50 min-h-screen">
            <h1 class="text-3xl font-bold text-gray-900 mb-8">Human Resource Dashboard</h1>

            <!-- Dashboard Grid -->
            <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <!-- Left Side Summary Cards Section -->
                <div class="lg:col-span-3 grid grid-cols-1 md:grid-cols-3 gap-6">
                    <!-- Card for Total Requests -->
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <div class="w-1/4 flex justify-center">
                            <i class="fas fa-file-invoice text-5xl text-blue-600"></i>
                        </div>
                        <div class="w-3/4 pl-4">
                            <h3 class="text-xl font-semibold text-black">Total Reimbursement Requests</h3>
                            <p class="text-3xl font-bold text-blue-900">{{ totalRequests }}</p>
                        </div>
                    </div>

                    <!-- Card for Total Titles -->
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <div class="w-1/4 flex justify-center">
                            <i class="fas fa-user-tag text-5xl text-purple-600"></i>
                        </div>
                        <div class="w-3/4 pl-4">
                            <h3 class="text-xl font-semibold text-black">Total Titles</h3>
                            <p class="text-3xl font-bold text-purple-900">{{ totalTitles }}</p>
                        </div>
                    </div>

                    <!-- Card for Total Registered Accounts -->
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <div class="w-1/4 flex justify-center">
                            <i class="fas fa-users text-5xl text-green-600"></i>
                        </div>
                        <div class="w-3/4 pl-4">
                            <h3 class="text-xl font-semibold text-black">Total Registered Accounts</h3>
                            <p class="text-3xl font-bold text-green-900">{{ totalAccounts }}</p>
                        </div>
                    </div>

                    <!-- Monthly Reimbursements Card -->
                    <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300">
                        <h3 class="text-lg font-semibold text-black mb-2">Monthly Reimbursements</h3>
                        <p class="text-4xl font-bold text-indigo-900">Rp {{ monthlyReimbursements }}</p>
                    </div>

                    <!-- Pending Approvals Card -->
                    <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300">
                        <div class="flex items-center">
                            <i class="fas fa-clock text-4xl text-yellow-500"></i>
                            <div class="ml-4">
                                <h3 class="text-lg font-semibold text-black mb-2">Pending Approvals</h3>
                                <p class="text-4xl font-bold text-yellow-900">{{ pendingApprovals }}</p>
                            </div>
                        </div>
                    </div>

                    <!-- Rejected Requests Card -->
                    <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300">
                        <div class="flex items-center">
                            <i class="fas fa-times-circle text-4xl text-red-500"></i>
                            <div class="ml-4">
                                <h3 class="text-lg font-semibold text-black mb-2">Decline Requests</h3>
                                <p class="text-4xl font-bold text-red-900">{{ rejectedRequests }}</p>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Right Side Charts Section -->
                <div class="lg:col-span-3 grid grid-cols-1 lg:grid-cols-2 gap-8 mt-8">
                    <!-- Bar Chart -->
                    <div class="bg-white p-8 rounded-lg shadow-lg">
                        <h3 class="text-lg font-semibold text-gray-700 mb-4">Banyaknya Reimbursement per Kategori</h3>
                        <div class="chart-container-lg">
                            <canvas id="barChart"></canvas>
                        </div>
                    </div>

                    <!-- Line Chart -->
                    <div class="bg-white p-8 rounded-lg shadow-lg">
                        <h3 class="text-lg font-semibold text-gray-700 mb-4">Total Reimbursement per Bulan</h3>
                        <div class="chart-container-lg">
                            <canvas id="lineChart"></canvas>
                        </div>
                    </div>

                    <!-- Scatter Plot -->
                    <div class="bg-white p-8 rounded-lg shadow-lg col-span-2">
                        <h3 class="text-lg font-semibold text-gray-700 mb-4">Title Reimbursements per Bulan</h3>
                        <div class="chart-container-lg">
                            <canvas id="scatterPlot"></canvas>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </MainLayout>
</template>

<script>
import { defineComponent, ref, onMounted } from 'vue';
import axios from 'axios';
import Chart from 'chart.js/auto';
import MainLayout from '../../layouts/MainLayout.vue';

export default defineComponent({
    components: {
        MainLayout
    },
    setup() {
        const totalRequests = ref(0);
        const totalTitles = ref(0); 
        const totalAccounts = ref(0);
        const monthlyReimbursements = ref(5000000);
        const pendingApprovals = ref(10);
        const rejectedRequests = ref(0);

        onMounted(async () => {
            if (document.getElementById('scatterPlot')) {
                new Chart(document.getElementById('scatterPlot'), {
                    type: 'scatter',
                    data: {
                        datasets: [{
                            label: 'Title Reimbursements per Bulan',
                            data: [
                                { x: 1, y: 10, title: 'Employee' },
                                { x: 2, y: 20, title: 'Manager' },
                                { x: 3, y: 5, title: 'General Manager' },
                                { x: 4, y: 25, title: 'Director' },
                                { x: 5, y: 30, title: 'CEO' }
                            ],
                            backgroundColor: '#f59e0b'
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            tooltip: {
                                callbacks: {
                                    label: function(context) {
                                        const title = context.raw.title || 'No Title';
                                        const x = context.raw.x;
                                        const y = context.raw.y;
                                        return `Title: ${title}, X: ${x}, Y: ${y}`;
                                    }
                                }
                            }
                        },
                        scales: {
                            x: {
                                title: {
                                    display: true,
                                    text: 'Index'
                                }
                            },
                            y: {
                                title: {
                                    display: true,
                                    text: 'Jumlah Reimbursement'
                                }
                            }
                        }
                    }
                });
            }

            try {
                // Fetch data total reimbursement
                const reimbursementResponse = await axios.get('https://localhost:7102/api/reimbursement', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem("token")}`
                    }
                });
                totalRequests.value = reimbursementResponse.data.data.length;

                // Filter and count declined requests
                const declinedRequests = reimbursementResponse.data.data.filter(request => request.status.toLowerCase().includes('decline'));
                rejectedRequests.value = declinedRequests.length;

                // Fetch data total akun
                const accountResponse = await axios.get('https://localhost:7102/api/Account', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem("token")}`
                    }
                });
                totalAccounts.value = accountResponse.data.data.length;

                // Fetch data total titles
                const titlesResponse = await axios.get('https://localhost:7102/api/Titles', {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem("token")}`
                    }
                });
                totalTitles.value = titlesResponse.data.data.length;

                // Render charts jika elemen chart tersedia
                if (document.getElementById('barChart')) {
                    new Chart(document.getElementById('barChart'), {
                        type: 'bar',
                        data: {
                            labels: ['Employee', 'Manager', 'General Manager', 'Director'],
                            datasets: [{
                                label: 'Reimbursements per Kategori',
                                data: [30, 45, 10, 15],
                                backgroundColor: ['#3b82f6', '#8b5cf6', '#10b981', '#f59e0b']
                            }]
                        },
                        options: { responsive: true, maintainAspectRatio: false }
                    });
                }

                if (document.getElementById('lineChart')) {
                    new Chart(document.getElementById('lineChart'), {
                        type: 'line',
                        data: {
                            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                            datasets: [{
                                label: 'Total Reimbursements per Bulan',
                                data: [120, 150, 170, 140, 200, 220, 210, 190, 230, 250, 240, 260],
                                fill: false,
                                borderColor: '#3b82f6',
                                tension: 0.1
                            }]
                        },
                        options: { responsive: true, maintainAspectRatio: false }
                    });
                }
            } catch (error) {
                console.error("Error fetching data:", error);
            }
        });

        return {
            totalRequests,
            totalTitles,
            totalAccounts,
            monthlyReimbursements,
            pendingApprovals,
            rejectedRequests
        };
    }
});
</script>

<style lang="scss" scoped>
.container {
    max-width: 1200px;
}
.chart-container-lg {
    max-height: 500px;
    overflow: hidden;
}

.bg-blue-100, .bg-purple-100, .bg-green-100, .bg-indigo-100, .bg-yellow-100, .bg-red-100 {
    background-color: #ebf8ff;
}
.text-blue-600, .text-purple-600, .text-green-600, .text-indigo-800, .text-yellow-800, .text-red-800 {
    font-size: 1.25rem;
}
.text-4xl {
    font-size: 2.25rem;
    font-weight: bold;
}
.text-3xl {
    font-size: 1.875rem;
}
.text-gray-900 {
    color: #1f2937;
}
.hover\:shadow-lg:hover {
    box-shadow: 0 10px 20px rgba(0, 0, 0, 0.15);
}
.hover\:shadow-xl:hover {
    box-shadow: 0 20px 30px rgba(0, 0, 0, 0.2);
}
</style>
