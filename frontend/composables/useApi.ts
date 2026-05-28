export const useApi = () => {
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase as string

  return {
    baseURL,
    get: <T>(url: string) => $fetch<T>(url, { baseURL }),
    post: <T>(url: string, body: Record<string, unknown>) =>
      $fetch<T>(url, { baseURL, method: 'POST', body }),
    put: <T>(url: string, body: Record<string, unknown>) =>
      $fetch<T>(url, { baseURL, method: 'PUT', body }),
    delete: <T>(url: string) =>
      $fetch<T>(url, { baseURL, method: 'DELETE' })
  }
}
