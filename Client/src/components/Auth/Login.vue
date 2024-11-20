<template>
    <div class="bg-indigo-950 min-h-screen flex items-center justify-center text-indigo-950">
        <div class="flex w-full max-w-4xl bg-white rounded-lg shadow-lg overflow-hidden">
            <!-- Left side - Login Form -->
            <div class="flex flex-col justify-center items-center w-1/2 p-8">
                <div class="w-40 mb-6">
                    <img src="../../assets/images/logo.png" alt="Berca logo" />
                </div>
                <!-- <h2 class="text-2xl font-bold text-gray-700 mb-2">LOGIN</h2> -->
                <p class="text-gray-500 font-bold mb-6">Berca Reimbursement</p>

                <form id="login" class="w-full flex flex-col gap-4">
                    <!-- Email input -->
                    <label class="flex flex-col">
                        <input type="email" v-model="state.email" class="bg-gray-100 rounded-md p-3 w-full border-indigo-950 border-2 focus:outline-none" placeholder="Email" />
                        <span v-if="v$.email.$error" class="text-sm text-red-500">{{ v$.email.$errors[0].$message }}</span>
                    </label>

                    <!-- Password input -->
                    <label class="flex flex-col">
                        <input :type="showPassword ? 'text' : 'password'" v-model="state.password" class="bg-gray-100 rounded-md p-3 w-full border-2 border-indigo-950 focus:outline-none" placeholder="Password" />
                        <span v-if="v$.password.$error" class="text-sm text-red-500">{{ v$.password.$errors[0].$message }}</span>
                    </label>

                    <!-- Show Password Checkbox -->
                    <label for="showPass" class="flex items-center mt-2">
                        <input type="checkbox" v-model="showPassword" id="showPass" class="mr-2" /> Show Password
                    </label>

                    <!-- Login Button -->
                    <button type="submit" form="login" class="bg-indigo-950 hover:bg-indigo-900 text-white rounded-md p-3 w-full mt-5" @click.prevent="login">
                        <div :class="stateLoading ? 'loading loading-spinner loading-sm text-white' : ''">
                            Login Now
                        </div>
                    </button>
                </form>
            </div>

            <!-- Right side - Image and Decorative Elements -->
            <div class="w-1/2 bg-gradient-to-l from-red-600 to-red-400 relative flex items-center justify-center">
                <img src="../../assets/images/login-banner.jpg" alt="Profile" class="h-full" />
            </div>
        </div>
    </div>
</template>

<script>
import axios from 'axios';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import useVuelidate from '@vuelidate/core';
import { required, email } from '@vuelidate/validators';
import { reactive, computed } from 'vue';
import { toast } from 'vue3-toastify';
import 'vue3-toastify/dist/index.css';

export default {
    data() {
        return {
            stateLoading: ref(false),
            showPassword: false,
            router: useRouter(),
        };
    },
    methods: {
        async login(){
            this.v$.$validate();

            if(this.v$.$error){
                console.log("fix the error first!")
            } else {
                this.stateLoading = true;

                try{
                    const api = "https://localhost:7102/api/Login";
                    const response = await axios.post(api, {
                        email: this.state.email,
                        password: this.state.password,
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
                            ? "/dashboard"
                            : role === "HR"
                            ? "/dashboard"
                            : role === "Finance"
                            ? "/dashboard"
                            : "/"); // Default fallback path if none match
                    this.router.push(redirectPath);
                }catch(error){
                    console.error(error.response.data.message);
                    // toast.error(error.response.data.message, {
                    //     autoClose: 1000,
                    // });
                }finally{
                    this.stateLoading = false;
                }
            }
        }
    },
    setup () {
        const state = reactive({
            email: "",
            password: "",
        });

        const rules = computed(()=>{
            return {
                email: { required, email },
                password: { required },
            };
        });

        const v$ = useVuelidate(rules, state);
        return {
            state, 
            v$
        }
    },
}
</script>

<style lang="scss" scoped>
</style>