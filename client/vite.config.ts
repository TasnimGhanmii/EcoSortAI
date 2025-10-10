import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite'
import Icons from 'unplugin-icons/vite'
import IconsResolver from 'unplugin-icons/resolver'
// https://vite.dev/config/
export default defineConfig({
  plugins: [vue(),
    tailwindcss(),
    AutoImport({
      resolvers: [
        // Auto-import icon APIs (like `defineComponent` for icons)
        IconsResolver({
          componentPrefix: 'Icon', // Optional: prefix for auto-imported icons
        }),
      ],
    }),
    Components({
      resolvers: [
        // Automatically register icons as components
        IconsResolver({
          componentPrefix: 'Icon',
        }),
      ],
    }),

    // Load icons
    Icons({
      autoInstall: true, // Automatically install missing icon sets
    }),
  ],
  
  
  
})
