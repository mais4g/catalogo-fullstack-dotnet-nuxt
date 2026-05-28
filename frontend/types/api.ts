export interface Categoria {
  id: number
  nome: string
  descricao: string | null
}

export interface CategoriaInput {
  nome: string
  descricao: string | null
}

export interface Produto {
  id: number
  nome: string
  descricao: string | null
  preco: number
  categoriaId: number
  categoria: Categoria
}

export interface ProdutoInput {
  nome: string
  descricao: string | null
  preco: number
  categoriaId: number
}

export interface ApiError {
  mensagem?: string
  errors?: Record<string, string[]>
}
