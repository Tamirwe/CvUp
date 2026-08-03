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
    // The entry chunk is react, mui, emotion, the router and the stores, which
    // will not shrink without splitting mui apart -- and that reorders module
    // initialisation in ways the build cannot catch. 600 kB raw is roughly
    // 175 kB over the wire, so this quiets a warning rather than hiding one.
    chunkSizeWarningLimit: 600,
    // No manualChunks on purpose. Naming a package there makes it a chunk
    // entry point, which overrides the dynamic-import boundary and drags it
    // back into the initial graph -- it was preloading the 587 kB pdf chunk on
    // the login screen. The lazy routes in Router.tsx split these far better.
  },
});
