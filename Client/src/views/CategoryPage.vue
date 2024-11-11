<template>
    <div class="container mx-auto p-4">
      <h1 class="text-2xl font-bold mb-4">Category List</h1>
      <ul class="space-y-2">
        <li v-for="category in categories" :key="category.Id_Category" class="p-4 bg-base-200 rounded-lg shadow">
          <div class="font-semibold">{{ category.Category_Name }}</div>
          <div class="text-sm text-gray-500">ID: {{ category.Id_Category }}</div>
        </li>
      </ul>
    </div>
  </template>
  
  <script>
  export default {
    name: 'CategoryPage',
    data() {
      return {
        categories: [
          { Id_Category: 'C001', Category_Name: 'Office Supplies' },
          { Id_Category: 'C002', Category_Name: 'Travel Expenses' },
          { Id_Category: 'C003', Category_Name: 'Medical' },
          { Id_Category: 'C004', Category_Name: 'Training' }
        ]
      };
    }
  };
  </script>
  
  <style scoped>
  .container {
    max-width: 600px;
  }
  </style>
  