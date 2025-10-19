<script setup lang="ts">
import type { Component } from 'vue'
import { ref } from 'vue'

defineProps<{
  icon: Component
  title: string
  description: string
  tips: string[]
  examples: string[]
  bgColor: string
}>()

const toggle = ref(false)

const toggleSection = () => {
  toggle.value = !toggle.value
}

// transition hooks for smooth height animation
const onEnter = (el: Element, done: () => void) => {
  const target = el as HTMLElement
  target.style.height = '0px'
  target.style.opacity = '0'
  // force reflow
  void target.offsetHeight
  // animate to full height
  target.style.transition = 'height 0.3s ease, opacity 0.3s ease'
  target.style.height = target.scrollHeight + 'px'
  target.style.opacity = '1'
  target.addEventListener('transitionend', done, { once: true })
}

const onLeave = (el: Element, done: () => void) => {
  const target = el as HTMLElement
  target.style.height = target.scrollHeight + 'px'
  target.style.opacity = '1'
  // force reflow
  void target.offsetHeight
  target.style.transition = 'height 0.3s ease, opacity 0.3s ease'
  target.style.height = '0px'
  target.style.opacity = '0'
  target.addEventListener('transitionend', done, { once: true })
}
</script>

<template>
  <div class="flex flex-col w-full">
    <div
      class="flex justify-between bg-[#FEFFFF] p-5 border border-[#d1ded5] rounded-xl transition-colors duration-300 hover:cursor-pointer hover:bg-emerald-100"
      :class="toggle ? 'border-b-0 rounded-b-none bg-emerald-100 ' : 'border-b'"
      @click="toggleSection"
    >
      <div class="flex gap-6">
        <div class="flex justify-center items-center p-2 rounded-full" :style="{ backgroundColor: bgColor }">
          <Component :is="icon" class="w-10 h-10 text-white" />
        </div>
        <div>
          <p class="font-bold text-xl mb-1">{{ title }}</p>
          <p class="text-gray-500 font-medium">{{ description }}</p>
        </div>
      </div>

      <div
        class="flex justify-center items-center transition-transform duration-300"
        :class="{ 'rotate-180': toggle}"
      >
        <IconMdiArrowDownDropCircleOutline class="h-8 w-8 text-slate-700" />
      </div>
    </div>

    <Transition
      :css="false"
      @enter="onEnter"
      @leave="onLeave"
    >
      <div v-show="toggle" class="overflow-hidden bg-[#FEFFFF] border border-t-0 border-[#d1ded5] rounded-b-xl">
        <div class="p-5 flex flex-col sm:flex-row justify-between gap-6">
          <div>
            <p class="my-3 font-bold">Disposal Tips</p>
            <ul class="list-disc list-inside space-y-2 marker:text-emerald-500 text-gray-500 text-sm">
              <li v-for="tip in tips" :key="tip">{{ tip }}</li>
            </ul>
          </div>
          <div>
            <p class="font-bold">Common Examples</p>
            <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3 mt-2">
              <p
                class="bg-[#C3F0D3] p-2 rounded-lg text-sm text-center"
                v-for="example in examples"
                :key="example"
              >
                {{ example }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>