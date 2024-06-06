// store/turmas.js

const state = () => ({
  turmas: [],
  loading: false,
});

const mutations = {
  SET_TURMAS(state, turmas) {
    state.turmas = turmas;
  },
  SET_LOADING(state, loading) {
    state.loading = loading;
  },
};

const actions = {
  async fetchTurmas({ commit }) {
    try {
      commit('SET_LOADING', true);
      // Fazer a chamada para a API para buscar as turmas
      const response = await fetch('http://localhost:5210/api/turma');
      const turmas = await response.json();
      commit('SET_TURMAS', turmas);
    } catch (error) {
      console.error('Erro ao buscar turmas:', error);
    } finally {
      commit('SET_LOADING', false);
    }
  },
  async deleteTurma({ commit, state }, turma) {
    try {
      // Fazer a chamada para a API para excluir a turma
      await fetch(`http://localhost:5210/api/turma/${turma.id}`, {
        method: 'DELETE',
      });
      // Atualizar a lista de turmas após a exclusão
      commit('SET_TURMAS', state.turmas.filter(t => t.id !== turma.id));
    } catch (error) {
      console.error('Erro ao excluir turma:', error);
    }
  },
  async updateTurma({ commit, state }, turma) {
    try {
      // Fazer a chamada para a API para atualizar a turma
      await fetch(`http://localhost:5210/api/turma/${turma.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(turma),
      });
      // Atualizar a lista de turmas após a edição
      const index = state.turmas.findIndex(t => t.id === turma.id);
      if (index !== -1) {
        const updatedTurmas = [...state.turmas];
        updatedTurmas.splice(index, 1, turma);
        commit('SET_TURMAS', updatedTurmas);
      }
    } catch (error) {
      console.error('Erro ao atualizar turma:', error);
    }
  },
  async createTurma({ commit, state }, turma) {
    try {
      // Fazer a chamada para a API para criar a nova turma
      const response = await fetch('http://localhost:5210/api/turma', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(turma),
      });
      const newTurma = await response.json();
      // Atualizar a lista de turmas após a criação
      commit('SET_TURMAS', [...state.turmas, newTurma]);
    } catch (error) {
      console.error('Erro ao criar turma:', error);
    }
  },
};

const getters = {
  turmas: state => state.turmas,
  loading: state => state.loading,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};
