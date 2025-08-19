import { defineConfig } from 'vite';
import path from 'path';

export default defineConfig({
    root: './',
    base: './',
    build: {
        outDir: './wwwroot/dist',
        minify: 'esbuild',
        emptyOutDir: true,
        manifest: 'manifest.json',
        rollupOptions: {
            input: {
                main: path.resolve(__dirname, 'Frontend/js/main.js'),
                style: path.resolve(__dirname, 'Frontend/css/style.css')
            }
        }
    }
});