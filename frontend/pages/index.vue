<template>
  <div>
    <h1 class="text-h4 mb-6">Bem-vinda(o) ao Catálogo</h1>

    <v-row>
      <v-col cols="12" md="6">
        <v-card
          to="/categorias"
          variant="outlined"
          class="pa-6 h-100"
          aria-label="Acessar gerenciamento de categorias"
        >
          <v-icon icon="mdi-tag-multiple" size="48" color="primary" aria-hidden="true" />
          <h2 class="text-h5 mt-4 mb-2">Categorias</h2>
          <p class="text-body-2">
            Cadastre e organize as categorias que agrupam os produtos do catálogo.
          </p>
        </v-card>
      </v-col>

      <v-col cols="12" md="6">
        <v-card
          to="/produtos"
          variant="outlined"
          class="pa-6 h-100"
          aria-label="Acessar gerenciamento de produtos"
        >
          <v-icon icon="mdi-cube-outline" size="48" color="primary" aria-hidden="true" />
          <h2 class="text-h5 mt-4 mb-2">Produtos</h2>
          <p class="text-body-2">
            Cadastre produtos vinculados a uma categoria, com preço e descrição.
          </p>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>
