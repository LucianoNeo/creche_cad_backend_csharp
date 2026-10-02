import axios from 'axios';
const api = axios.create({ baseURL: '/', withCredentials: true });
let csrfToken = null;
let reportError = () => {};
export function onApiError(handler) { reportError = handler; }
export async function refreshCsrf() {
  const response = await axios.get('/api/auth/csrf');
  csrfToken = response.data.token;
}
api.interceptors.request.use(async config => {
  if (!['get', 'head', 'options'].includes(config.method)) {
    if (!csrfToken) await refreshCsrf();
    config.headers['X-CSRF-TOKEN'] = csrfToken;
  }
  return config;
});
api.interceptors.response.use(response => response, error => {
  const data = error.response?.data;
  const validation = data?.errors ? Object.values(data.errors).flat().join(' ') : null;
  if (!error.config?.url?.includes('/api/auth/me')) reportError(validation || data?.message || 'Não foi possível concluir a operação. Tente novamente.');
  return Promise.reject(error);
});
export async function download(url, name) {
  const response = await api.get(url, { responseType: 'blob' });
  const objectUrl = URL.createObjectURL(response.data);
  const anchor = document.createElement('a'); anchor.href = objectUrl; anchor.download = name;
  anchor.click(); setTimeout(() => URL.revokeObjectURL(objectUrl), 1000);
}
export default api;
