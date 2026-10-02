import api from '~/utils/api';
export const state = () => ({ alunos: [], loading: false });
export const mutations = { SET_ALUNOS(state, values) { state.alunos = values; }, SET_LOADING(state, value) { state.loading = value; } };
export const getters = { alunos: state => state.alunos, loading: state => state.loading };
export const actions = {
  async fetchAlunos({ commit }) {
    commit('SET_LOADING', true);
    try { const { data } = await api.get('/api/aluno'); commit('SET_ALUNOS', data); }
    finally { commit('SET_LOADING', false); }
  },
  async createAluno({ dispatch }, item) { await api.post('/api/aluno', item); await dispatch('fetchAlunos'); },
  async updateAluno({ dispatch }, item) { await api.put(`/api/aluno/${item.id}`, item); await dispatch('fetchAlunos'); },
  async deleteAluno({ dispatch }, item) { await api.delete(`/api/aluno/${item.id}`); await dispatch('fetchAlunos'); }
};
