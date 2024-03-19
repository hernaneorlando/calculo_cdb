/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        'cor-primaria': '#001B74',
        'cor-primaria-var': '#5062B6',
        'cor-secundaria': '#FFD862',
        'cor-secundaria-var': '#FFC536',
        'preto-placeholder': '#A4A4A4'
      },
      fontFamily: {
        mulish: ['"Mulish"', 'sans-serif']
      }
    },
  },
  plugins: [],
}

