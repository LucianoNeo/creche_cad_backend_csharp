import api, { refreshCsrf } from '~/utils/api';
export const state = () => ({ isAuthenticated: false, user: null });
export const mutations = { SESSION(state, user) { state.user = user; state.isAuthenticated = !!user; } };
export const getters = { isAuthenticated: state => state.isAuthenticated, currentUser: state => state.user };
export const actions = {
  async restore({ commit }) { try { const { data } = await api.get('/api/auth/me'); commit('SESSION', data.username); } catch (e) { if (e.response?.status !== 401) throw e; commit('SESSION', null); } },
  async login({ commit }, credentials) { const { data } = await api.post('/api/auth/login', credentials); commit('SESSION', data.username); await refreshCsrf(); },
  async logout({ commit }) { await api.post('/api/auth/logout'); commit('SESSION', null); await refreshCsrf(); }
};
