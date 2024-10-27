import { ref } from 'vue'
import { defineStore } from 'pinia'
import { useRouter } from 'vue-router';
import apiClient from '@/config/axios';
import { useMenuStore } from './menuStore';

export interface User {
  id: string
  email: string
  fullName: string
  phoneNumber?: string
  emailConfirmed: boolean
  address?: string
  image?: string
  createdAt: string
  updatedAt: string
}

export const useUserStore = defineStore('user', () => {
  const router = useRouter();
  const menuStore = useMenuStore();
  const user = ref<User | null>(null)

  async function getCurrentUser() {
    try {
      const response = await apiClient.get('/api/Account/current-user');

      user.value = response.data.user;

      await menuStore.getAllMenus();

    } catch (error) {
      console.log(error)
    }
  }

  async function login(email: string, password: string) {
    const response = await apiClient.post('/api/Account/login', {
      email,
      password
    });

    localStorage.setItem('token', response.data.token);
    localStorage.setItem('refreshToken', response.data.refreshToken);

    user.value = response.data.user;

    await menuStore.getAllMenus();
  }

  const logout = async () => {
    await apiClient.post('/api/Account/logout', {
      refreshToken: localStorage.getItem('refreshToken')
    });

    user.value = null

    localStorage.setItem('token', "");
    localStorage.setItem('refreshToken', "");

    await router.push('/login');
  };

  return { user, login, logout, getCurrentUser }
})
