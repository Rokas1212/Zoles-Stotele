import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

// Production-only configuration
export default defineConfig({
  plugins: [plugin()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  },
  // No proxy or HTTPS in production, 
  // as the frontend and backend will run on the same server/container.
  server: {
    port: 5173
  }
});
