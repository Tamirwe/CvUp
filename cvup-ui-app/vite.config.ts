import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3030,
    // Fail rather than slide onto the next free port. The origin is part of the
    // localStorage key, so a silent move to 3031 hands the app an empty
    // settings bucket and looks like a broken login instead of a busy port.
    strictPort: true,
    open: true,
  },
  preview: {
    // 4173, vite's default, is not in the API's CORS allowlist; 3031 is.
    port: 3031,
    strictPort: true,
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
