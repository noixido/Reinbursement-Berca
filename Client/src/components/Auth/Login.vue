<template>
    <div>
        <div class="card bg-white text-primary-content w-96 mx-auto mt-72">
            <div class="card-body">
                <h2 class="card-title">Login</h2>
                <form id="login">
                    <input type="email" v-model="email" class="bg-white border rounded-md p-3 w-full mb-2" name="email" id="email" placeholder="Email">
                    <input :type="showPassword ? 'text' : 'password'" v-model="password" class="bg-white border rounded-md p-3 w-full mb-2" name="password" id="password" placeholder="Password">
                    <label for="showPass">
                        <input type="checkbox" v-model="showPassword" id="showPass"> Show Password
                    </label>
                </form>
                <div class="card-actions justify-end">
                    <button type="submit" form="login" class="btn bg-primary text-white border-none" @click.prevent="login">
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
                        ? "/employee/dahsboard"
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