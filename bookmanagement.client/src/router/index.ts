import { createRouter, createWebHistory } from "vue-router";
import { useUserStore } from "@/stores/userStore";
import AuthLayout from "@/components/layouts/AuthLayout.vue";
import AdminLayout from "@/components/layouts/AdminLayout.vue";
import LoginView from "@/views/LoginView.vue";
import { useMenuStore } from "@/stores/menuStore";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      redirect: "/admin",
    },
    {
      path: "/login",
      component: AuthLayout,
      children: [
        {
          path: "",
          name: "login",
          component: LoginView,
        },
      ],
    },
    {
      path: "/admin",
      component: AdminLayout,
      children: [
        {
          path: "",
          name: "dashboard",
          component: () => import("../views/DashboardView.vue"),
        },
        {
          path: "store",
          name: "store",
          component: () => import("../views/StoreView.vue"),
        },
        {
          path: "book",
          name: "book",
          component: () => import("../views/BookView.vue"),
        },
        {
          path: "report",
          name: "report",
          redirect: { name: "report1" },
          children: [
            {
              path: "report1",
              name: "report1",
              component: () => import("../views/Report1View.vue"),
            },
            {
              path: "report2",
              name: "report2",
              component: () => import("../views/Report2View.vue"),
            },
            {
              path: "report3",
              name: "report3",
              component: () => import("../views/Report3View.vue"),
            },
          ],
        },
        {
          path: "Profile",
          name: "Profile",
          component: () => import("../views/ProfileView.vue"),
        },
      ],
      meta: { requiresAuth: true },
    },
    {
      path: "/error",
      name: "Error",
      component: () => import("../views/ErrorView.vue"),
    },
  ],
});

router.beforeEach(async (to, from, next) => {
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);
  const userStore = useUserStore();
  const menuStore = useMenuStore();

  if (requiresAuth) {
    if (!userStore.user) {
      await userStore.getCurrentUser();
    }

    if (userStore.user) {
      const hasAccess = menuStore.hasAccessPath(to.path);
      if (hasAccess) {
        next();
      } else {
        if (menuStore.firstPathMenu) {
          next(menuStore.firstPathMenu);
        } else {
          next("/error");
        }
      }
    } else {
      next("/login");
    }
  } else {
    next();
  }
});

export default router;
