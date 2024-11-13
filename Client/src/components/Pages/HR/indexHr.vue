<template>
    <div>
        <MainLayout>
            <h1 class="text-black text-center my-72 text-9xl font-bold">Home</h1>
        </MainLayout>
    </div>
</template>

<script>
import MainLayout from '../../layouts/MainLayout.vue';

export default {
    name: "hr-dashboard",
    components: {
        MainLayout,
    },
    setup () {
        console.log("HR dashboard component loaded");
        return {};
    }
}
</script>

<style lang="scss" scoped>

</style>