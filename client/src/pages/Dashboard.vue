
<script setup lang="ts">
//TypeScript doesn’t check component usage inside <template> by default.
//It only checks code inside <script setup>

//The template is compiled separately by Vue’s compiler (with help from Vite plugins like unplugin-vue-components), which resolves auto-imported components at build time, regardless of TypeScript.
import ImpactCard from '../components/ImpactCard.vue';
import IconMdiRecycle from '~icons/mdi/recycleVariant';        
import IconMdiLeaf from '~icons/mdi/leaf';        
import IconMdiSproutOutline from '~icons/mdi/sproutOutline';        
import IconMdiSetAll from '~icons/mdi/setAll';        
import StatisticRing from '../components/statisticRing.vue';


</script>


<template>
    <div class="flex flex-col bg-[#F2F9F4]">
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
          <!--virtual components generated at build time by VUE & Vite plugins-->
          <IconMdiUpload class="h-15 w-15 text-[#21C55C]"/>
         </div>
         <p class="font-semibold text-lg mt-5 mb-1">Upload Waste Image</p>
         <p class="text-sm text-gray-400">Drag and drop an image here, or click to select</p>
         <button class="text-white font-medium cursor-pointer rounded-lg bg-gradient-to-r from-[#25C55E] to-[#63E392] p-2 px-6 my-6 active:scale-95 transition-all duration-300 ease-in-out">Select Image</button>
         <p class="text-xs text-gray-400">Supports JPG, PNG, WEBP up to 10MB</p>
      
      </div>

      <div class="flex flex-col mx-auto my-8 p-2  w-[60%]">
          <div class="flex justify-between mb-10">
               <div>
                   <p class="font-bold text-4xl">Your Impact</p>
                   <p class="text-sm text-gray-500">Track your waste sorting progress and environmental contribution</p>
               </div>

               <button class="bg-[#2AC864] cursor-pointer h-10 my-auto px-8 text-xs text-white font-medium rounded-2xl flex items-center gap-1 active:scale-95 transition duration-300 ease-in-out"><IconMdiFileExport/>Export Report</button>
          </div>
        
          <div class=" flex flex-wrap md:flex-nowrap  gap-6">
              <ImpactCard :icon="IconMdiSetAll" number="100" title="Total Items Sorted" bg-color="#52DD84" />
              <ImpactCard :icon="IconMdiRecycle" number="5" title="Recyclable Items" bg-color="#3A82F6" />
              <ImpactCard :icon="IconMdiLeaf" number="10" title="Compostable Items" bg-color="#F3DA5C" />
              <ImpactCard :icon="IconMdiSproutOutline" number="50%" title="Eco Score" bg-color="#BDB9D5" />
          </div>

          <div class="flex flex-col w-full p-4 md:p-10 border mt-13 rounded-3xl border-[#E2EDE5] bg-[#FEFFFF] shadow-[0_0_15px_5px_rgba(209,250,229,0.7)] ">
            <p class="font-bold text-2xl mb-8 text-center">Waste Category Breakdown</p>
            
            <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6">
              <StatisticRing :percentage="50" nb-items="3" title="Recyclable" color="green"/>
              <StatisticRing :percentage="30" nb-items="2" title="Compostable" color="amber"/>
              <StatisticRing :percentage="15" nb-items="1" title="Hazardous" color="red"/>
              <StatisticRing :percentage="5" nb-items="0" title="Landfill" color="gray"/>
            </div>
         </div>
      </div>
    </div>
    
        
</template>