<script setup lang="ts">

</script>


<template>
    <div class="flex flex-col">
      <div class="bg-[#28C762] justify-items-center -my-6.5 p-5 pt-30 pb-20">
        <div class="flex items-center  gap-2 my-6">
          <IconMdiRecycle class="text-white sm:w-10 sm:h-auto" />
          <p class="text-center font-bold text-white text-xl sm:text-4xl">EcoSort AI</p>
        </div>
        
        <p class="text-center text-white sm:text-lg">Smart waste classification powered by AI. Upload an image and instantly know how to dispose of it responsibly.</p>
        
        <div class="flex gap-10">
          <p class="text-sm text-white opacity-75 text-center mt-10 flex justify-center gap-1"><IconMdiLeaf />Eco-Friendly</p>
          <p class="text-sm text-white opacity-75 text-center mt-10 flex justify-center gap-1"><IconMdiRecycle />Eco-Friendly</p>
        </div>
      </div>

      <div class="flex flex-col  m-3 -mt-5 p-8 bg-white items-center w-[60%]  border border-[#D6E2DA] rounded-2xl mx-auto">
         <div class="bg-[#C3F1D3] p-5 rounded-full">
            <IconMdiUpload class="h-15 w-15 text-[#21C55C]"/>
         </div>
         <p class="font-semibold text-lg mt-5 mb-1">Upload Waste Image</p>
         <p class="text-sm text-gray-400">Drag and drop an image here, or click to select</p>
         <button class="text-white font-medium cursor-pointer rounded-lg bg-gradient-to-r from-[#25C55E] to-[#63E392] p-2 px-6 my-6 active:scale-95 transition-all duration-300 ease-in-out">Select Image</button>
         <p class="text-xs text-gray-400">Supports JPG, PNG, WEBP up to 10MB</p>
      
        </div>
    </div>
    
        
</template>