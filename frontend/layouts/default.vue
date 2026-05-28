<template>
  <v-app>
    <a href="#main-content" class="skip-link">Ir para o conteúdo principal</a>

    <v-app-bar color="primary" :elevation="2" role="banner">
      <v-app-bar-title>
        <v-icon icon="mdi-package-variant" class="me-2" aria-hidden="true" />
        Catálogo
      </v-app-bar-title>

      <template #append>
        <v-btn
          to="/categorias"
          prepend-icon="mdi-tag-multiple"
          variant="text"
          color="white"
          aria-label="Ir para gerenciamento de categorias"
        >
          Categorias
        </v-btn>
        <v-btn
          to="/produtos"
          prepend-icon="mdi-cube-outline"
          variant="text"
          color="white"
          aria-label="Ir para gerenciamento de produtos"
        >
          Produtos
        </v-btn>
      </template>
    </v-app-bar>

    <v-main id="main-content" tag="main">
      <v-container>
        <slot />
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
.skip-link {
  position: absolute;
  top: -48px;
  left: 8px;
  background: #1f2937;
  color: #ffffff;
  padding: 8px 16px;
  border-radius: 4px;
  z-index: 9999;
  text-decoration: none;
  font-weight: 600;
  transition: top 0.15s ease-in-out;
}

.skip-link:focus {
  top: 8px;
  outline: 3px solid #4db6ac;
  outline-offset: 2px;
}
</style>
