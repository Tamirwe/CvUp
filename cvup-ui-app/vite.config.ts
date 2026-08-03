import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3030,
    open: true,
  },
  build: {
    outDir: "build",
    rollupOptions: {
      output: {
        // Only the self-contained heavy leaves are split out. React, MUI and
        // emotion stay in the main chunk on purpose: separating emotion from
        // MUI reorders their module initialisation and breaks styling at
        // runtime in ways the build cannot catch.
        manualChunks: {
          pdf: [
            "pdfjs-dist",
            "@react-pdf-viewer/core",
            "@react-pdf-viewer/default-layout",
          ],
          charts: ["react-google-charts"],
          editor: ["react-quill"],
          docviewer: ["@cyntler/react-doc-viewer"],
        },
      },
    },
  },
});
