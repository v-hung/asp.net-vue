<script setup lang="ts">
import AppLogo from "@/components/icons/AppLogo.vue";
import InputText from "primevue/inputtext";
import Checkbox from "primevue/checkbox";
import Button from "primevue/button";
import GoogleIcon from "@/components/icons/GoogleIcon.vue";
import GithubIcon from "@/components/icons/GithubIcon.vue";
import { useUserStore } from "@/stores/userStore";
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useMenuStore } from "@/stores/menuStore";

const loading = ref(false);

const router = useRouter();
const userStore = useUserStore();
const menuStore = useMenuStore();

const handleSubmit = async (e: Event) => {
  if (loading.value) return;

  loading.value = true;

  const { email, password } = Object.fromEntries(new FormData(e.target as HTMLFormElement)) as { email: string, password: string }

  await userStore.login(email, password);

  if (menuStore.firstPathMenu) {
    router.push({ path: menuStore.firstPathMenu });
  }
  else {
    router.push({ path: "/error" });
  }


  loading.value = false;
};
</script>

<template>
  <div class="flex flex-col items-center gap-3">
    <AppLogo class="w-10 h-10" />
    <h5 class="text-lg md:text-xl lg:text-2xl font-semibold mt-4">
      Welcome back to VH Admin
    </h5>
    <p class="text-gray-500">Enter your username and password to continue</p>

    <form
      action=""
      class="w-full mt-6 flex flex-col gap-4"
      @submit.prevent="handleSubmit"
    >
      <div class="flex flex-col gap-2">
        <label for="email">Email</label>
        <InputText id="email" name="email" placeholder="Enter your email" />
      </div>
      <div class="flex flex-col gap-2">
        <label for="password">Password</label>
        <InputText id="password" type="password" name="password" placeholder="Enter your password" />
      </div>
      <div class="flex justify-between">
        <div class="flex items-center">
          <Checkbox inputId="remember" />
          <label for="remember" class="ml-2"> Remember Me </label>
        </div>
        <RouterLink to="#" class="font-semibold">Forgot Password?</RouterLink>
      </div>

      <Button type="submit" class="w-full" :loading="loading" label="Sign In" />
    </form>

    <div class="w-full flex items-center gap-3">
      <div class="flex-1 h-[1px] bg-gray-300"></div>
      <span class="text-gray-500">Or login with</span>
      <div class="flex-1 h-[1px] bg-gray-300"></div>
    </div>

    <div class="w-full flex gap-4">
      <Button class="flex-1" outlined><GoogleIcon /> Google</Button>
      <Button class="flex-1" outlined><GithubIcon /> Github</Button>
    </div>
  </div>
</template>
