import { dirname, resolve } from "path";
import { defineConfig } from "vite";
import { fileURLToPath } from "url";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

export default defineConfig({
  base: "/dist/",
  css: {
    preprocessorOptions: {
      scss: {
        // Bootstrap still emits Sass deprecations internally; keep app warnings visible.
        quietDeps: true,
        silenceDeprecations: ["legacy-js-api", "import", "global-builtin", "color-functions", "if-function"]
      }
    }
  },
  build: {
    outDir: resolve(__dirname, "..", "wwwroot", "dist"),
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      input: {
        // Register every TS entry here (src/ts/<feature>/<feature>.ts).
        main: resolve(__dirname, 'src/ts/main.ts'),
      },
      output: {
        entryFileNames: "[name].entry.js",
        assetFileNames: (assetInfo) => {
          if (assetInfo.name && assetInfo.name.endsWith(".css")) {
            return "[name].css";
          }
          return "[name].[ext]";
        }
      }
    }
  },
  resolve: {
    extensions: [".ts", ".js"]
  }
});
