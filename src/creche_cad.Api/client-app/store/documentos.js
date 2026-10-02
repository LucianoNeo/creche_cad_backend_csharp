import api, { download } from '~/utils/api';
export const state = () => ({ documentos: [], loading: false });
export const mutations = { LIST(state, value) { state.documentos = value; }, LOADING(state, value) { state.loading = value; } };
export const getters = { documentos: state => state.documentos, loading: state => state.loading };
export const actions = {
  async fetchDocumentos({ commit }, id) {
    commit('LOADING', true);
    try { const { data } = await api.get(`/api/documento/aluno/${id}/documentos`); commit('LIST', data); }
    finally { commit('LOADING', false); }
  },
  async deleteDocument({ dispatch }, { alunoId, documentId }) { await api.delete(`/api/documento/${documentId}`); await dispatch('fetchDocumentos', alunoId); },
  async uploadDocument({ dispatch }, { alunoId, files }) {
    const body = new FormData(); files.forEach(file => body.append('files', file));
    await api.post(`/api/documento/aluno/${alunoId}/upload`, body);
    await dispatch('fetchDocumentos', alunoId);
  },
  async downloadAllDocuments(context, { alunoId }) { await download(`/api/documento/aluno/${alunoId}/download`, 'documentos.zip'); return true; }
};
