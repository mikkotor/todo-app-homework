import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import fs from "fs";
import path from "path";

const certDir = path.resolve(__dirname, "certs");
const certPath = path.join(certDir, "localhost.crt");
const keyPath = path.join(certDir, "localhost.key");

function loadHttps() {
  try {
    if (fs.existsSync(certPath) && fs.existsSync(keyPath)) {
      return {
        key: fs.readFileSync(keyPath),
        cert: fs.readFileSync(certPath),
      };
    }
  } catch (e) {
    // fall through to disabling https
  }
  return undefined;
}

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    https: loadHttps(),
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
