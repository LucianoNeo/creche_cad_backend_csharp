import axios from "axios";

const state = () => ({
  documentos: [],
  loading: false,
});

const mutations = {
  SET_DOCUMENTOS(state, documentos) {
    state.documentos = documentos;
  },
  SET_LOADING(state, loading) {
    state.loading = loading;
  },
};

const actions = {
  async fetchDocumentos({ commit }, alunoId) {
    commit("SET_LOADING", true);
    try {
      const response = await axios.get(
        `http://localhost:5210/api/documento/aluno/${alunoId}/documentos`
      );
      commit("SET_DOCUMENTOS", response.data);
    } catch (error) {
      console.error("Failed to fetch documentos:", error);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async deleteDocument({ commit, dispatch }, { alunoId, documentId }) {
    commit("SET_LOADING", true);
    try {
      await axios.delete(`http://localhost:5210/api/documento/${documentId}`);
      await dispatch("fetchDocumentos", alunoId); // Re-fetch documentos after deletion
    } catch (error) {
      console.error("Failed to delete document:", error);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async uploadDocument({ commit, dispatch }, { alunoId, files }) {
    commit("SET_LOADING", true);
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    try {
      await axios.post(
        `http://localhost:5210/api/documento/aluno/${alunoId}/upload`,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      await dispatch("fetchDocumentos", alunoId); // Re-fetch documentos after upload
    } catch (error) {
      console.error("Failed to upload documents:", error);
    } finally {
      commit("SET_LOADING", false);
    }
  },

  async downloadAllDocuments({ commit }, { alunoId, alunoNome }) {
    try {
      const response = await axios.get(
        `http://localhost:5210/api/documento/aluno/${alunoId}/download`,
        {
          responseType: "blob",
        }
      );

      let filename = `documentos${alunoNome}.zip`; // Nome padrão se não houver cabeçalho Content-Disposition
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
      console.error("Erro ao baixar todos os documentos do aluno:", error);
      return false;
    }
  },
};

const getters = {
  documentos: (state) => state.documentos,
  loading: (state) => state.loading,
};

export default {
  namespaced: true,
  state,
  mutations,
  getters,
  actions,
};
