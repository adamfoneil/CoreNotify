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
      colors: {
          transparent: 'transparent',
          current: 'currentColor',
          black: colors.black,
          white: colors.white,
          gray: colors.trueGray,
          indigo: colors.indigo,
          red: colors.rose,
          yellow: colors.amber,
          teal: colors.teal,
          orange: colors.orange,
          green: colors.green
      }
  },
  variants: {
    extend: {},
  },
  plugins: []
}
