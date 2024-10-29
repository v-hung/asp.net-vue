import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useRouter } from 'vue-router';
import { useMenuStore } from './menuStore';
import { accountApi } from '@/config/api';
import type { LoginResponse } from '@/generate-api';

export const useUserStore = defineStore('user', () => {
  const router = useRouter();
  const menuStore = useMenuStore();
  const user = ref<LoginResponse['user'] | null>(null)

  async function getCurrentUser() {
    try {
      const response = await accountApi.apiAccountCurrentUserGet();

      user.value = response.data;

      await menuStore.getAllMenus();

    } catch (error) {
      console.log(error)
    }
  }

  async function login(email: string, password: string) {
    const response = await accountApi.apiAccountLoginPost({
      email,
      password
    });

    localStorage.setItem('token', response.data.token);
    localStorage.setItem('refreshToken', response.data.refreshToken);

    user.value = response.data.user;

    await menuStore.getAllMenus();
  }

  const logout = async () => {
    await accountApi.apiAccountLogoutPost({
      refreshToken: localStorage.getItem('refreshToken')
    });

    user.value = null

    localStorage.setItem('token', "");
    localStorage.setItem('refreshToken', "");

    await router.push('/login');
  };

  return { user, login, logout, getCurrentUser }
})
