import { createRouter } from "vue-router";
import Universities from "../components/pages/Universities.vue";
import { createWebHistory } from "vue-router";

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: "/",
            redirect: "/universities"
        },
        {
            path: "/universities",
            name: "Universities",
            component: Universities,
            props: true,
        }
    ],
});

export default router;