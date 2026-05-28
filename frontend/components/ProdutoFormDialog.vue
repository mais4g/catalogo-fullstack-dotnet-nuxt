<template>
  <v-dialog
    v-model="open"
    max-width="640"
    persistent
    role="dialog"
    aria-modal="true"
    :aria-labelledby="tituloId"
  >
    <v-card>
      <v-card-title :id="tituloId" class="text-h6">
        {{ modoEdicao ? 'Editar produto' : 'Novo produto' }}
      </v-card-title>

      <v-card-text>
        <v-form @submit.prevent="salvar">
          <v-text-field
            v-model="form.nome"
            label="Nome"
            :rules="[regraNomeObrigatorio, regraNomeMinimo, regraNomeMaximo]"
            :error-messages="erroNome"
            :aria-describedby="ajudaNomeId"
            required
            autofocus
            counter="200"
          />
          <p :id="ajudaNomeId" class="text-caption text-medium-emphasis mb-2">
            Mínimo de 5 caracteres. Obrigatório.
          </p>

          <v-textarea
            v-model="form.descricao"
            label="Descrição"
            rows="2"
            counter="1000"
          />

          <v-row dense>
            <v-col cols="12" sm="6">
              <v-text-field
                v-model.number="form.preco"
                label="Preço (R$)"
                type="number"
                step="0.01"
                min="0.01"
                :rules="[regraPrecoPositivo]"
                required
                prefix="R$"
              />
            </v-col>
            <v-col cols="12" sm="6">
              <v-select
                v-model="form.categoriaId"
                :items="categorias"
                item-title="nome"
                item-value="id"
                label="Categoria"
                :rules="[regraCategoriaObrigatoria]"
                :loading="carregandoCategorias"
                :aria-busy="carregandoCategorias"
                required
                no-data-text="Nenhuma categoria cadastrada"
              />
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <v-card-actions class="px-4 pb-4">
        <v-spacer />
        <v-btn
          variant="text"
          :disabled="salvando"
          @click="fechar"
        >
          Cancelar
        </v-btn>
        <v-btn
          color="primary"
          variant="flat"
          :disabled="!formValido || salvando"
          :loading="salvando"
          :aria-busy="salvando"
          @click="salvar"
        >
          Salvar
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import type { Produto, ProdutoInput, Categoria, ApiError } from '~/types/api'

const props = defineProps<{
  modelValue: boolean
  produto: Produto | null
  categorias: Categoria[]
  carregandoCategorias: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  saved: [produto: Produto]
}>()

const api = useApi()
const toast = useToast()

const open = computed({
  get: () => props.modelValue,
  set: (v) => emit('update:modelValue', v)
})

const modoEdicao = computed(() => props.produto !== null)
const tituloId = useId()
const ajudaNomeId = useId()

const form = ref<ProdutoInput>({
  nome: '',
  descricao: '',
  preco: 0,
  categoriaId: 0
})

const salvando = ref(false)
const erroNome = ref<string[]>([])

const nomeValido = computed(() => (form.value.nome ?? '').trim().length >= 5)
const precoValido = computed(() => Number(form.value.preco) > 0)
const categoriaValida = computed(() => Number(form.value.categoriaId) > 0)
const formValido = computed(() => nomeValido.value && precoValido.value && categoriaValida.value)

const regraNomeObrigatorio = (v: string) => !!v?.trim() || 'O nome é obrigatório.'
const regraNomeMinimo = (v: string) => (v?.length ?? 0) >= 5 || 'Mínimo de 5 caracteres.'
const regraNomeMaximo = (v: string) => (v?.length ?? 0) <= 200 || 'Máximo de 200 caracteres.'
const regraPrecoPositivo = (v: number) => Number(v) > 0 || 'O preço deve ser maior que zero.'
const regraCategoriaObrigatoria = (v: number) => Number(v) > 0 || 'Selecione uma categoria.'

watch(() => props.modelValue, (novoValor) => {
  if (novoValor) {
    erroNome.value = []
    if (props.produto) {
      form.value = {
        nome: props.produto.nome,
        descricao: props.produto.descricao ?? '',
        preco: props.produto.preco,
        categoriaId: props.produto.categoriaId
      }
    } else {
      form.value = { nome: '', descricao: '', preco: 0, categoriaId: 0 }
    }
  }
})

const fechar = () => {
  open.value = false
}

const salvar = async () => {
  if (!formValido.value) return
  salvando.value = true
  erroNome.value = []

  try {
    let salvo: Produto
    if (modoEdicao.value && props.produto) {
      salvo = await api.put<Produto>(`/api/produtos/${props.produto.id}`, form.value as unknown as Record<string, unknown>)
      toast.success('Produto atualizado com sucesso.')
    } else {
      salvo = await api.post<Produto>('/api/produtos', form.value as unknown as Record<string, unknown>)
      toast.success('Produto criado com sucesso.')
    }
    emit('saved', salvo)
    open.value = false
  } catch (error: any) {
    const data = error?.data as ApiError | undefined
    if (data?.errors?.Nome) {
      erroNome.value = data.errors.Nome
    } else if (data?.mensagem) {
      toast.error(data.mensagem)
    } else {
      toast.error('Não foi possível salvar o produto. Tente novamente.')
    }
  } finally {
    salvando.value = false
  }
}
</script>
