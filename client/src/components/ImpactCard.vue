<script setup lang="ts">

defineProps<{
  icon:any
  bgColor: string
  number: string | number
  title: string
}>()


</script>

<template>
    <div class="flex flex-col">
        <p>{{ number }}</p>
        <p>{{ title }}</p>
    </div>
</template>

