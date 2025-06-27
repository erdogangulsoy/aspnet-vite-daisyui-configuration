import { defineConfig } from 'vite';
import path from 'path';

export default defineConfig({
    base: './',
    build: {
        outDir: './wwwroot',
        minify: 'esbuild',
        emptyOutDir: true,
        manifest: true,
        rollupOptions: {
            input: {
                main: path.resolve(__dirname, 'Frontend/js/main.js'),
                style: path.resolve(__dirname, 'Frontend/css/style.css')
            }
        }
    }
});