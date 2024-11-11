<template>
    <div class="breadcrumbs text-sm flex justify-end pt-0">
        <ul>
            <li v-for="(item, index) in items" :key="index">
                <RouterLink :to="item.path">
                    {{ item.name }}
                </RouterLink>
            </li>
        </ul>
    </div>
</template>

<script>
export default {
    props: {
        items: {
            type: Array,
            required: true
        }
    }
}
</script>

<style scoped>
.breadcrumb {
    margin-bottom: 1rem;
}
</style>