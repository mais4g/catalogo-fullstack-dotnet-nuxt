<template>
  <v-dialog
    v-model="open"
    max-width="600"
    persistent
    role="dialog"
    aria-modal="true"
    :aria-labelledby="tituloId"
  >
    <v-card>
      <v-card-title :id="tituloId" class="text-h6">
        {{ modoEdicao ? 'Editar categoria' : 'Nova categoria' }}
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
            rows="3"
            counter="1000"
          />
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
          :disabled="!nomeValido || salvando"
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
import type { Categoria, CategoriaInput, ApiError } from '~/types/api'

const props = defineProps<{
  modelValue: boolean
  categoria: Categoria | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  saved: [categoria: Categoria]
}>()

const api = useApi()
const toast = useToast()

const open = computed({
  get: () => props.modelValue,
  set: (v) => emit('update:modelValue', v)
})

const modoEdicao = computed(() => props.categoria !== null)
const tituloId = useId()
const ajudaNomeId = useId()

const form = ref<CategoriaInput>({ nome: '', descricao: '' })
const salvando = ref(false)
const erroNome = ref<string[]>([])

const nomeValido = computed(() => (form.value.nome ?? '').trim().length >= 5)

const regraNomeObrigatorio = (v: string) => !!v?.trim() || 'O nome é obrigatório.'
const regraNomeMinimo = (v: string) => (v?.length ?? 0) >= 5 || 'Mínimo de 5 caracteres.'
const regraNomeMaximo = (v: string) => (v?.length ?? 0) <= 200 || 'Máximo de 200 caracteres.'

watch(() => props.modelValue, (novoValor) => {
  if (novoValor) {
    erroNome.value = []
    if (props.categoria) {
      form.value = {
        nome: props.categoria.nome,
        descricao: props.categoria.descricao ?? ''
      }
    } else {
      form.value = { nome: '', descricao: '' }
    }
  }
})

const fechar = () => {
  open.value = false
}

const salvar = async () => {
  if (!nomeValido.value) return
  salvando.value = true
  erroNome.value = []

  try {
    let salva: Categoria
    if (modoEdicao.value && props.categoria) {
      salva = await api.put<Categoria>(`/api/categorias/${props.categoria.id}`, form.value)
      toast.success('Categoria atualizada com sucesso.')
    } else {
      salva = await api.post<Categoria>('/api/categorias', form.value)
      toast.success('Categoria criada com sucesso.')
    }
    emit('saved', salva)
    open.value = false
  } catch (error: any) {
    const data = error?.data as ApiError | undefined
    if (data?.errors?.Nome) {
      erroNome.value = data.errors.Nome
    } else if (data?.mensagem) {
      toast.error(data.mensagem)
    } else {
      toast.error('Não foi possível salvar a categoria. Tente novamente.')
    }
  } finally {
    salvando.value = false
  }
}
</script>
