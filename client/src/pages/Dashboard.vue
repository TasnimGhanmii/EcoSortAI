
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
import { ref, onMounted,computed } from 'vue';
import type { ClassificationItem } from '../types/ClassificationItem';
import jsPDF from 'jspdf';
import router from '../router';

const logout = () => {
  localStorage.removeItem('token')
  router.push('/auth')
}

const searchQuery = ref('');
const selectedCategory = ref('All Categories');

const filteredClassifications = computed(() => {
  const query = searchQuery.value.toLowerCase().trim();
  if (!query) return classifications.value.filter(item => 
    selectedCategory.value === 'All Categories' || item.category === selectedCategory.value
  );

  const queryWords = query.split(/\s+/); 
  return classifications.value.filter(item => {
    const title = item.title.toLowerCase();
    const matchesTitle = queryWords.every(word => title.includes(word));
    const matchesCategory =
      selectedCategory.value === 'All Categories' || item.category === selectedCategory.value;
    return matchesTitle && matchesCategory;
  });
});


const hasClassifications = ref(false);
const classifications = ref<ClassificationItem[]>([]);
const progress = ref({
  TotalItemsSorted: 0,
  RecyclableItems: 0,
  CompostableItems: 0,
  HazardousItems: 0,
  GeneralItems: 0,
  RecyclablePercentage: 0,
  CompostablePercentage: 0,
  HazardousPercentage: 0,
  GeneralPercentage: 0,
  EcoScore: 0,
  LastClassifiedAt: null as string | null
});

