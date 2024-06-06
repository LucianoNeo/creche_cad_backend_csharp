// store/professores.js

const state = () => ({
  professores: [],
  loading: false,
});

const mutations = {
  SET_PROFESSORES(state, professores) {
    state.professores = professores;
  },
  SET_LOADING(state, loading) {
    state.loading = loading;
  },
};

const actions = {
  async fetchProfessores({ commit }) {
    try {
      commit('SET_LOADING', true);
      const response = await fetch('http://localhost:5210/api/professor/');
      const professores = await response.json();
      commit('SET_PROFESSORES', professores);
    } catch (error) {
      console.error('Erro ao buscar professores:', error);
    } finally {
      commit('SET_LOADING', false);
    }
  },
  async createProfessor({ commit ,state}, professor) {
    try {
      const response = await fetch('http://localhost:5210/api/professor/', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(professor),
      });
      const newProfessor = await response.json();
      // Atualize a lista de professores após a criação
      commit('SET_PROFESSORES', [...state.professores, newProfessor]);
    } catch (error) {
      console.error('Erro ao criar professor:', error);
    }
  },
  async updateProfessor({ commit ,state}, professor) {
    try {
      // Implemente a lógica para atualizar um professor na API
      // Substitua este exemplo com sua lógica de atualização
      const response = await fetch(`http://localhost:5210/api/professor/${professor.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(professor),
      });
      const updatedProfessor = await response.json();
      // Atualize a lista de professores após a atualização
      const updatedProfessors = state.professores.map(a => (a.id === updatedProfessor.id ? updatedProfessor : a));
      commit('SET_PROFESSORES', updatedProfessors);
    } catch (error) {
      console.error('Erro ao atualizar professor:', error);
    }
  },
  async deleteProfessor({ commit,state }, professor) {
    try {
      // Implemente a lógica para excluir um professor na API
      // Substitua este exemplo com sua lógica de exclusão
      await fetch(`http://localhost:5210/api/professor/${professor.id}`, {
        method: 'DELETE',
      });
      // Atualize a lista de professores após a exclusão
      const updatedProfessors = state.professores.filter(a => a.id !== professor.id);
      commit('SET_PROFESSORES', updatedProfessors);
    } catch (error) {
      console.error('Erro ao excluir professor:', error);
    }
  },
};

const getters = {
  professores: state => state.professores,
  loading: state => state.loading,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};
