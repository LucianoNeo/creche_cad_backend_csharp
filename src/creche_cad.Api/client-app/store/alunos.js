// store/alunos.js

const state = () => ({
  alunos: [],
  loading: false,
});

const mutations = {
  SET_ALUNOS(state, alunos) {
    state.alunos = alunos;
  },
  SET_LOADING(state, loading) {
    state.loading = loading;
  },
};

const actions = {
  async fetchAlunos({ commit }) {
    try {
      commit('SET_LOADING', true);
      const response = await fetch('http://localhost:5210/api/aluno/');
      const alunos = await response.json();
      commit('SET_ALUNOS', alunos);
    } catch (error) {
      console.error('Erro ao buscar alunos:', error);
    } finally {
      commit('SET_LOADING', false);
    }
  },
  async createAluno({ commit ,state}, aluno) {
    try {
      // Implemente a lógica para criar um novo aluno na API
      // Substitua este exemplo com sua lógica de criação
      const response = await fetch('http://localhost:5210/api/aluno/', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(aluno),
      });
      const newAluno = await response.json();
      // Atualize a lista de alunos após a criação
      commit('SET_ALUNOS', [...state.alunos, newAluno]);
    } catch (error) {
      console.error('Erro ao criar aluno:', error);
    }
  },
  async updateAluno({ commit ,state}, aluno) {
    try {
      // Implemente a lógica para atualizar um aluno na API
      // Substitua este exemplo com sua lógica de atualização
      const response = await fetch(`http://localhost:5210/api/aluno/${aluno.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(aluno),
      });
      const updatedAluno = await response.json();
      // Atualize a lista de alunos após a atualização
      const updatedAlunos = state.alunos.map(a => (a.id === updatedAluno.id ? updatedAluno : a));
      commit('SET_ALUNOS', updatedAlunos);
    } catch (error) {
      console.error('Erro ao atualizar aluno:', error);
    }
  },
  async deleteAluno({ commit,state }, aluno) {
    try {
      // Implemente a lógica para excluir um aluno na API
      // Substitua este exemplo com sua lógica de exclusão
      await fetch(`http://localhost:5210/api/aluno/${aluno.id}`, {
        method: 'DELETE',
      });
      // Atualize a lista de alunos após a exclusão
      const updatedAlunos = state.alunos.filter(a => a.id !== aluno.id);
      commit('SET_ALUNOS', updatedAlunos);
    } catch (error) {
      console.error('Erro ao excluir aluno:', error);
    }
  },
};

const getters = {
  alunos: state => state.alunos,
  loading: state => state.loading,
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};
