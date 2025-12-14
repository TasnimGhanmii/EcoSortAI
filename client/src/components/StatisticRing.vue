<script setup lang="ts">
import { ref, watch, computed } from 'vue';

const props = defineProps<{
  percentage: number;
  nbItems: string;
  title: string;
  color?: string;
}>();

const animatedPercentage = ref(0);

// Animate whenever percentage changes
watch(
  () => props.percentage,
  (newVal, oldVal) => {
    const target = Math.min(Math.max(newVal, 0), 100);
    const duration = 1200;
    const start = performance.now();
    const startValue = animatedPercentage.value;

    const animate = (time: number) => {
      const elapsed = time - start;
      const progress = Math.min(elapsed / duration, 1);
      animatedPercentage.value = Math.floor(startValue + progress * (target - startValue));
      if (progress < 1) requestAnimationFrame(animate);
    };

    requestAnimationFrame(animate);
  },
  { immediate: true }
);

const radius = 45;
const circumference = 2 * Math.PI * radius;

const strokeDashoffset = computed(() => {
  return circumference - (animatedPercentage.value / 100) * circumference;
});

const strokeColor = computed(() => {
  if (!props.color) return '#25C55E';
  const colors: Record<string, string> = {
    green: '#25C55E',
    blue: '#3B82F6',
    amber: '#F59E0B',
    red: '#EF4444',
  };
  return colors[props.color] || props.color;
});
</script>

<template>
  <div class="flex flex-col items-center">
    <svg width="120" height="120" viewBox="0 0 120 120" class="relative">
      <circle
        cx="60"
        cy="60"
        :r="radius"
        fill="none"
        stroke="#E5E7EB"
        stroke-width="8"
      />
      <circle
        cx="60"
        cy="60"
        :r="radius"
        fill="none"
        :stroke="strokeColor"
        stroke-width="8"
        stroke-linecap="round"
        :stroke-dasharray="circumference"
        :stroke-dashoffset="strokeDashoffset"
        transform="rotate(-90 60 60)"
      />
      <text
        x="60"
        y="53"
        text-anchor="middle"
        dominant-baseline="middle"
        class="text-gray-800 font-bold"
        style="font-size: 20px; fill: currentColor;"
      >
        {{ animatedPercentage }}%
      </text>
      <text
        x="60"
        y="76"
        text-anchor="middle"
        class="text-gray-500"
        style="font-size: 10px; fill: currentColor;"
      >
        {{ nbItems }} items
      </text>
    </svg>
    <p class="mt-4 text-sm font-medium text-gray-700">{{ title }}</p>
  </div>
</template>
