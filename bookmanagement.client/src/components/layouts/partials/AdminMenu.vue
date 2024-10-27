<script setup lang="ts">
import AvatarUser from "@/components/common/AvatarUser.vue";
import Collapse from "@/components/common/Collapse.vue";
import AppLogo from "@/components/icons/AppLogo.vue";
import BellIcon from "@/components/icons/BellIcon.vue";
import ChevronRightIcon from "@/components/icons/ChevronRightIcon.vue";
import MoreIcon from "@/components/icons/MoreIcon.vue";
import UserIcon from "@/components/icons/UserIcon.vue";
import { useMenuStore } from "@/stores/menuStore";
import { useUserStore } from "@/stores/userStore";
import Button from "primevue/button";
import Popover from "primevue/popover";
import { ref } from "vue";
import { useRoute } from "vue-router";

const op = ref();

const toggle = (event: MouseEvent) => {
  op.value.toggle(event);
};

const menuStore = useMenuStore();
const userStore = useUserStore();

const route = useRoute();

const isActive = (parentPath: string) => {
  return route.path.startsWith(parentPath);
};
</script>

<template>
  <div class="w-72 p-4 flex flex-col overflow-y-auto">
    <div class="flex items-center gap-2">
      <AppLogo class="w-8 h-8" />
      <span class="text-2xl font-bold pb-1">VH Admin</span>
    </div>

    <div class="mt-8 flex flex-col gap-2">
      <template v-for="menu in menuStore.menus" :key="menu.id">
        <template v-if="menu.children.length > 0">
          <Collapse :isOpen="isActive(menu.url)">
            <template #header>
              <div class="menu-item" :class="{ active: isActive(menu.url) }">
                <component :is="menu.icon" class="w-6 h-6" />
                <span>{{ menu.name }}</span>
                <ChevronRightIcon
                  class="menu-item-icon w-4 h-4 ml-auto transition-transform"
                />
              </div>
            </template>
            <div class="menu-child">
              <template v-for="child in menu.children" :key="child.id">
                <RouterLink
                  :to="child.url"
                  class="menu-item"
                  activeClass="active"
                >
                  <span>{{ child.name }}</span>
                </RouterLink>
              </template>
            </div>
          </Collapse>
        </template>
        <template v-else>
          <RouterLink
            :to="menu.url"
            class="menu-item"
            exactActiveClass="active"
          >
            <component :is="menu.icon" class="w-6 h-6" />
            <span>{{ menu.name }}</span>
          </RouterLink>
        </template>
      </template>
    </div>

    <div class="flex flex-col gap-2 mt-auto pt-2">
      <RouterLink to="/admin" class="menu-item">
        <BellIcon class="w-6 h-6" />
        <span>Notifications</span>
        <div class="badge">4</div>
      </RouterLink>
      <hr />
      <div class="menu-item">
        <AvatarUser class="w-8 h-8" />
        <span>{{ userStore.user?.fullName }}</span>
        <button class="ml-auto" @click="toggle">
          <MoreIcon class="w-6 h-6" />
        </button>
        <Popover ref="op">
          <div class="flex flex-col gap-4 -m-2 w-40">
            <RouterLink
              to="/admin/profile"
              @click="toggle"
              class="py-2 flex gap-2 items-center rounded px-2 hover:bg-blue-300"
            >
              <UserIcon class="w-5 h-5" />
              <span>Profile</span>
            </RouterLink>
            <Button
              severity="danger"
              size="small"
              @click.prevent="userStore.logout()"
              >Logout</Button
            >
          </div>
        </Popover>
      </div>
    </div>
  </div>
</template>

<style>
.menu-item {
  @apply rounded-xl flex gap-2 items-center font-semibold px-2 py-2 text-gray-600 border border-transparent;
}
.menu-item.active,
.menu-item:hover {
  @apply bg-white/80 border-gray-200 shadow-sm !text-black;
}
.menu-item .badge {
  @apply ml-auto min-w-5 h-5 rounded-full bg-gradient-to-b from-red-400 to-red-600 text-white text-xs shadow-sm px-0.5 leading-5 text-center;
}

.app-collapse.open .menu-item-icon {
  @apply rotate-90;
}
.menu-child {
  @apply bg-gray-200 rounded-lg mt-2;
}

.menu-child > .menu-item {
  @apply ml-8 border-none shadow-none bg-transparent;
}
.menu-child > .menu-item.active,
.menu-child > .menu-item:hover {
  @apply text-black underline;
}
</style>
