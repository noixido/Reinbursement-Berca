<template>
    <div class="bg-grey-100 min-h-screen flex items-center justify-center">
        <div class="card bg-white text-primary-content w-96 mx-auto border-red-500 border-2">
            <div class="card-body ">
                <!-- <h2 class="card-title">Login</h2> -->
                <div class="w-48 mx-auto text-center flex flex-col gap-2">
                    <img src="../../assets/images/logo.png" alt="Becare logo">
                    <span class="font-bold text-red-600 text-lg w-full">Berca Reimbursement</span>
                </div>
                <form id="login" class="mt-5">
                    <input type="email" v-model="email" class="bg-white rounded-md p-3 w-full mb-2 border-red-500 border" name="email" id="email" placeholder="Email">
                    <input :type="showPassword ? 'text' : 'password'" v-model="password" class="bg-white rounded-md p-3 w-full mb-2 border-red-500 border" name="password" id="password" placeholder="Password">
                    <label for="showPass">
                        <input type="checkbox" v-model="showPassword" id="showPass"> Show Password
                    </label>
                </form>
                <div class="card-actions justify-end">
                    <button type="submit" form="login" class="btn text-white border-none w-full mt-5 bg-red-600 hover:bg-red-500 " @click.prevent="login">
                        <div :class="stateLoading ? 'loading loading-spinner loading-sm text-white' : ''">
                            Login
                        </div>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import axios from 'axios';
import { ref } from 'vue';
import { useRouter } from 'vue-router';


export default {
    data() {
        return {
            stateLoading: ref(false),
            email: "",
            password: "",
            showPassword: false,
            router: useRouter(),
        };
    },
    methods: {
        async login(){
            this.stateLoading = true;

            try{
                const api = "https://localhost:7102/api/Login";
                const response = await axios.post(api, {
                    email: this.email,
                    password: this.password,
                });

                // console.log(response.data.token);

                const token = response.data.token;
                localStorage.setItem("token", token);

                const payload = JSON.parse(atob(token.split(".")[1]));
                const role = payload.role;

                // console.log(role);

                const redirectPath =
                    this.$route.query.redirect ||
                    (role === "Employee"
                        ? "/employee/dashboard"
                        : role === "HR"
                        ? "/hr/dashboard"
                        : role === "Finance"
                        ? "/finance/dashboard"
                        : "/"); // Default fallback path if none match
                this.router.push(redirectPath);
            }catch(error){
                console.error(error.response.data.message);
            }finally{
                this.stateLoading = false;
            }
        }
    },
    setup () {
        

        return {}
    }
}
</script>

<style lang="scss" scoped>

</style>