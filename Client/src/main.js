import { createApp } from 'vue'
import './assets/tailwind.css';
import App from './App.vue'
import router from './router/index';
import moment from 'moment';


// createApp(App).mount('#app')
const app = createApp(App);
app.use(router);
app.config.globalProperties.$moment = moment;
app.mount('#app');