onMounted(async () => {
  const token = localStorage.getItem('token');
  if (!token) {
    // Redirect to login
    return;
  }

  try {
    // Fetch classifications
    const response = await fetch('/api/classifications', {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (!response.ok) throw new Error('Failed to fetch classifications');
    
    const rawItems = await response.json();
    classifications.value = rawItems.map((item: any) => ({
      ...item,
      icon: iconMap[item.icon] || iconMap.default
    }));
    hasClassifications.value = true;
  } catch (err) {
    console.error('Classification fetch error:', err);
  }

  try {
    // Fetch progress
    const progressResponse = await fetch('/api/classifications/progress', {
      headers: { 'Authorization': `Bearer ${token}` }
    });
    if (progressResponse.ok) {
      const data = await progressResponse.json();
      progress.value = {
        TotalItemsSorted: data.totalItemsSorted ?? 0,
        RecyclableItems: data.recyclableItems ?? 0,
        CompostableItems: data.compostableItems ?? 0,
        HazardousItems: data.hazardousItems ?? 0,
        GeneralItems: data.generalItems ?? 0,
        RecyclablePercentage: data.recyclablePercentage ?? 0,
        CompostablePercentage: data.compostablePercentage ?? 0,
        HazardousPercentage: data.hazardousPercentage ?? 0,
        GeneralPercentage: data.generalPercentage ?? 0,
        EcoScore: data.ecoScore ?? 0,
        LastClassifiedAt: data.lastClassifiedAt ?? null
      };   }
  } catch (err) {
    console.error('Progress fetch error:', err);
  }
});


const iconMap: Record<string, any> = {
  'mdi:recycleVariant': IconMdiRecycle,
  'mdi:leaf': IconMdiLeaf,
  'mdi:biohazard': IconMdiBiohazard,
  'mdi:deleteVariant': IconMdiDeleteVariant,
  'mdi:setAll': IconMdiSetAll,
  'mdi:sproutOutline': IconMdiSproutOutline,
  'mdi:trashCanOutline': IconMdiTrashCanOutline,
  default: IconMdiTrashCanOutline
};

const updateProgress = (newItem: any) => {
  // Increment total items
  progress.value.TotalItemsSorted += 1;

  // Increment the right category count
  switch (newItem.category) {
    case 'Recyclable':
      progress.value.RecyclableItems += 1;
      break;
    case 'Compostable':
      progress.value.CompostableItems += 1;
      break;
    case 'Hazardous':
      progress.value.HazardousItems += 1;
      break;
    case 'General':
      progress.value.GeneralItems += 1;
      break;
  }

  // Recalculate percentages
  const total = progress.value.TotalItemsSorted;
  progress.value.RecyclablePercentage = Math.round((progress.value.RecyclableItems / total) * 100);
  progress.value.CompostablePercentage = Math.round((progress.value.CompostableItems / total) * 100);
  progress.value.HazardousPercentage = Math.round((progress.value.HazardousItems / total) * 100);
  progress.value.GeneralPercentage = Math.round((progress.value.GeneralItems / total) * 100);

  // Recalculate EcoScore
  progress.value.EcoScore = Math.round(
    ((progress.value.RecyclableItems + progress.value.CompostableItems) / total) * 100
  );

  // Update last classified time
  progress.value.LastClassifiedAt = newItem.date; // or Date.now() depending on your backend format
};


const handleFileUpload = async (event: any) => {
  const file = event.target.files[0];
  if (!file) return;

  const token = localStorage.getItem('token');
  if (!token) {
    alert('Please log in first.');
    return;
  }

  const formData = new FormData();
  formData.append('image', file);

  try {
    const response = await fetch('/api/classifications', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(`Upload failed: ${error}`);
    }

    const result = await response.json();

    // ✅ Add the new item
    classifications.value.push(result);
    hasClassifications.value = true;

    // ✅ Update progress immediately
    updateProgress(result);

    alert('✅ Classification successful!');
  } catch (err) {
    console.error('Upload error:', err);
    alert('❌ Failed to classify image.');
  }
};

const exportReportPDF = () => {
  const doc = new jsPDF({
    orientation: 'portrait',
    unit: 'pt',
    format: 'a4'
  });

  // Title
  doc.setFontSize(22);
  doc.setTextColor('#28C762');
  doc.text('EcoSort AI - Waste Sorting Report', 40, 50);

  // Date
  const now = new Date();
  doc.setFontSize(12);
  doc.setTextColor('#555');
  doc.text(`Generated on: ${now.toLocaleString()}`, 40, 75);

  // Section: Overall Progress
  doc.setFontSize(16);
  doc.setTextColor('#000');
  doc.text('Your Overall Progress', 40, 110);

  // Draw some metrics as colored boxes
  const metrics = [
    { title: 'Total Items Sorted', value: progress.value.TotalItemsSorted, color: '#52DD84' },
    { title: 'Recyclable Items', value: progress.value.RecyclableItems, color: '#3A82F6' },
    { title: 'Compostable Items', value: progress.value.CompostableItems, color: '#F3DA5C' },
    { title: 'Hazardous Items', value: progress.value.HazardousItems, color: '#EF4444' },
    { title: 'General Items', value: progress.value.GeneralItems, color: '#6B7281' },
    { title: 'Eco Score', value: progress.value.EcoScore + '%', color: '#BDB9D5' }
  ];

  let yPosition = 140;
  const boxWidth = 200;
  const boxHeight = 50;
  let xPosition = 40;

  metrics.forEach((m, index) => {
    doc.setFillColor(m.color);
    doc.roundedRect(xPosition, yPosition, boxWidth, boxHeight, 5, 5, 'F');

    doc.setTextColor('#fff');
    doc.setFontSize(12);
    doc.text(`${m.title}: ${m.value}`, xPosition + 10, yPosition + 30);

    xPosition += boxWidth + 20;

    // Wrap to next row
    if ((index + 1) % 2 === 0) {
      xPosition = 40;
      yPosition += boxHeight + 20;
    }
  });

  // Section: Category Breakdown
  yPosition += 40;
  doc.setTextColor('#000');
  doc.setFontSize(16);
  doc.text('Category Breakdown (%)', 40, yPosition);

  yPosition += 20;
  const categoryPercentages = [
    { title: 'Recyclable', value: progress.value.RecyclablePercentage, color: '#3A82F6' },
    { title: 'Compostable', value: progress.value.CompostablePercentage, color: '#F59E0B' },
    { title: 'Hazardous', value: progress.value.HazardousPercentage, color: '#EF4444' },
    { title: 'General', value: progress.value.GeneralPercentage, color: '#6B7281' }
  ];

  let xCat = 40;
  categoryPercentages.forEach(c => {
    doc.setFillColor(c.color);
    doc.rect(xCat, yPosition, 100, 20, 'F');

    doc.setTextColor('#fff');
    doc.setFontSize(12);
    doc.text(`${c.title}: ${c.value}%`, xCat + 5, yPosition + 14);

    xCat += 120;
  });

  // Footer
  doc.setTextColor('#999');
  doc.setFontSize(10);
  doc.text('© 2025 EcoSort AI', 40, 780);

  // Save PDF
  doc.save('EcoSort_Report.pdf');
};






const Categories=['All Categories','Recyclable','Compost','Hazardous','General']

</script>


<template>
    <div class="flex flex-col bg-[#F2F9F4]">
      <div class="bg-[#28C762] justify-items-center -my-6.5 p-5 pt-30 pb-20">
        <button
    @click="logout"
    class="text-white font-medium bg-red-500 hover:bg-red-600 px-4 py-2 rounded-lg transition-all duration-300 ease-in-out"
  >
    Logout
  </button>
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
     <IconMdiUpload class="h-15 w-15 text-[#21C55C]"/>
   </div>
   <p class="font-semibold text-lg mt-5 mb-1">Upload Waste Image</p>
   <p class="text-sm text-gray-400">Drag and drop an image here, or click to select</p>

   <input 
     type="file" 
     accept=".jpg,.jpeg,.png" 
     @change="handleFileUpload" 
     class="hidden" 
     id="uploadInput"
   />
   <label 
     for="uploadInput" 
     class="text-white font-medium cursor-pointer rounded-lg bg-gradient-to-r from-[#25C55E] to-[#63E392] p-2 px-6 my-6 active:scale-95 transition-all duration-300 ease-in-out"
   >
     Select Image
   </label>

   <p class="text-xs text-gray-400">Supports JPG, PNG, WEBP up to 10MB</p>
</div>

      <div v-if="hasClassifications" class="flex flex-col mx-auto my-8 p-2  w-[70%]">
          
          <div class="flex justify-between mb-10">
               <div>
                   <p class="font-bold text-4xl">Your Impact</p>
                   <p class="text-sm text-gray-500">Track your waste sorting progress and environmental contribution</p>
               </div>

               <button @click="exportReportPDF" class="bg-[#2AC864] cursor-pointer h-10 my-auto px-8 text-xs text-white font-medium rounded-2xl flex items-center gap-1 active:scale-95 transition duration-300 ease-in-out"><IconMdiFileExport/>Export Report</button>
          </div>
        
          <div class=" flex flex-wrap md:flex-nowrap  gap-6">
                           <ImpactCard 
                :icon="IconMdiSetAll" 
                :number="progress.TotalItemsSorted" 
                title="Total Items Sorted" 
                bg-color="#52DD84" />
              
              <ImpactCard 
                :icon="IconMdiRecycle" 
                :number="progress.RecyclableItems" 
                title="Recyclable Items" 
                bg-color="#3A82F6" />
              
              <ImpactCard 
                :icon="IconMdiLeaf" 
                :number="progress.CompostableItems" 
                title="Compostable Items" 
                bg-color="#F3DA5C" />
              
              <ImpactCard 
                :icon="IconMdiSproutOutline" 
                :number="progress.EcoScore" 
                title="Eco Score" 
                bg-color="#BDB9D5" />
          </div>

          <div class="flex flex-col w-full p-4 md:p-10 border mt-13 rounded-3xl border-[#E2EDE5] bg-[#FEFFFF] shadow-[0_0_15px_5px_rgba(209,250,229,0.7)] ">
            <p class="font-bold text-2xl mb-8 text-center">Waste Category Breakdown</p>
            
            <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6">
                             <StatisticRing 
                 :percentage="progress.RecyclablePercentage?? 0" 
                 :nb-items="String(progress.RecyclableItems?? 0)" 
                 title="Recyclable" 
                 color="green"/>

               <StatisticRing 
                 :percentage="progress.CompostablePercentage?? 0" 
                 :nb-items="String(progress.CompostableItems?? 0)" 
                 title="Compostable" 
                 color="amber"/>

               <StatisticRing 
                 :percentage="progress.HazardousPercentage?? 0" 
                 :nb-items="String(progress.HazardousItems?? 0)" 
                 title="Hazardous" 
                 color="red"/>

               <StatisticRing 
                 :percentage="progress.GeneralPercentage?? 0" 
                 :nb-items="String(progress.GeneralItems?? 0)" 
                 title="General" 
                 color="gray"/>
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
                 appearance-none" type="search" name=""  v-model="searchQuery" id="" placeholder=" 🔍 Search Waste Item...">
                 <select v-model="selectedCategory"
  class="bg-[#FEFFFF] border rounded-xl border-[#809786] py-3 px-6 focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent"
>
  <option value="All Categories">All Categories</option>
  <option v-for="category in Categories.slice(1)" :key="category" :value="category">
    {{ category }}
  </option>
</select>
             </div>
             <div class="flex gap-3 items-center my-8">
                    <IconMdiTrashCanOutline class="h-15 w-15 text-green-500"/>
                    <p class="text-2xl font-bold">classification Results</p>
                </div>
             <div class="grid grid-cols-1 sm:grid-cols-1 md:grid-cols-1 gap-10 mt-2">
                 <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-10 mt-2">
  <ItemCard 
    v-for="item in filteredClassifications"
    :key="item.id"
    :icon="item.icon"
    :image="item.image"
    :category="item.category"
    :title="item.title"
    :accuracy="item.accuracy"
    :date="item.date"
    :time="item.time"
  />
</div>

             </div>
         </div>
      </div>
    
      <footer>
      <p class="text-sm text-gray-500 text-center">© 2025 EcoSort AI. Help protect our planet, one item at a time.</p>
    </footer>
    
    </div>

   
        
</template>