import daisyui from 'daisyui';

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./Pages/**/*.{cshtml,html,js}",
    "./wwwroot/js/**/*.js",
    "./wwwroot/**/*.html",
    "./Frontend/js/**/*.js"
  ],
  theme: {
    extend: {},
  },
  plugins: [daisyui],
}

