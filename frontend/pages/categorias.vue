<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-2">
      <h1 class="text-h4">Categorias</h1>
      <v-btn
        color="primary"
        prepend-icon="mdi-plus"
        @click="abrirNovo"
      >
        Nova categoria
      </v-btn>
    </div>

    <v-card variant="outlined">
      <v-data-table
        :headers="headers"
        :items="categorias"
        :loading="carregando"
        :items-per-page="10"
        no-data-text="Nenhuma categoria cadastrada."
        loading-text="Carregando categorias..."
        aria-label="Tabela de categorias"
      >
        <template #[`item.descricao`]="{ item }">
          <span class="text-medium-emphasis">{{ item.descricao || '—' }}</span>
        </template>

        <template #[`item.acoes`]="{ item }">
          <v-btn
            icon="mdi-pencil"
            variant="text"
            size="small"
            color="primary"
            :aria-label="`Editar categoria ${item.nome}`"
            @click="abrirEdicao(item)"
          />
          <v-btn
            icon="mdi-delete"
            variant="text"
            size="small"
            color="error"
            :aria-label="`Excluir categoria ${item.nome}`"
            @click="prepararExclusao(item)"
          />
        </template>
      </v-data-table>
    </v-card>

    <CategoriaFormDialog
      v-model="dialogForm"
      :categoria="categoriaSelecionada"
      @saved="onSalvou"
    />

    <ConfirmDialog
      v-model="dialogConfirm"
      titulo="Excluir categoria"
      :mensagem="mensagemExclusao"
      @confirm="excluir"
    />
  </div>
</template>

<script setup lang="ts">
import type { Categoria, ApiError } from '~/types/api'

const api = useApi()
const toast = useToast()

const categorias = ref<Categoria[]>([])
const carregando = ref(false)
const dialogForm = ref(false)
const dialogConfirm = ref(false)
const categoriaSelecionada = ref<Categoria | null>(null)

const headers = [
  { title: 'ID', key: 'id', sortable: true, width: 80 },
  { title: 'Nome', key: 'nome', sortable: true },
  { title: 'Descrição', key: 'descricao', sortable: false },
  { title: 'Ações', key: 'acoes', sortable: false, align: 'end' as const, width: 140 }
]

const mensagemExclusao = computed(() =>
  categoriaSelecionada.value
    ? `Tem certeza que deseja excluir a categoria "${categoriaSelecionada.value.nome}"? Esta ação não pode ser desfeita.`
    : ''
)

const carregar = async () => {
  carregando.value = true
  try {
    categorias.value = await api.get<Categoria[]>('/api/categorias')
  } catch {
    toast.error('Não foi possível carregar a lista de categorias.')
  } finally {
    carregando.value = false
  }
}

const abrirNovo = () => {
  categoriaSelecionada.value = null
  dialogForm.value = true
}

const abrirEdicao = (categoria: Categoria) => {
  categoriaSelecionada.value = categoria
  dialogForm.value = true
}

const prepararExclusao = (categoria: Categoria) => {
  categoriaSelecionada.value = categoria
  dialogConfirm.value = true
}

const onSalvou = (categoria: Categoria) => {
  const idx = categorias.value.findIndex(c => c.id === categoria.id)
  if (idx >= 0) {
    categorias.value[idx] = categoria
  } else {
    categorias.value.push(categoria)
  }
}

const excluir = async () => {
  if (!categoriaSelecionada.value) return
  const id = categoriaSelecionada.value.id
  try {
    await api.delete(`/api/categorias/${id}`)
    categorias.value = categorias.value.filter(c => c.id !== id)
    toast.success('Categoria excluída com sucesso.')
  } catch (error: any) {
    const data = error?.data as ApiError | undefined
    toast.error(data?.mensagem || 'Não foi possível excluir a categoria.')
  } finally {
    categoriaSelecionada.value = null
  }
}

onMounted(carregar)
</script>
