<template>
    <MainLayout>
        <div class="container mx-auto p-6 bg-gray-50 min-h-screen">
            <h1 class="text-3xl font-bold text-gray-900 mb-8">Finance Dashboard</h1>

            <div class="flex space-x-6">
                <!-- Cards Section -->
                <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 w-full">
                    <div class="bg-blue-50 p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <i class="fas fa-file-invoice text-3xl text-blue-600 mr-4"></i>
                        <div>
                            <h3 class="text-lg font-semibold text-gray-900">Total Requests</h3>
                            <p class="text-2xl font-bold text-blue-900">{{ totalRequests }}</p>
                        </div>
                    </div>
                    <!-- Adjusted the width of the Monthly Reimbursements card -->
                    <div class="bg-yellow-50 p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300 w-full">
                        <i class="fas fa-coins text-4xl text-yellow-500 mr-4"></i>
                        <div>
                            <h3 class="text-lg font-semibold text-gray-900">
                                Approved Funds</h3>
                            <p class="text-2xl font-bold text-black">{{ formattedMonthlyReimbursements }}</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Charts Section -->
            <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300 transform hover:scale-105 mt-8">
                <h3 class="text-lg font-bold text-gray-800 mb-4">Banyaknya Reimbursement per Kategori</h3>
                <div class="chart-container">
                    <canvas id="barChart"></canvas>
                </div>
            </div>

            <!-- Line Chart Section -->
            <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300 transform hover:scale-105 mt-8">
                <h3 class="text-lg font-bold text-gray-800 mb-4">Tracking Status (Approved vs Declined)</h3>
                <div class="chart-container">
                    <canvas id="lineChart"></canvas>
                </div>
            </div>
        </div>
    </MainLayout>
</template>

<script>
import { defineComponent, ref, onMounted, computed } from 'vue';
import axios from 'axios';
import Chart from 'chart.js/auto';
import MainLayout from '../../layouts/MainLayout.vue';

