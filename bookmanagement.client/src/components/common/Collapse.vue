<!-- eslint-disable vue/multi-word-component-names -->
<script setup lang="ts">
import { ref, defineProps, watch, onMounted } from "vue";

const props = defineProps<{
  isOpen: boolean;
}>();

const isOpen = ref(props.isOpen);
const height = ref(0);

watch(
  () => props.isOpen,
  (newValue) => {
    isOpen.value = newValue;
  }
);

const calculateHeight = () => {
  const content = document.querySelector(".content");
  if (content) {
    height.value = content.scrollHeight;
  }
};

// Hàm toggle
const toggle = () => {
  isOpen.value = !isOpen.value;
};

onMounted(calculateHeight);
</script>

<template>
  <div class="app-collapse" :class="{ open: isOpen }">
    <div @click="toggle" class="header">
      <slot name="header" />
    </div>
    <div :style="{ maxHeight: isOpen ? height + 'px' : '0px' }" class="content">
      <slot />
    </div>
  </div>
</template>

<style scoped>
.header {
  cursor: pointer;
}

.content {
  overflow: hidden; /* Ẩn phần nội dung ra ngoài */
  transition: max-height 0.3s ease; /* Hiệu ứng chuyển đổi cho max-height */
}
</style>
