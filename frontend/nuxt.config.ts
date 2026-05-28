export default defineNuxtConfig({
  compatibilityDate: '2025-01-01',
  devtools: { enabled: true },
  modules: ['vuetify-nuxt-module'],

  app: {
    head: {
      title: 'Catálogo — Gestão de Produtos e Categorias',
      htmlAttrs: { lang: 'pt-BR' },
      meta: [
        { charset: 'utf-8' },
        { name: 'viewport', content: 'width=device-width, initial-scale=1' },
        { name: 'description', content: 'Sistema de gestão de catálogo de produtos e categorias' }
      ]
    }
  },

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE || 'http://localhost:5000'
    }
  },

  vuetify: {
    moduleOptions: {},
    vuetifyOptions: {
      theme: {
        defaultTheme: 'menta',
        themes: {
          menta: {
            dark: false,
            colors: {
              primary: '#00897B',
              secondary: '#4DB6AC',
              accent: '#80CBC4',
              background: '#F5F9F7',
              surface: '#FFFFFF',
              error: '#D32F2F',
              success: '#388E3C',
              warning: '#F57C00',
              info: '#0288D1',
              'on-primary': '#FFFFFF',
              'on-secondary': '#1F2937',
              'on-background': '#1F2937',
              'on-surface': '#1F2937',
              'on-error': '#FFFFFF',
              'on-success': '#FFFFFF'
            }
          }
        }
      },
      defaults: {
        VBtn: { variant: 'flat' },
        VTextField: { variant: 'outlined', density: 'comfortable' },
        VSelect: { variant: 'outlined', density: 'comfortable' },
        VTextarea: { variant: 'outlined', density: 'comfortable' }
      }
    }
  }
})
