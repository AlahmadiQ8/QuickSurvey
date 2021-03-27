module.exports = {
  purge: {
    enabled: process?.argv?.indexOf("build") !== -1,
    content: [
      "./src/**/*.html",
      "./src/**/*.ts",
    ]
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
