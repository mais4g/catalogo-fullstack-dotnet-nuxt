<template>
  <div>
    <div class="d-flex align-center justify-space-between mb-6 flex-wrap ga-2">
      <h1 class="text-h4">Produtos</h1>
      <v-btn
        color="primary"
        prepend-icon="mdi-plus"
        :disabled="categorias.length === 0"
        @click="abrirNovo"
      >
        Novo produto
      </v-btn>
    </div>

    <v-alert
      v-if="categorias.length === 0 && !carregandoCategorias"
      type="warning"
      variant="tonal"
      class="mb-4"
    >
      Cadastre ao menos uma categoria antes de criar produtos.
      <template #append>
        <v-btn to="/categorias" variant="text" color="warning">
          Ir para Categorias
        </v-btn>
      </template>
    </v-alert>

    <v-card variant="outlined">
      <v-data-table
        :headers="headers"
        :items="produtos"
        :loading="carregando"
        :items-per-page="10"
        no-data-text="Nenhum produto cadastrado."
        loading-text="Carregando produtos..."
        aria-label="Tabela de produtos"
      >
        <template #[`item.preco`]="{ item }">
          {{ formatarPreco(item.preco) }}
        </template>

        <template #[`item.categoria`]="{ item }">
          <v-chip size="small" color="primary" variant="tonal">
            {{ item.categoria?.nome ?? '—' }}
          </v-chip>
        </template>

        <template #[`item.descricao`]="{ item }">
          <span class="text-medium-emphasis">{{ item.descricao || '—' }}</span>
        </template>

        <template #[`item.acoes`]="{ item }">
          <v-btn
            icon="mdi-pencil"
            variant="text"
            size="small"
            color="primary"
            :aria-label="`Editar produto ${item.nome}`"
            @click="abrirEdicao(item)"
          />
          <v-btn
            icon="mdi-delete"
            variant="text"
            size="small"
            color="error"
            :aria-label="`Excluir produto ${item.nome}`"
            @click="prepararExclusao(item)"
          />
        </template>
      </v-data-table>
    </v-card>

    <ProdutoFormDialog
      v-model="dialogForm"
      :produto="produtoSelecionado"
      :categorias="categorias"
      :carregando-categorias="carregandoCategorias"
      @saved="onSalvou"
    />

    <ConfirmDialog
      v-model="dialogConfirm"
      titulo="Excluir produto"
      :mensagem="mensagemExclusao"
      @confirm="excluir"
    />
  </div>
</template>

<script setup lang="ts">
import type { Produto, Categoria, ApiError } from '~/types/api'

const api = useApi()
const toast = useToast()

const produtos = ref<Produto[]>([])
const categorias = ref<Categoria[]>([])
const carregando = ref(false)
const carregandoCategorias = ref(false)
const dialogForm = ref(false)
const dialogConfirm = ref(false)
const produtoSelecionado = ref<Produto | null>(null)

const headers = [
  { title: 'ID', key: 'id', sortable: true, width: 80 },
  { title: 'Nome', key: 'nome', sortable: true },
  { title: 'Categoria', key: 'categoria', sortable: false },
  { title: 'Preço', key: 'preco', sortable: true, align: 'end' as const, width: 140 },
  { title: 'Descrição', key: 'descricao', sortable: false },
  { title: 'Ações', key: 'acoes', sortable: false, align: 'end' as const, width: 140 }
]

const mensagemExclusao = computed(() =>
  produtoSelecionado.value
    ? `Tem certeza que deseja excluir o produto "${produtoSelecionado.value.nome}"? Esta ação não pode ser desfeita.`
    : ''
)

const formatarPreco = (valor: number) =>
  Number(valor).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

const carregarProdutos = async () => {
  carregando.value = true
  try {
    produtos.value = await api.get<Produto[]>('/api/produtos')
  } catch {
    toast.error('Não foi possível carregar a lista de produtos.')
  } finally {
    carregando.value = false
  }
}

const carregarCategorias = async () => {
  carregandoCategorias.value = true
  try {
    categorias.value = await api.get<Categoria[]>('/api/categorias')
  } catch {
    toast.error('Não foi possível carregar a lista de categorias.')
  } finally {
    carregandoCategorias.value = false
  }
}

const abrirNovo = () => {
  produtoSelecionado.value = null
  dialogForm.value = true
}

const abrirEdicao = (produto: Produto) => {
  produtoSelecionado.value = produto
  dialogForm.value = true
}

const prepararExclusao = (produto: Produto) => {
  produtoSelecionado.value = produto
  dialogConfirm.value = true
}

const onSalvou = (produto: Produto) => {
  const idx = produtos.value.findIndex(p => p.id === produto.id)
  if (idx >= 0) {
    produtos.value[idx] = produto
  } else {
    produtos.value.push(produto)
  }
}

const excluir = async () => {
  if (!produtoSelecionado.value) return
  const id = produtoSelecionado.value.id
  try {
    await api.delete(`/api/produtos/${id}`)
    produtos.value = produtos.value.filter(p => p.id !== id)
    toast.success('Produto excluído com sucesso.')
  } catch (error: any) {
    const data = error?.data as ApiError | undefined
    toast.error(data?.mensagem || 'Não foi possível excluir o produto.')
  } finally {
    produtoSelecionado.value = null
  }
}

onMounted(() => {
  carregarProdutos()
  carregarCategorias()
})
</script>
