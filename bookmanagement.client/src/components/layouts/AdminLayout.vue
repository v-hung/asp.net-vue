<script setup lang="ts">
import AdminBreadcrumb from "../common/AdminBreadcrumb.vue";
import AdminMenu from "./partials/AdminMenu.vue";
</script>

<template>
  <div class="flex bg-gray-100 h-screen">
    <AdminMenu />
    <div
      class="flex-grow bg-white rounded-l-xl shadow-sm border overflow-y-auto"
    >
      <AdminBreadcrumb />
      <div class="px-6 py-2">
        <RouterView />
      </div>
    </div>
  </div>
</template>
