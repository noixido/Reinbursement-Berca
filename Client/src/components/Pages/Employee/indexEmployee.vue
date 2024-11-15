<template>
    <MainLayout>
        <div class="container mx-auto p-6">
            <h1 class="text-3xl font-semibold text-gray-800 mb-6">Dashboard Employee</h1>

            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                <!-- Saldo Card -->
                <div class="bg-white p-6 rounded-lg shadow-md flex items-center">
                    <div class="w-1/4">
                        <i class="fas fa-check-circle text-4xl text-green-500"></i>
                    </div>
                    <div class="w-3/4 pl-4">
                        <h3 class="text-xl font-semibold">Current Balance</h3>
                        <p class="text-2xl font-bold">Rp. {{ formatCurrency(accountDetail.current_Limit) }},00</p>
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
    components:{
        MainLayout
    },
    data() {
        const token = localStorage.getItem('token');
        const payload = JSON.parse(atob(token.split('.')[1]));
        const email_Account = payload.email
        const role_Account = payload.role
        const id_Account = payload.id_account;
        const name_Account = payload.name;
        return {
            email_Account,
            accountDetail: {},
        };
    },
    methods: {
        fetchAccountDetail(){
            axios
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
        formatCurrency(value) {
            if (!value) return '0';
            return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
        },
    },
    mounted() {
        this.fetchAccountDetail();
    },
}
</script>

<style lang="scss" scoped>

</style>