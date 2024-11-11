<template>
    <table class="table table-zebra">
        <thead>
            <tr>
                <th v-for="(header, index) in headers" :key="index">{{ header }}</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(item, index) in items" :key="item.id">
                <td v-for="(field, fieldIndex) in fields" :key="fieldIndex">{{ item[field] }}</td>
                <td>
                    <button @click="$emit('edit', item[fields[0]])" class="btn btn-sm btn-warning">Edit</button>
                    <button @click="$emit('delete', item[fields[0]])" class="ml-1 btn btn-sm btn-error">Delete</button>
                </td>
            </tr>
        </tbody>
    </table>
</template>

<script>
export default {
    props: {
        items: {
            type: Array,
            required: true
        },
        fields: {
            type: Array,
            required: true
        },
        headers: {
            type: Array,
            required: true
        }
    }
}
</script>

<style scoped>
.table {
    width: 100%;
    border-collapse: collapse;
}

.table th,
.table td {
    border: 1px solid #ccc;
    padding: 8px;
    text-align: left;
}

.table th {
    background-color: #f8f8f8;
    font-weight: bold;
}

.table tr:nth-child(even) {
    background-color: #f2f2f2;
}
</style>
