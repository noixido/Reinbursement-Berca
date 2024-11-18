<template>
    <MainLayout>
        <div class="container mx-auto p-6 bg-gray-50 min-h-screen">
            <h1 class="text-3xl font-bold text-gray-900 mb-8">Human Resource Dashboard</h1>

            <div class="flex space-x-6">
                <!-- Cards Section -->
                <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 w-full">
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <i class="fas fa-clock text-3xl text-yellow-500 mr-4"></i>
                        <div>
                            <h3 class="text-lg font-semibold text-gray-900">Pending Approvals</h3>
                            <p class="text-2xl font-bold text-yellow-900">{{ pendingApprovals }}</p>
                        </div>
                    </div>
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <i class="fas fa-times-circle text-3xl text-red-500 mr-4"></i>
                        <div>
                            <h3 class="text-lg font-semibold text-gray-900">Decline Requests</h3>
                            <p class="text-2xl font-bold text-red-900">{{ rejectedRequests }}</p>
                        </div>
                    </div>
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <i class="fas fa-check-circle text-3xl text-teal-600 mr-4"></i>
                        <div>
                            <h3 class="text-lg font-semibold text-gray-900">Approved Status</h3>
                            <p class="text-2xl font-bold text-teal-900">{{ approvedRequests }}</p>
                        </div>
                    </div>
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300">
                        <i class="fas fa-file-invoice text-3xl text-blue-600 mr-4"></i>
                        <div>
                            <h3 class="text-lg font-semibold text-gray-900">Total Requests</h3>
                            <p class="text-2xl font-bold text-blue-900">{{ totalRequests }}</p>
                        </div>
                    </div>
                    <!-- Adjusted the width of the Monthly Reimbursements card -->
                    <div class="bg-white p-6 rounded-lg shadow-lg flex items-center hover:shadow-xl transition duration-300 w-full">
                        <i class="fas fa-coins text-4xl text-yellow-500 mr-4"></i>
                        <div>
                            <h3 class="text-lg font-semibold text-gray-900">
                                Approved Saldo</h3>
                            <p class="text-2xl font-bold text-black">{{ formattedMonthlyReimbursements }}</p>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Charts Section -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mt-8">
                <!-- Line Chart -->
                <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300 transform hover:scale-105">
                    <h3 class="text-lg font-bold text-gray-800 mb-4">Total Reimbursement per Bulan</h3>
                    <div class="chart-container">
                        <canvas id="lineChart"></canvas>
                    </div>
                </div>
                <!-- Bar Chart -->
                <div class="bg-white p-6 rounded-lg shadow-lg hover:shadow-xl transition duration-300 transform hover:scale-105">
                    <h3 class="text-lg font-bold text-gray-800 mb-4">Banyaknya Reimbursement per Kategori</h3>
                    <div class="chart-container">
                        <canvas id="barChart"></canvas>
                    </div>
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
        const pendingApprovals = ref(0);
        const rejectedRequests = ref(0);
        const approvedRequests = ref(0);

        // Fungsi untuk membuat line chart
        const createLineChart = (monthlyRequestCount) => {
            new Chart(document.getElementById('lineChart'), {
                type: 'line',
                data: {
                    labels: Object.keys(monthlyRequestCount), // Nama bulan
                    datasets: [
                        {
                            label: 'Jumlah Pengajuan Reimbursement',
                            data: Object.values(monthlyRequestCount), // Total pengajuan reimbursement per bulan
                            fill: false,
                            borderColor: '#3b82f6',
                            tension: 0.1
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    scales: {
                        x: { title: { display: true, text: 'Bulan' } },
                        y: { title: { display: true, text: 'Jumlah Pengajuan' } }
                    }
                }
            });
        };

        const createBarChart = (categories) => {
            new Chart(document.getElementById('barChart'), {
                type: 'bar',
                data: {
                    labels: categories.map(cat => cat.name),
                    datasets: [
                        {
                            label: 'Jumlah Reimbursement',
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
                            title: { display: true, text: 'Kategori' },
                            ticks: { display: false } // Menyembunyikan label kategori
                        },
                        y: { 
                            title: { display: true, text: 'Jumlah Reimbursement' },
                            ticks: {
                                beginAtZero: true, // Mulai dari 0
                                stepSize: 1, // Langkah nilai adalah 1
                                callback: function(value) {
                                    return Number.isInteger(value) ? value : ''; // Tampilkan hanya angka bulat
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
                const currentYear = new Date().getFullYear(); // Tahun berjalan
                const totalApprovedThisMonth = reimbursements.reduce((sum, item) => {
                    const submitDate = new Date(item.submit_Date);
                    if (
                        item.status.toLowerCase().includes('approved') && // Hanya yang berstatus approved
                        submitDate.getMonth() === currentMonth && // Bulan saat ini
                        submitDate.getFullYear() === currentYear // Tahun saat ini
                    ) {
                        return sum + item.approve_Amount;
                    }
                    return sum;
                }, 0);

                // Update nilai card Monthly Reimbursements
                monthlyReimbursements.value = totalApprovedThisMonth;

                // Reimbursement status "Progress in HR" atau "Progress in Finance"
                const inProgressRequests = reimbursements.filter((request) =>
                    ['progress in hr', 'progress in finance'].includes(request.status)
                );
                pendingApprovals.value = inProgressRequests.length;

                // Hitung jumlah reimbursement yang ditolak
                const declinedRequests = reimbursements.filter(request =>
                    request.status.toLowerCase().includes('decline')
                );
                rejectedRequests.value = declinedRequests.length;

                // Hitung jumlah Approved Requests
                const approved = reimbursements.filter(request =>
                    request.status.toLowerCase().includes('approved')
                );
                approvedRequests.value = approved.length;


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

                createBarChart(categories);

                // Hitung jumlah reimbursement per bulan untuk line chart
                const monthlyRequestCount = reimbursements.reduce((acc, item) => {
                    const submitDate = new Date(item.submit_Date);
                    const month = submitDate.toLocaleString('default', { month: 'short' });
                    acc[month] = (acc[month] || 0) + 1;
                    return acc;
                }, {
                    Jan: 0, Feb: 0, Mar: 0, Apr: 0, May: 0, Jun: 0,
                    Jul: 0, Aug: 0, Sep: 0, Oct: 0, Nov: 0, Dec: 0
                });

                // Filter data hanya sampai bulan berjalan
                const currentMonthIndex = new Date().getMonth(); // 0 = Januari
                const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                const filteredMonthlyRequestCount = months.slice(0, currentMonthIndex + 1).reduce((filtered, month) => {
                    filtered[month] = monthlyRequestCount[month] || 0; // Pastikan bulan tanpa data memiliki nilai 0
                    return filtered;
                }, {});

                // Buat line chart berdasarkan data yang difilter
                createLineChart(filteredMonthlyRequestCount);

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
            pendingApprovals,
            rejectedRequests,
            formattedMonthlyReimbursements,
            approvedRequests,
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

.speedometer-container {
    margin: 0 auto;
    text-align: center;
    position: relative;
}

.chart-container {
    height: 250px;
    margin-top: -10px;
}

.speedometer-title {
    margin-bottom: 10px;
    font-weight: bold;
    font-size: 1.125rem;
    color: #374151;
}

.flex {
    gap: 1.5rem; /* Jarak antar kartu dan chart */ 
}
</style>