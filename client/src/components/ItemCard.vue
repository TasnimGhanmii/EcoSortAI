<script setup lang="ts">
import { computed } from 'vue'
import type { Component } from 'vue'

const props = defineProps<{
  icon: Component
  image: string
  category: string
  title: string
  accuracy: string | number
  date: string
  time: string
}>()

const accuracyPercent = computed(() => {
  let raw = props.accuracy
  if (typeof raw === 'string') {
    const match = raw.match(/[\d.]+/)
    raw = match ? match[0] : '0'
  }
  const num = parseFloat(String(raw))
  return isNaN(num) ? 0 : Math.min(100, Math.max(0, num))
})

const barColor = computed(() => {
  const val = accuracyPercent.value
  if (val < 33) return '#ef4444'
  if (val < 60) return '#f97316'
  if (val < 70) return '#eab308'
  return '#22c55e'
})
</script>

<template>
  <div class="flex flex-col border border-[#508762] bg-[#FEFFFF] rounded-xl shadow-md shadow-green-100 hover:scale-105 transition-all ease-in-out duration-300">
    <div class="relative inline-block mb-2 rounded-xl">
      <img :src="image" alt="image" class="w-full h-auto rounded-t-xl" />
      <p class="absolute flex gap-1 items-center top-3 right-3 p-0.5 px-5 text-sm bg-[#E7EFE9] rounded-xl font-medium text-[#6F898B] hover:text-white hover:bg-[#20C45D] transition-all ease-in-out duration-300">
        <component :is="icon" />{{ category }}
      </p>
    </div>

    <div class="flex flex-col p-5">
      <div class="flex justify-between items-center">
        <p class="font-medium">{{ title }}</p>
        <p class="text-xs text-gray-500">{{ accuracyPercent }}% confident</p>
      </div>

      <div class="mt-2 w-full h-2 bg-gray-200 rounded-full overflow-hidden">
        <div
          class="h-full rounded-full transition-all duration-500 ease-out"
          :style="{ width: `${accuracyPercent}%`, backgroundColor: barColor }"
        ></div>
      </div>

      <div class="flex gap-4 mt-2 text-sm text-gray-500">
        <p>{{ date }}</p>
        <p>{{ time }}</p>
      </div>
    </div>
  </div>
</template>