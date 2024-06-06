import axios from 'axios';

const state = () => ({
  // Defina o estado inicial, se necessário
});

const mutations = {
  // Se houver mutações, defina-as aqui
};

const actions = {
  async cadastrarTurma({ commit }, { nome, metragem }) {
    try {
      // Realiza a requisição HTTP para o backend para cadastrar a turma
      const response = await axios.post('http://localhost:5210/api/turma', { nome, metragem });
      console.log(response.data)
      // Lógica para tratar a resposta, se necessário

      return response.data; // Retorna os dados, se necessário
    } catch (error) {
      throw error; // Lança o erro para ser tratado pelo componente Vue.js
    }
  },
};

const getters = {
  // Se houver getters, defina-os aqui
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters,
};
