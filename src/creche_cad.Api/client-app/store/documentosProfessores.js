import api, { download } from '~/utils/api';
export const state = () => ({ documentosProfessores: [], loading: false });
export const mutations = { LIST(state, value) { state.documentosProfessores = value; }, LOADING(state, value) { state.loading = value; } };
export const getters = { documentosProfessores: state => state.documentosProfessores, loading: state => state.loading };
export const actions = {
  async fetchDocumentosProfessores({ commit }, id) {
    commit('LOADING', true);
    try { const { data } = await api.get(`/api/documento/professor/${id}/documentos`); commit('LIST', data); }
    finally { commit('LOADING', false); }
  },
  async deleteDocument({ dispatch }, { professorId, documentId }) { await api.delete(`/api/documento/${documentId}`); await dispatch('fetchDocumentosProfessores', professorId); },
  async uploadDocument({ dispatch }, { professorId, files }) {
    const body = new FormData(); files.forEach(file => body.append('files', file));
    await api.post(`/api/documento/professor/${professorId}/upload`, body);
    await dispatch('fetchDocumentosProfessores', professorId);
  },
  async downloadAllDocuments(context, { professorId }) { await download(`/api/documento/professor/${professorId}/download`, 'documentos.zip'); return true; }
};
