<script setup lang="ts">
import type { Component } from 'vue'

defineProps<{
  icon:Component
  bgColor: string
  number: string | number
  title: string
}>()


</script>

<template>
    <div class="flex flex-col w-full border border-[#E2EDE5] p-5 rounded-3xl bg-[#FEFFFF]">
        <div class="rounded-full flex justify-center items-center w-15 h-15 mb-7"
         :style="{ backgroundColor: bgColor }">
          <component :is="icon" class="w-10 h-10 text-white"/>
        </div>
        <p class="font-bold text-black">{{ number }}</p>
        <p class="text-sm text-gray-500">{{ title }}</p>
    </div>
</template>

