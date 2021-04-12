const colors = require('tailwindcss/colors');
module.exports = {
  purge: {
    enabled: true,
    content: [
        './**/*.html',
        './**/*.razor'
    ],
  },
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {},
  },
  variants: {
    extend: {},
  },
  plugins: [],
}
