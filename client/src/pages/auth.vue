<script setup lang="ts">
import { ref } from 'vue'


const isLogin = ref(true)
const toggleForm = (login: boolean) => (isLogin.value = login)
</script>

<template>
  <div class="bg-[#F2F8F5] flex flex-col min-h-screen items-center justify-center">
    <div class="flex items-center  gap-2 my-6">
      <IconMdiRecycle class="text-green-500 sm:w-10 sm:h-auto" />
      <p class="text-center font-bold text-[#082111] text-xl sm:text-4xl">EcoSort AI</p>
    </div>

    <form class="flex flex-col bg-[#FEFFFF] w-full xs:w-64 sm:w-md md:w-lg p-4 xs:p-5 sm:p-10 rounded-xl border border-[#a5b2a7] pb-5 transition-all duration-300 ease-in-out overflow-hidden">

      <div class="flex flex-col gap-3 sm:flex-row sm:justify-between bg-[#E7EFE8] p-3 mb-10 rounded-lg transition-all duration-300 ease-in-out">
        <!--used :class for dynamic change in the styling
            used an array so we can combine multiple classes(static & dynamic) & we can seperate them usin , instead of having one string block-->
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
      <!--used to ensure smooth animation when the field appears & disappears
          <transition> listens to the appearance or disappearance of a child element -->
      <transition
        enter-active-class="transition-all duration-700 ease-in-out"
        enter-from-class="opacity-0 -translate-y-2 max-h-0"
        enter-to-class="opacity-100 translate-y-0 max-h-40"
        leave-active-class="transition-all duration-700 ease-in-out"
        leave-from-class="opacity-100 translate-y-0 max-h-40"
        leave-to-class="opacity-0 -translate-y-2 max-h-0"
      >
        <div v-if="!isLogin" class="mb-6 overflow-hidden">
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
