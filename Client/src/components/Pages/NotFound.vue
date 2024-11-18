<template>
    <div class="not-found">
    <h1>404 - Page Not Found</h1>
    <p>Sorry, the page you are looking for does not exist.</p>
    <button @click="goBack" class="go-back-button">Go Back</button>
    </div>
</template>

<script>
export default {
    name: 'NotFound',
    methods: {
    goBack() {
        window.history.back(); 
    }
    }
};
</script>

<style scoped>
.not-found {
    text-align: center;
    margin-top: 50px;
}

.go-back-button {
    margin-top: 20px;
    padding: 10px 20px;
    font-size: 16px;
    cursor: pointer;
    background-color: #f44336;
    color: white;
    border: none;
    border-radius: 5px;
}
.go-back-button:hover {
    background-color: #d32f2f;
}
</style>
