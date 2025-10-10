<script setup lang="ts">
import { ref } from 'vue'


const isLogin = ref(true)
const toggleForm = (login: boolean) => (isLogin.value = login)
</script>

<template>
  <div class="bg-[#F2F8F5] flex flex-col min-h-screen items-center justify-center">
    <div class="flex gap-2 my-6">
      <IconMdiRecycle class="text-green-500 w-10 h-auto" />
      <p class="text-center font-bold text-[#082111] text-4xl">EcoSort AI</p>
    </div>

    <form class="flex flex-col bg-[#FEFFFF] w-md p-10 rounded-xl border border-[#a5b2a7] pb-5 transition-all duration-300 ease-in-out">

      <div class="flex justify-between bg-[#E7EFE8] p-3 mb-10 rounded-lg transition-colors duration-300 ease-in-out">
        <button
          type="button"
          @click="toggleForm(true)"
          :class="['text-center font-medium p-1.5 px-10 rounded-lg shadow-xs transition-colors duration-300 ease-in-out',
                   isLogin ? 'bg-[#25C55E] text-white' : 'bg-[#F5FAF7] text-gray-500']">
          Login
        </button>
        <button
          type="button"
          @click="toggleForm(false)"
          :class="['text-center font-medium p-1.5 px-10 rounded-lg shadow-xs transition-colors duration-300 ease-in-out',
                   !isLogin ? 'bg-[#25C55E] text-white' : 'bg-[#F5FAF7] text-gray-500']">
          Register
        </button>
      </div>

      <transition
        enter-active-class="transition-all duration-300 ease-in-out"
        enter-from-class="opacity-0 -translate-y-2"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition-all duration-300 ease-in-out"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 -translate-y-2"
      >
        <div v-if="!isLogin" class="mb-6">
          <label class="font-semibold mb-2 block" for="fullname">Full Name</label>
          <input
            class="p-2 border border-[#a5b2a7] rounded-lg w-full"
            type="text"
            id="fullname"
            placeholder="Full Name"
          />
        </div>
      </transition>

      <label for="email" class="font-semibold mb-2 block">Email</label>
      <input
        class="p-2 border border-[#a5b2a7] rounded-lg mb-6 w-full"
        type="email"
        id="email"
        placeholder="example@mail.com"
      />

      <label for="password" class="font-semibold mb-2 block">Password</label>
      <input
        class="p-2 border border-[#a5b2a7] rounded-lg mb-6 w-full"
        type="password"
        id="password"
        placeholder="Enter your password"
      />

      <button
        type="submit"
        class="text-white font-medium cursor-pointer rounded-lg bg-gradient-to-r from-[#25C55E] to-[#63E392] p-2 active:scale-95 transition-all duration-300 ease-in-out">
        {{ isLogin ? 'Login' : 'Register' }}
      </button>

      <p class="text-sm text-gray-500 text-center mt-10 flex justify-center gap-1">
        <IconMdiLeaf />Join the eco-friendly movement
      </p>
    </form>
  </div>
</template>
