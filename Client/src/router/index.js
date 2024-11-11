import { createRouter } from "vue-router";
import Universities from "../components/pages/Universities.vue";
import { createWebHistory } from "vue-router";
import ApprovalByHR from "../components/pages/ApprovalByHR.vue";

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
        },
        {
            path: '/hr-approval',
            name: 'HRApproval',
            component: ApprovalByHR,
        },
    ],
});

export default router;