
<script setup lang="ts">
//TypeScript doesn’t check component usage inside <template> by default.
//It only checks code inside <script setup>

//The template is compiled separately by Vue’s compiler (with help from Vite plugins like unplugin-vue-components), which resolves auto-imported components at build time, regardless of TypeScript.
import ImpactCard from '../components/ImpactCard.vue';
import IconMdiRecycle from '~icons/mdi/recycleVariant';        
import IconMdiLeaf from '~icons/mdi/leaf';        
import IconMdiSproutOutline from '~icons/mdi/sproutOutline';        
import IconMdiSetAll from '~icons/mdi/setAll';        
import IconMdiBiohazard from '~icons/mdi/biohazard';        
import IconMdiDeleteVariant from '~icons/mdi/deleteVariant';        
import IconMdiTrashCanOutline from '~icons/mdi/trashCanOutline';        
import CategoryCard from '../components/CategoryCard.vue';
import ItemCard from '../components/ItemCard.vue';

const Categories=['All Categories','Recyclable','Compost','Hazardous','General']

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

      <div class="flex flex-col  m-3 -mt-5 p-8 bg-white items-center w-[70%]  border border-[#D6E2DA] rounded-2xl mx-auto">
         <div class="bg-[#C3F1D3] p-5 rounded-full">
          <!--virtual components generated at build time by VUE & Vite plugins-->
          <IconMdiUpload class="h-15 w-15 text-[#21C55C]"/>
         </div>
         <p class="font-semibold text-lg mt-5 mb-1">Upload Waste Image</p>
         <p class="text-sm text-gray-400">Drag and drop an image here, or click to select</p>
         <button class="text-white font-medium cursor-pointer rounded-lg bg-gradient-to-r from-[#25C55E] to-[#63E392] p-2 px-6 my-6 active:scale-95 transition-all duration-300 ease-in-out">Select Image</button>
         <p class="text-xs text-gray-400">Supports JPG, PNG, WEBP up to 10MB</p>
      
      </div>

      <div class="flex flex-col mx-auto my-8 p-2  w-[70%]">
          
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
              <StatisticRing :percentage="5" nb-items="0" title="General" color="gray"/>
            </div>
         </div>

         <div class="flex flex-col w-full gap-3.5 mt-20 p-5">
             <p class="font-bold text-3xl  mb-10">Waste Category Guide</p>

             <CategoryCard :icon="IconMdiRecycle" title="Recycable Waste" description="Materials that can be processed & reused" 
             :tips="['Rinse containers before recycling','Remove caps and lids from bottles','Keep paper and cardboard dry','Flatten boxes to save space']" 
             :examples="['Plastic bottles','Glass jars','Aluminum cans','Cardboard boxes','Paper']"  bg-color="#3A83F6" />

             <CategoryCard :icon="IconMdiLeaf" title="Composable Waste" description="Organic materials that decompose naturally" 
             :tips="['Avoid meat and dairy in home compost','Cut larger items into smaller pieces','Balance green and brown materials','Keep compost moist but not soggy']" 
             :examples="['Fruit peels','Vegetable scraps','Coffee grounds','Eggshells','Leaves']"  bg-color="#22C45E" />

             <CategoryCard :icon="IconMdiBiohazard" title="Hazardous Waste" description="Materials that can be processed & reused" 
             :tips="['Never mix with regular trash','Keep in original containers when possible','Take to designated collection sites','Check local regulations for disposal']" 
             :examples="['Batteries','Paint','Electronics','Chemicals','Fluorescent bulbs']"  bg-color="#EE4444" />

             <CategoryCard :icon="IconMdiDeleteVariant" title="General Waste" description="Non-recyclable, non-hazardous items" 
             :tips="['Minimize general waste where possible','Consider reuse before disposal','Use designated waste bins','Bag waste securely to prevent spills']" 
             :examples="['Plastic wrap','Styrofoam','Broken ceramics','Disposable diapers']"  bg-color="#6B7281" />
         </div>

         <div class="flex flex-col w-full gap-2.5 p-6">
             <div class="flex gap-10">
                 <input class="border w-full p-2 bg-[#FEFFFF] rounded-xl border-[#809786] focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent 
                 appearance-none" type="search" name="" id="" placeholder=" 🔍 Search Waste Item...">
                 <select class="bg-[#FEFFFF] border rounded-xl border-[#809786] py-3 px-6 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent 
                 appearance-none" name="" id="">
                  <option value="" disabled selected>Select a category</option>
                  <option v-for="category in Categories" :key="category" :value="category">
                    {{ category }}
                  </option>
                 </select>
             </div>
             <div class="flex gap-3 items-center my-8">
                    <IconMdiTrashCanOutline class="h-15 w-15 text-green-500"/>
                    <p class="text-2xl font-bold">classification Results</p>
                </div>
             <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-10 mt-2">
                 <ItemCard 
                   :icon="IconMdiRecycle" 
                   image="https://d2j02ha532z66v.cloudfront.net/wp-content/uploads/2024/09/image-2-scaled.jpeg" 
                   category="Recycable" 
                   title="trash" 
                   accuracy="12" 
                   date="25/10/2025" 
                   time="18:03:22"
                 />

             </div>
         </div>
      </div>
    
      <footer>
      <p class="text-sm text-gray-500 text-center">© 2025 EcoSort AI. Help protect our planet, one item at a time.</p>
    </footer>
    
    </div>

   
        
</template>