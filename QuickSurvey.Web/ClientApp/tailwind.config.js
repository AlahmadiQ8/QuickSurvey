module.exports = {
  purge: {
    enabled: process?.argv?.indexOf("build") !== -1,
    content: [
      "./projects/*/src/**/*.html",
      "./projects/*/src/**/*.ts",
    ]
  },
  darkMode: false, // or 'media' or 'class'
  theme: {
    extend: {
      colors: {
        // --color-bg-primary
        "bgcolor": '#0d1117',

        // --color-border-primary
        "bordercolor": '#30363d',

        // --color-text-primary
        "textcolor": "#c9d1d9",

        // --color-state-focus-border
        "focusbordercolor": "#a371f7",

        // --color-text-link
        "linkcolor": "#bc8cff",

        // info box
        "infotextcolor": "#bc8cff",
        "infobordercolor": "rgba(140,139,253,0.4)",
        "infobackgroundcolor": " linear-gradient(rgba(140,139,253,0.1),rgba(140,139,253,0.1))",

        // error box
        "errorboxbordercolor": "rgba(253, 56, 65, 0.4)",
        "errorboxbgcolor": "rgba(253, 56, 65, 0.1)",
        "errortextcolor": "rgba(253, 56, 65, .8)"
      }
    },
  },
  variants: {
    extend: {
      opacity: ['disabled'],
      cursor: ['disabled'],
      borderWidth: ['disabled', 'focus']
    },
  },
  plugins: [],
}
