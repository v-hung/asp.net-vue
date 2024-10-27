import { computed, ref, shallowRef, type Component } from 'vue'
import { defineStore } from 'pinia'
import DashboardIcon from "@/components/icons/DashboardIcon.vue";
import StoreIcon from '@/components/icons/StoreIcon.vue';
import BookIcon from '@/components/icons/BookIcon.vue';
import ReportIcon from '@/components/icons/ReportIcon.vue';
import apiClient from '@/config/axios';
import type { MenuResponse } from '@/types/menu';
import UserIcon from '@/components/icons/UserIcon.vue';

export type PermissionType = 'View' | 'Create' | 'Update' | 'Delete' | 'Import'

export type Menu = MenuResponse & {
  icon?: Component
}

const MENU_PATH_ICONS: Record<string, { icon: Component, name?: string}> = {
  '/admin': {
    icon: shallowRef(DashboardIcon)
  },
  '/admin/store': {
    icon: shallowRef(StoreIcon)
  },
  '/admin/book': {
    icon: shallowRef(BookIcon)
  },
  '/admin/report': {
    icon: shallowRef(ReportIcon)
  },
  '/admin/profile': {
    icon: UserIcon,
    name: "Profile",
  },
}

const MENU_PATH_IGNORED = ['/admin/profile']

export const useMenuStore = defineStore('menu', () => {
  const menus = ref<Menu[]>([])

  const getAllMenus = async () => {
    try {

      const response = await apiClient.get('/api/Menu');

      const data = response.data.menus as MenuResponse[];

      menus.value = data.map((menu) => {
        return {
          ...menu,
          icon: MENU_PATH_ICONS[menu.url].icon ?? null
        }
      })
    } catch (error) {
      console.log(error)
    }
  }

  const hasAccessPath = (path: string) => {
    if (MENU_PATH_IGNORED.includes(path)) {
      return true
    }
    return menus.value.some((menu) => menu.url === path || menu.children.some((child) => child.url === path))
  }

  const firstPathMenu = computed(() => {
    if (menus.value.length === 0) {
      return null
    }

    if (menus.value[0].url) {
      return menus.value[0].url
    }
    else if (menus.value[0].children.length > 0) {
      return menus.value[0].children[0].url
    }
    return null;
  })

  const getCurrentMenu = (path: string) => {
    const currentMenu: Menu[] = [];

    if (MENU_PATH_IGNORED.includes(path)) {
      currentMenu.push({
        id: path,
        url: path,
        name: MENU_PATH_ICONS[path].name ?? '',
        icon: MENU_PATH_ICONS[path].icon,
        isVisible: true,
        parentId: null,
        permissionTypes: [],
        children: [],
        permissions: []
      })

      return currentMenu
    }

    for (const menu of menus.value) {
      if (menu.url === path) {
        currentMenu.push(menu)
        break
      }
      else if (menu.children.length > 0) {
        for (const child of menu.children) {
          if (child.url === path) {
            currentMenu.push(menu)
            currentMenu.push(child)
            break
          }
        }
      }
    }
    return currentMenu
  }

  return { menus, getAllMenus, firstPathMenu, hasAccessPath, getCurrentMenu }
})
