import api from '~/utils/api';
export const state = () => ({ turmas: [], loading: false });
export const mutations = { SET_TURMAS(state, values) { state.turmas = values; }, SET_LOADING(state, value) { state.loading = value; } };
export const getters = { turmas: state => state.turmas, loading: state => state.loading };
export const actions = {
  async fetchTurmas({ commit }) {
    commit('SET_LOADING', true);
    try { const { data } = await api.get('/api/turma'); commit('SET_TURMAS', data); }
    finally { commit('SET_LOADING', false); }
  },
  async createTurma({ dispatch }, item) { await api.post('/api/turma', item); await dispatch('fetchTurmas'); },
  async updateTurma({ dispatch }, item) { await api.put(`/api/turma/${item.id}`, item); await dispatch('fetchTurmas'); },
  async deleteTurma({ dispatch }, item) { await api.delete(`/api/turma/${item.id}`); await dispatch('fetchTurmas'); }
};
