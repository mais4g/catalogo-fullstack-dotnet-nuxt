type ToastColor = 'success' | 'error' | 'info' | 'warning'

const state = reactive({
  show: false,
  message: '',
  color: 'success' as ToastColor
})

export const useToast = () => {
  const exibir = (mensagem: string, cor: ToastColor) => {
    state.message = mensagem
    state.color = cor
    state.show = true
  }

  return {
    state,
    success: (mensagem: string) => exibir(mensagem, 'success'),
    error: (mensagem: string) => exibir(mensagem, 'error'),
    info: (mensagem: string) => exibir(mensagem, 'info'),
    warning: (mensagem: string) => exibir(mensagem, 'warning')
  }
}