export default defineComponent({
    components: {
        MainLayout
    },
    setup() {
        const totalRequests = ref(0);
        const monthlyReimbursements = ref(0);

        // Fungsi untuk membuat bar chart
        const createBarChart = (categories) => {
            new Chart(document.getElementById('barChart'), {
                type: 'bar',
                data: {
                    labels: categories.map(cat => cat.name),
                    datasets: [
                        {
                            label: 'Total Reimbursement',
                            data: categories.map(cat => cat.count),
                            backgroundColor: ['#3b82f6', '#6366f1', '#10b981', '#f59e0b', '#f87171', '#34d399', '#818cf8'],
                            borderColor: '#1e3a8a',
                            borderWidth: 1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: { 
                            title: { display: true, text: 'Category' },
                            ticks: { display: false } 
                        },
                        y: { 
                            title: { display: true, text: 'Total Reimbursement' },
                            ticks: {
                                beginAtZero: true, // Mulai dari 0
                                stepSize: 1, 
                                callback: function(value) {
                                    return Number.isInteger(value) ? value : ''; // Tampilkan hanya angka bulat
                                }
                            }
                        }
                    }
                }
            });
        };

        const createLineChart = (monthlyRequestCount) => {
            new Chart(document.getElementById('lineChart'), {
                type: 'line',
                data: {
                    labels: Object.keys(monthlyRequestCount),
                    datasets: [
                        {
                            label: 'Approved Status',
                            data: Object.values(monthlyRequestCount).map(month => month.approved),
                            borderColor: 'green',
                            backgroundColor: 'rgba(34, 197, 94, 0.2)',
                            fill: true,
                            tension: 0.4,
                        },
                        {
                            label: 'Rejected Status',
                            data: Object.values(monthlyRequestCount).map(month => month.rejected),
                            borderColor: 'red',
                            backgroundColor: 'rgba(239, 68, 68, 0.2)',
                            fill: true,
                            tension: 0.4,
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: {
                            title: { display: true, text: 'Month' },
                        },
                        y: {
                            title: { display: true, text: 'Count' },
                            ticks: {
                                beginAtZero: true, // Mulai dari 0
                                stepSize: 1, 
                                callback: function(value) {
                                    return value % 1 === 0 ? value : '';
                                }
                            }
                        }
                    }
                }
            });
        };


        // Computed untuk format angka dengan titik koma
        const formattedMonthlyReimbursements = computed(() => {
            return new Intl.NumberFormat('id-ID', { style: 'currency', currency: 'IDR' }).format(monthlyReimbursements.value);
        });

        const fetchData = async () => {
            try {
                // Fetch data reimbursement
                const reimbursementResponse = await axios.get('https://localhost:7102/api/Reimbursement', {
                    headers: { Authorization: `Bearer ${localStorage.getItem("token")}` }
                });

                const reimbursements = reimbursementResponse.data.data;

                // Total semua request
                totalRequests.value = reimbursements.length;

                // Hitung total dana yang disetujui untuk bulan berjalan
                const currentMonth = new Date().getMonth(); // Index bulan berjalan (0 = Januari, 11 = Desember)
                const currentYear = new Date().getFullYear(); 
                const totalApprovedThisMonth = reimbursements.reduce((sum, item) => {
                    const submitDate = new Date(item.submit_Date);
                    if (
                        item.status.toLowerCase().includes('approved') && 
                        submitDate.getMonth() === currentMonth && 
                        submitDate.getFullYear() === currentYear
                    ) {
                        return sum + item.approve_Amount;
                    }
                    return sum;
                }, 0);

                // Update nilai card Monthly Reimbursements
                monthlyReimbursements.value = totalApprovedThisMonth;

                // Hitung jumlah reimbursement per kategori
                const categoryCounts = reimbursements.reduce((acc, item) => {
                    const categoryName = item.category_Name;
                    acc[categoryName] = (acc[categoryName] || 0) + 1;
                    return acc;
                }, {});

                // Format data untuk bar chart
                const categories = Object.keys(categoryCounts).map(name => ({
                    name,
                    count: categoryCounts[name]
                }));

                // Buat bar chart berdasarkan data kategori
                createBarChart(categories);

                // Menghitung status Reimbursement berdasarkan bulan
                const monthlyStatusCounts = {};

                for (let month = 0; month < 12; month++) {
                    const monthData = reimbursements.filter(item => {
                        const submitDate = new Date(item.submit_Date);
                        return submitDate.getMonth() === month;
                    });

                    const approved = monthData.filter(item => item.status.toLowerCase().includes('approved')).length;
                    const declined = monthData.filter(item => item.status.toLowerCase().includes('decline')).length;

                    // Set default jika belum ada data untuk bulan tertentu
                    monthlyStatusCounts[new Date(2024, month).toLocaleString('default', { month: 'short' })] = {
                        approved,
                        rejected: declined
                    };
                }

                // Filter data hanya sampai bulan berjalan
                const currentMonthIndex = new Date().getMonth(); // 0 = Januari
                const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                const filteredMonthlyStatusCounts = months.slice(0, currentMonthIndex + 1).reduce((filtered, month) => {
                    filtered[month] = monthlyStatusCounts[month] || { approved: 0, rejected: 0 };
                    return filtered;
                }, {});

                // Buat line chart berdasarkan data status
                createLineChart(filteredMonthlyStatusCounts);

            } catch (error) {
                console.error('Error fetching data:', error.response?.data?.message || error.message);
            }
        };

        onMounted(() => {
            fetchData();
        });

        return {
            totalRequests,
            monthlyReimbursements,
            formattedMonthlyReimbursements,
        };
    }
});
</script>

<style lang="scss" scoped>
.container {
    padding: 1.5rem;
}

.grid {
    gap: 1.5rem; /* Perlebar jarak antar grid */
}

.card {
    padding: 1.5rem;
    border-radius: 0.5rem;
}

.chart-container {
    height: 350px; /* Diperlebar untuk tampilan chart */
}

.flex {
    gap: 1.5rem; /* Jarak antar kartu dan chart */ 
}
</style>
