<template>
  <v-dialog
    v-model="open"
    max-width="500"
    persistent
    role="alertdialog"
    aria-modal="true"
    :aria-labelledby="tituloId"
    :aria-describedby="mensagemId"
  >
    <v-card>
      <v-card-title :id="tituloId" class="text-h6">
        <v-icon icon="mdi-alert-circle-outline" color="warning" class="me-2" aria-hidden="true" />
        {{ titulo }}
      </v-card-title>

      <v-card-text :id="mensagemId">
        {{ mensagem }}
      </v-card-text>

      <v-card-actions class="px-4 pb-4">
        <v-spacer />
        <v-btn
          variant="text"
          @click="cancelar"
          ref="botaoCancelar"
        >
          Cancelar
        </v-btn>
        <v-btn
          color="error"
          variant="flat"
          @click="confirmar"
        >
          Confirmar
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
const props = defineProps<{
  modelValue: boolean
  titulo: string
  mensagem: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  confirm: []
  cancel: []
}>()

const open = computed({
  get: () => props.modelValue,
  set: (v) => emit('update:modelValue', v)
})

const tituloId = useId()
const mensagemId = useId()
const botaoCancelar = ref<HTMLElement | null>(null)

watch(() => props.modelValue, async (novoValor) => {
  if (novoValor) {
    await nextTick()
    botaoCancelar.value?.focus()
  }
})

const confirmar = () => {
  emit('confirm')
  open.value = false
}

const cancelar = () => {
  emit('cancel')
  open.value = false
}
</script>
