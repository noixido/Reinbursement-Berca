import { createApp } from 'vue'
// import './style.css'
import './assets/tailwind.css';
import App from './App.vue'
import router from './router/index';


// createApp(App).mount('#app')
const app = createApp(App);
app.use(router);
app.mount('#app');
