<template>
  <v-container>
    <v-row>
      <v-col>
        <v-text-field
          v-model="search"
          label="Buscar"
          outlined
          dense
          append-icon="mdi-magnify"
        ></v-text-field>
      </v-col>
      <v-col class="d-flex justify-end">
        <v-btn color="primary" @click="openCreateDialog">Novo Aluno</v-btn>
      </v-col>
    </v-row>

    <v-dialog v-model="dialogDelete" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">Confirmar Exclusão</v-card-title>
        <v-card-text> Tem certeza que deseja excluir este aluno? </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" text @click="closeDelete">Cancelar</v-btn>
          <v-btn color="error" text @click="deleteAlunoConfirm"
            >Confirmar</v-btn
          >
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogEdit" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">Editar Aluno</v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-select
              v-model="editedAluno.turmaId"
              :items="
                turmas.map((turma) => ({ text: turma.nome, value: turma.id }))
              "
              label="Turma"
              required
              @change="updateTurmaNome"
            ></v-select>
            <v-text-field
              v-model="editedAluno.nome"
              label="Nome"
              required
            ></v-text-field>
            <v-text-field
              v-model="editedAluno.dataNascimento"
              label="Data de Nascimento"
              required
              type="date"
              @input="updateDate"
            ></v-text-field>
            <v-text-field
              v-model="editedAluno.nomePai"
              label="Nome do Pai"
              required
            ></v-text-field>
            <v-text-field
              v-model="editedAluno.nomeMae"
              label="Nome da Mãe"
              required
            ></v-text-field>
            <v-text-field
              v-model="editedAluno.endereco"
              label="Endereço"
              required
            ></v-text-field>
            <v-text-field
              v-model="editedAluno.telefone"
              label="Telefone"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" text @click="closeEdit">Cancelar</v-btn>
          <v-btn color="primary" text @click="saveEditedAlunoConfirm"
            >Salvar</v-btn
          >
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogCreate" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">Novo Aluno</v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-select
              v-model="newAluno.turmaId"
              :items="
                turmas.map((turma) => ({ text: turma.nome, value: turma.id }))
              "
              label="Turma"
              required
            ></v-select>
            <v-text-field
              v-model="newAluno.nome"
              label="Nome"
              required
            ></v-text-field>
            <v-text-field
              v-model="newAluno.dataNascimento"
              label="Data de Nascimento"
              required
              type="date"
            ></v-text-field>
            <v-text-field
              v-model="newAluno.nomePai"
              label="Nome do Pai"
              required
            ></v-text-field>
            <v-text-field
              v-model="newAluno.nomeMae"
              label="Nome da Mãe"
              required
            ></v-text-field>
            <v-text-field
              v-model="newAluno.endereco"
              label="Endereço"
              required
            ></v-text-field>
            <v-text-field
              v-model="newAluno.telefone"
              label="Telefone"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" text @click="closeCreate">Cancelar</v-btn>
          <v-btn color="primary" text @click="saveNewAlunoConfirm"
            >Salvar</v-btn
          >
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogDocuments" max-width="600px">
      <v-card>
        <v-card-title class="text-h5">Documentos do Aluno</v-card-title>
        <v-card-text>
          <v-list>
            <v-list-item-group>
              <v-list-item
                v-for="document in alunoDocuments"
                :key="document.id"
              >
                <v-list-item-content>
                  <v-list-item-title>
                    <v-icon medium class="mr-2"> mdi-file </v-icon
                    >{{ document.nomeArquivo }}</v-list-item-title
                  >
                </v-list-item-content>
                <v-list-item-action>
                  <v-icon
                    small
                    color="red"
                    @click="deleteDocumentHandler(document)"
                  >
                    mdi-delete
                  </v-icon>
                </v-list-item-action>
              </v-list-item>
            </v-list-item-group>
          </v-list>
          <v-file-input
            v-model="newDocumentFiles"
            label="Adicionar Documentos"
            prepend-icon="mdi-paperclip"
            multiple
            outlined
          ></v-file-input>
          <v-btn
            :disabled="alunoDocuments.length < 1"
            color="primary"
            @click="downloadAllDocumentsHandler"
            >Baixar Docs</v-btn
          >
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" text @click="closeDocuments">Fechar</v-btn>
          <v-btn
            :disabled="newDocumentFiles.length < 1"
            color="primary"
            text
            @click="uploadDocumentsHandler"
            >Enviar</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-row>
      <v-col>
        <v-data-table
          :headers="headers"
          :items="alunos"
          :search="search"
          :loading="loading"
          item-key="id"
          :footer-props="{
            'items-per-page-text': 'Registros por página',
          }"
          no-data-text="Nenhum registro cadastrado"
        >
          <template v-slot:[`item.nome`]="{ item }">{{ item.nome }}</template>
          <template v-slot:[`item.dataNascimento`]="{ item }">{{
            formatDate(item.dataNascimento)
          }}</template>
          <template v-slot:item.actions="{ item }">
            <v-tooltip top>
              <template v-slot:activator="{ on }">
                <v-icon
                  class="me-2"
                  size="medium"
                  @click="editItem(item)"
                  v-on="on"
                  title="Editar"
                  >mdi-pencil</v-icon
                >
              </template>
              <span>Editar</span>
            </v-tooltip>

            <v-tooltip top>
              <template v-slot:activator="{ on }">
                <v-icon
                  size="medium"
                  color="red"
                  @click="deleteItem(item)"
                  v-on="on"
                  title="Excluir"
                  >mdi-delete</v-icon
                >
              </template>
              <span>Excluir</span>
            </v-tooltip>

            <v-tooltip top>
              <template v-slot:activator="{ on }">
                <v-icon
                  class="ml-2"
                  size="big"
                  @click="openDocuments(item)"
                  v-on="on"
                  title="Abrir Documentos"
                  >mdi-file-pdf-box</v-icon
                >
              </template>
              <span>Abrir Documentos</span>
            </v-tooltip>
          </template>
        </v-data-table>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import { mapActions, mapGetters } from "vuex";

