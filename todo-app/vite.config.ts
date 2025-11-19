import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    // proxy API requests to your .NET backend if needed:
    proxy: {
      "/api": {
        target: "http://localhost:5000",
        changeOrigin: true,
        secure: false,
      },
    },
  },
  build: {
    outDir: "build", // keeps CRA-style output folder
  },
  resolve: {
    alias: {
      "@": "/src",
    },
  },
});
