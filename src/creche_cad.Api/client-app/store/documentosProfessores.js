import axios from "axios";

const state = () => ({
  documentosProfessores: [],
  loading: false,
});

const mutations = {
  SET_DOCUMENTOS(state, documentosProfessores) {
    state.documentosProfessores = documentosProfessores;
  },
  SET_LOADING(state, loading) {
    state.loading = loading;
  },
};

const actions = {
  async fetchDocumentosProfessores({ commit }, professorId) {
    commit("SET_LOADING", true);
    try {
      const response = await axios.get(
        `http://localhost:5210/api/documento/professor/${professorId}/documentos`
      );
      commit("SET_DOCUMENTOS", response.data);
    } catch (error) {
      console.error("Failed to fetch documentos:", error);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async deleteDocument({ commit, dispatch }, { professorId, documentId }) {
    commit("SET_LOADING", true);
    try {
      await axios.delete(`http://localhost:5210/api/documento/${documentId}`);
      await dispatch("fetchDocumentosProfessores", professorId); // Re-fetch documentosProfessores after deletion
    } catch (error) {
      console.error("Failed to delete document:", error);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async uploadDocument({ commit, dispatch }, { professorId, files }) {
    commit("SET_LOADING", true);
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    try {
      await axios.post(
        `http://localhost:5210/api/documento/professor/${professorId}/upload`,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      await dispatch("fetchDocumentosProfessores", professorId); // Re-fetch documentosProfessores after upload
    } catch (error) {
      console.error("Failed to upload documents:", error);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async downloadAllDocuments({ commit }, { professorId, professorNome }) {
    try {
      const response = await axios.get(
        `http://localhost:5210/api/documento/professor/${professorId}/download`,
        {
          responseType: "blob",
        }
      );

      let filename = `documentos${professorNome}.zip`; // Nome padrão se não houver cabeçalho Content-Disposition
      const contentDisposition = response.headers["content-disposition"];
      if (contentDisposition) {
        const match = contentDisposition.match(/filename\*?=['"]?([^"';]+)/);
        if (match) {
          filename = decodeURIComponent(match[1]);
        }
      }

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", filename);
      document.body.appendChild(link);
      link.click();
      window.URL.revokeObjectURL(url);

      return true;
    } catch (error) {
      console.error("Erro ao baixar todos os documentos do professor:", error);
      return false;
    }
  },
};

const getters = {
  documentosProfessores: (state) => state.documentosProfessores,
  loading: (state) => state.loading,
};

export default {
  namespaced: true,
  state,
  mutations,
  getters,
  actions,
};
