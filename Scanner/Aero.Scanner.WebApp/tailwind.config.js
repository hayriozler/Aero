/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: "class",
    content: [
        "./Components/**/*.{razor,html}",
        "./wwwroot/index.html"
    ],
    theme: {
        extend: {
            colors: {
                // All colors map to CSS custom properties.
                // RGB channel format lets Tailwind opacity modifiers work (e.g. bg-primary/20).
                "primary":                   "rgb(var(--primary) / <alpha-value>)",
                "primary-container":         "rgb(var(--primary-container) / <alpha-value>)",
                "primary-dim":               "rgb(var(--primary-dim) / <alpha-value>)",
                "on-primary":                "rgb(var(--on-primary) / <alpha-value>)",
                "secondary":                 "rgb(var(--secondary) / <alpha-value>)",
                "secondary-container":       "rgb(var(--secondary-container) / <alpha-value>)",
                "secondary-dim":             "rgb(var(--secondary-dim) / <alpha-value>)",
                "on-secondary":              "rgb(var(--on-secondary) / <alpha-value>)",
                "tertiary":                  "rgb(var(--tertiary) / <alpha-value>)",
                "tertiary-container":        "rgb(var(--tertiary-container) / <alpha-value>)",
                "on-tertiary":               "rgb(var(--on-tertiary) / <alpha-value>)",
                "on-tertiary-container":     "rgb(var(--on-tertiary-container) / <alpha-value>)",
                "background":                "rgb(var(--background) / <alpha-value>)",
                "on-background":             "rgb(var(--on-background) / <alpha-value>)",
                "surface":                   "rgb(var(--surface) / <alpha-value>)",
                "on-surface":                "rgb(var(--on-surface) / <alpha-value>)",
                "surface-dim":               "rgb(var(--surface-dim) / <alpha-value>)",
                "surface-bright":            "rgb(var(--surface-bright) / <alpha-value>)",
                "surface-variant":           "rgb(var(--surface-variant) / <alpha-value>)",
                "on-surface-variant":        "rgb(var(--on-surface-variant) / <alpha-value>)",
                "surface-container-lowest":  "rgb(var(--surface-container-lowest) / <alpha-value>)",
                "surface-container-low":     "rgb(var(--surface-container-low) / <alpha-value>)",
                "surface-container":         "rgb(var(--surface-container) / <alpha-value>)",
                "surface-container-high":    "rgb(var(--surface-container-high) / <alpha-value>)",
                "surface-container-highest": "rgb(var(--surface-container-highest) / <alpha-value>)",
                "outline":                   "rgb(var(--outline) / <alpha-value>)",
                "outline-variant":           "rgb(var(--outline-variant) / <alpha-value>)",
                "error":                     "rgb(var(--error) / <alpha-value>)",
                "error-container":           "rgb(var(--error-container) / <alpha-value>)",
                "on-error":                  "rgb(var(--on-error) / <alpha-value>)",
                "on-error-container":        "rgb(var(--on-error-container) / <alpha-value>)",
                "inverse-surface":           "rgb(var(--inverse-surface) / <alpha-value>)",
                "inverse-on-surface":        "rgb(var(--inverse-on-surface) / <alpha-value>)",
                "inverse-primary":           "rgb(var(--inverse-primary) / <alpha-value>)",
            },
            borderRadius: {
                DEFAULT: "0.125rem",
                lg: "0.25rem",
                xl: "0.5rem",
                full: "0.75rem"
            },
            fontFamily: {
                headline: ["Space Grotesk", "sans-serif"],
                body:     ["Inter", "sans-serif"],
                label:    ["Inter", "sans-serif"]
            }
        }
    },
    plugins: [
        require("@tailwindcss/forms"),
        require("@tailwindcss/container-queries")
    ]
}
