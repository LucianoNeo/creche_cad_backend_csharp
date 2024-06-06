// store/auth.js

const state = () => ({
  isAuthenticated: false,
  user: null
});

const mutations = {
  SET_AUTHENTICATED(state, isAuthenticated) {
    state.isAuthenticated = isAuthenticated;
  },
  SET_USER(state, user) {
    state.user = user;
  }
};

const actions = {
  async login({ commit }, { username, password }) {
    try {
      if (username === 'creche' && password === 'svp2024') {
        commit('SET_AUTHENTICATED', true);
        commit('SET_USER', username);
      } else {
        throw new Error('Usuário ou senha inválidos');
      }
    } catch (error) {
      throw error;
    }
  },
  logout({ commit }) {
    commit('SET_AUTHENTICATED', false);
    commit('SET_USER', null);
  }
};

const getters = {
  isAuthenticated: state => state.isAuthenticated,
  currentUser: state => state.user
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters
};