export default {
  computed: {
    ...mapGetters("alunos", ["alunos", "loading"]),
    ...mapGetters("turmas", ["turmas"]),
    ...mapGetters("documentos", ["documentos", "loading"]),
    headers() {
      return [
        { text: "Nome", align: "start", sortable: true, value: "nome" },
        { text: "Data de Nascimento", value: "dataNascimento", sortable: true },
        { text: "Nome do Pai", value: "nomePai", sortable: true },
        { text: "Nome da Mãe", value: "nomeMae", sortable: true },
        { text: "Endereço", value: "endereco", sortable: true },
        { text: "Telefone", value: "telefone", sortable: true },
        { text: "Turma", align: "start", sortable: true, value: "turmaNome" },
        { text: "Ações", align: "end", value: "actions", sortable: false },
      ];
    },
  },
  data() {
    return {
      search: "",
      dialogDelete: false,
      dialogEdit: false,
      dialogCreate: false,
      dialogDocuments: false,
      itemToDelete: null,
      editedAluno: {},
      newAluno: {},
      newDocumentFiles: [],
      alunoDocuments: [],
    };
  },
  methods: {
    ...mapActions("alunos", [
      "fetchAlunos",
      "deleteAluno",
      "updateAluno",
      "createAluno",
    ]),
    ...mapActions("turmas", ["fetchTurmas"]),
    ...mapActions("documentos", [
      "fetchDocumentos",
      "deleteDocument",
      "uploadDocument",
      "downloadAllDocuments",
    ]),

    editItem(item) {
      this.editedAluno = { ...item };
      this.dialogEdit = true;
    },

    async saveEditedAlunoConfirm() {
      await this.updateAluno(this.editedAluno);
      await this.fetchAlunos();
      this.closeEdit();
    },

    deleteItem(item) {
      this.itemToDelete = item;
      this.dialogDelete = true;
    },

    async deleteAlunoConfirm() {
      await this.deleteAluno(this.itemToDelete);
      await this.fetchAlunos();
      this.closeDelete();
    },

    closeEdit() {
      this.dialogEdit = false;
    },

    closeDelete() {
      this.dialogDelete = false;
    },

    openCreateDialog() {
      this.dialogCreate = true;
    },

    closeCreate() {
      this.dialogCreate = false;
      this.newAluno = {};
    },

    async saveNewAlunoConfirm() {
      await this.createAluno(this.newAluno);
      this.fetchAlunos();
      this.closeCreate();
    },

    updateTurmaNome() {
      const turma = this.turmas.find(
        (turma) => turma.id === this.editedAluno.turmaId
      );
      this.editedAluno.turmaNome = turma ? turma.nome : null;
    },

    formatDate(date) {
      if (!date) return null;

      const formattedDate = new Date(date);
      const day = formattedDate.getDate().toString().padStart(2, "0");
      const month = (formattedDate.getMonth() + 1).toString().padStart(2, "0");
      const year = formattedDate.getFullYear();

      return `${day}/${month}/${year}`;
    },

    updateDate() {
      const parts = this.editedAluno.dataNascimento.split("/");
      if (parts.length === 3) {
        this.editedAluno.dataNascimento = `${parts[2]}-${parts[1]}-${parts[0]}`;
      }
    },

    async openDocuments(item) {
      this.editedAluno = { ...item };
      await this.fetchDocumentos(item.id);
      this.alunoDocuments = this.documentos;
      this.dialogDocuments = true;
    },

    closeDocuments() {
      this.dialogDocuments = false;
      this.alunoDocuments = [];
      this.newDocumentFiles = [];
    },

    async uploadDocumentsHandler() {
      if (this.newDocumentFiles.length > 0) {
        await this.uploadDocument({
          alunoId: this.editedAluno.id,
          files: this.newDocumentFiles,
        });
        this.newDocumentFiles = [];
        await this.fetchDocumentos(this.editedAluno.id);
        this.alunoDocuments = this.documentos;
      }
    },

    async downloadAllDocumentsHandler() {
      try {
        const success = await this.downloadAllDocuments({
          alunoId: this.editedAluno.id,
          alunoNome: this.editedAluno.nome,
        });
        console.log(this.editedAluno.nome);
        if (success) {
          console.log("Download de todos os documentos concluído com sucesso!");
        } else {
          console.error("Falha ao baixar todos os documentos do aluno.");
        }
      } catch (error) {
        console.error(
          "Erro ao iniciar o download de todos os documentos do aluno:",
          error
        );
      }
    },

    async deleteDocumentHandler(document) {
      await this.deleteDocument({
        alunoId: this.editedAluno.id,
        documentId: document.id,
      });
      await this.fetchDocumentos(this.editedAluno.id);
      this.alunoDocuments = this.documentos;
    },
  },
  created() {
    this.fetchTurmas();
    this.fetchAlunos();
  },
  watch: {
    dialogEdit(val) {
      val || this.closeEdit();
    },
    dialogDelete(val) {
      val || this.closeDelete();
    },
    dialogCreate(val) {
      val || this.closeCreate();
    },
  },
};
</script>
