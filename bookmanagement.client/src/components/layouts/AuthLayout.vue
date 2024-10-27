<script setup lang="ts">
  const year = new Date().getFullYear()
</script>

<template>
  <div class="flex gap-12 min-h-screen p-4">
    <div class="hidden lg:block flex-1 rounded-lg bg-slate-100 bg-cover" style="background-image: url(/images/bg-auth.png);">
    </div>

    <div class="flex-1 flex flex-col pt-8 pb-4">
      <div class="flex-grow min-h-0 w-full max-w-lg mx-auto mb-6">
        <RouterView />
      </div>

      <div class="w-full mt-auto text-xs md:text-sm text-gray-500 flex flex-col sm:flex-row justify-between">
        <p>© {{ year }} Việt Hùng. All rights reserved.</p>
        <p class="font-semibold flex gap-2 text-gray-800">
          <a href="#">Private Policy</a>

          <a href="#">Terms & Conditions</a>
        </p>
      </div>
    </div>
  </div>
</template>
