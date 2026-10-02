import api from '~/utils/api';
export const state = () => ({ professores: [], loading: false });
export const mutations = { SET_PROFESSORES(state, values) { state.professores = values; }, SET_LOADING(state, value) { state.loading = value; } };
export const getters = { professores: state => state.professores, loading: state => state.loading };
export const actions = {
  async fetchProfessores({ commit }) {
    commit('SET_LOADING', true);
    try { const { data } = await api.get('/api/professor'); commit('SET_PROFESSORES', data); }
    finally { commit('SET_LOADING', false); }
  },
  async createProfessor({ dispatch }, item) { await api.post('/api/professor', item); await dispatch('fetchProfessores'); },
  async updateProfessor({ dispatch }, item) { await api.put(`/api/professor/${item.id}`, item); await dispatch('fetchProfessores'); },
  async deleteProfessor({ dispatch }, item) { await api.delete(`/api/professor/${item.id}`); await dispatch('fetchProfessores'); }
};
