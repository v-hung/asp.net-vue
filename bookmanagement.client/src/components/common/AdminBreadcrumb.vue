<script setup lang="ts">
import { useMenuStore } from "@/stores/menuStore";
import ChevronRightIcon from "../icons/ChevronRightIcon.vue";
import { useRoute } from "vue-router";
import { computed } from "vue";
import BellIcon from "../icons/BellIcon.vue";

const menuStore = useMenuStore();
const route = useRoute();

const currentMenus = computed(() => menuStore.getCurrentMenu(route.path));
</script>

<template>
  <div
    class="flex items-center gap-2 px-6 py-3 sticky top-0 z-10 bg-white text-gray-700"
  >
    <template v-for="(menu, index) in currentMenus" :key="menu.id">
      <div class="flex items-center gap-2">
        <template v-if="menu.icon">
          <component :is="menu.icon" class="w-6 h-6" />
        </template>
        <span class="font-semibold">{{menu.name}}</span>
        <template v-if="index < currentMenus.length - 1">
          <ChevronRightIcon class="w-4 h-4" />
        </template>
      </div>

    </template>

    <button class="ml-auto rounded-full p-2 border relative">
      <BellIcon class="w-6 h-6" />
      <div class="absolute w-2.5 h-2.5 rounded-full bg-red-500 top-0 right-0"></div>
    </button>
  </div>
</template>
