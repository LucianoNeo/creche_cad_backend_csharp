<template>

  <v-container class="records-page"><div class="page-header"><div><div class="eyebrow">SECRETARIA / CADASTROS</div><h1>Professores</h1><p>Contatos e documentos da equipe pedagógica.</p></div></div>

    <v-row>

      <v-col>

        <v-text-field

          v-model="search"

          label="Buscar professores"

          outlined

          dense

          append-icon="mdi-magnify"

        ></v-text-field>

      </v-col>

      <v-col class="d-flex justify-end">

        <v-btn color="primary" @click="openCreateDialog">Novo Professor</v-btn>

      </v-col>

    </v-row>



    <v-dialog v-model="dialogDelete" max-width="500px">

      <v-card>

        <v-card-title class="text-h5">Confirmar Exclusão</v-card-title>

        <v-card-text>

          Tem certeza que deseja excluir este professor?

        </v-card-text>

        <v-card-actions>

          <v-spacer></v-spacer>

          <v-btn color="primary" text @click="closeDelete">Cancelar</v-btn>

          <v-btn color="error" text @click="deleteProfessorConfirm"

            >Confirmar</v-btn

          >

          <v-spacer></v-spacer>

        </v-card-actions>

      </v-card>

    </v-dialog>



    <v-dialog v-model="dialogEdit" max-width="800px">

      <v-card>

        <v-card-title class="text-h5">Editar Professor</v-card-title>

        <v-card-text>

          <v-form ref="form">

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.nome"

                  label="Nome"

                  required

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.rg"

                  label="RG"

                  required

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.cpf"

                  label="CPF"

                  required

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.endereco"

                  label="Endereço"

                  required

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.telefonePrincipal"

                  label="Telefone Principal"

                  required

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.telefoneCelular"

                  label="Telefone Celular"

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.telefoneSecundario"

                  label="Telefone Secundário"

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.titulo"

                  label="Título"

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.carteiraTrabalho"

                  label="Carteira de Trabalho"

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.dataAdmissao"

                  label="Data de Admissão"

                  type="date"

                  required

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="editedProfessor.dataDemissao"

                  label="Data de Demissão"

                  type="date"

                ></v-text-field>

              </v-col>

            </v-row>

          </v-form>

        </v-card-text>

        <v-card-actions>

          <v-spacer></v-spacer>

          <v-btn color="error" text @click="closeEdit">Cancelar</v-btn>

          <v-btn color="primary" text @click="saveEditedProfessorConfirm"

            >Salvar</v-btn

          >

          <v-spacer></v-spacer>

        </v-card-actions>

      </v-card>

    </v-dialog>



    <v-dialog v-model="dialogCreate" max-width="800px">

      <v-card>

        <v-card-title class="text-h5">Novo Professor</v-card-title>

        <v-card-text>

          <v-form ref="form">

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.nome"

                  label="Nome"

                  required

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.rg"

                  label="RG"

                  required

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.cpf"

                  label="CPF"

                  required

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.endereco"

                  label="Endereço"

                  required

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.telefonePrincipal"

                  label="Telefone Principal"

                  required

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.telefoneCelular"

                  label="Telefone Celular"

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.telefoneSecundario"

                  label="Telefone Secundário"

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.titulo"

                  label="Título"

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.carteiraTrabalho"

                  label="Carteira de Trabalho"

                ></v-text-field>

              </v-col>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.dataAdmissao"

                  label="Data de Admissão"

                  type="date"

                  required

                ></v-text-field>

              </v-col>

            </v-row>

            <v-row>

              <v-col cols="12" md="6">

                <v-text-field

                  v-model="newProfessor.dataDemissao"

                  label="Data de Demissão"

                  type="date"

                ></v-text-field>

              </v-col>

            </v-row>

          </v-form>

        </v-card-text>

        <v-card-actions>

          <v-spacer></v-spacer>

          <v-btn color="error" text @click="closeCreate">Cancelar</v-btn>

          <v-btn

            color="primary"

            :disabled="!validarProfessor()"

            text

            @click="saveNewProfessorConfirm"

            >Salvar</v-btn

          >

          <v-spacer></v-spacer>

        </v-card-actions>

      </v-card>

    </v-dialog>



    <v-dialog v-model="dialogDocuments" max-width="600px">

      <v-card>

        <v-card-title class="text-h5">Documentos do Professor</v-card-title>

        <v-card-text>

          <v-list>

            <v-list-item-group>

              <v-list-item

                v-for="document in professorDocuments"

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

            label="Adicionar documentos"

            prepend-icon="mdi-paperclip"

            multiple

            outlined

          ></v-file-input>

          <v-btn

            :disabled="professorDocuments.length < 1"

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

      <v-col

        v-for="professor in professoresFiltrados"

        :key="professor.id"

        cols="12"

        md="6"

        lg="4"

      >

        <v-card class="mb-4">

          <v-card-title class="justify-space-between"

            >{{ professor.nome }}

            <v-icon size="56"> mdi-account </v-icon>

          </v-card-title>

          <v-card-text>

            <div>

              RG: <strong>{{ professor.rg }}</strong>

            </div>

            <div>

              CPF: <strong>{{ professor.cpf }}</strong>

            </div>

            <div>

              Endereço: <strong>{{ professor.endereco }}</strong>

            </div>

            <div>

              Telefone Principal:

              <strong>{{ professor.telefonePrincipal }}</strong>

            </div>

            <div>

              Telefone Celular: <strong>{{ professor.telefoneCelular }}</strong>

            </div>

            <div>

              Telefone Secundário:

              <strong>{{ professor.telefoneSecundario }}</strong>

            </div>

            <div>

              Título: <strong>{{ professor.titulo }}</strong>

            </div>

            <div>

              Carteira de Trabalho:

              <strong>{{ professor.carteiraTrabalho }}</strong>

            </div>

            <div>

              Data de Admissão:

              <strong>{{ formatDate(professor.dataAdmissao) }}</strong>

            </div>

            <div>

              Data de Demissão:

              <strong>{{ formatDate(professor.dataDemissao) }}</strong>

            </div>

          </v-card-text>

          <v-card-actions class="justify-space-around">

            <v-tooltip top>

              <template v-slot:activator="{ on }">

                <v-icon

                  size="36"

                  @click="editItem(professor)"

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

                  size="36"

                  @click="deleteItem(professor)"

                  color="red"

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

                  size="36"

                  @click="openDocuments(professor)"

                  v-on="on"

                  title="Abrir Documentos"

                  >mdi-file-pdf-box</v-icon

                >

              </template>

              <span>Abrir Documentos</span>

            </v-tooltip>

          </v-card-actions>

        </v-card>

      </v-col>

    </v-row>

  </v-container>

</template>



<script>

import { mapActions, mapGetters } from "vuex";



export default {

  computed: {

    ...mapGetters("professores", ["professores", "loading"]),

    ...mapGetters("documentosProfessores", [

      "documentosProfessores",

      "loading",

    ]),

    headers() {

      return [

        { text: "Nome", align: "start", value: "nome" },

        { text: "RG", value: "rg" },

        { text: "CPF", value: "cpf" },

        { text: "Endereço", value: "endereco" },

        { text: "Telefone Principal", value: "telefonePrincipal" },

        { text: "Telefone Celular", value: "telefoneCelular" },

        { text: "Telefone Secundário", value: "telefoneSecundario" },

        { text: "Título", value: "titulo" },

        { text: "Carteira de Trabalho", value: "carteiraTrabalho" },

        { text: "Data de Admissão", value: "dataAdmissao" },

        { text: "Data de Demissão", value: "dataDemissao" },

        { text: "Ações", align: "end", value: "actions", sortable: false },

      ];

    },

    professoresFiltrados() {

      return this.professores.filter((professor) => {

        const searchLowerCase = this.search.toLowerCase();

        const nomeLowerCase = professor.nome.toLowerCase();

        const rgLowerCase = professor.rg.toLowerCase();

        const cpfLowerCase = professor.cpf.toLowerCase();



        return (

          nomeLowerCase.includes(searchLowerCase) ||

          rgLowerCase.includes(searchLowerCase) ||

          cpfLowerCase.includes(searchLowerCase)

        );

      });

    },

  },

  data() {

    return {

      newProfessor: {

        nome: "",

        rg: "",

        cpf: "",

        endereco: "",

        telefonePrincipal: "",

        telefoneCelular: "",

        telefoneSecundario: "",

        titulo: "",

        carteiraTrabalho: "",

        dataAdmissao: "",

        dataDemissao: null,

      },

      editedProfessor: {

        nome: "",

        rg: "",

        cpf: "",

        endereco: "",

        telefonePrincipal: "",

        telefoneCelular: "",

        telefoneSecundario: "",

        titulo: "",

        carteiraTrabalho: "",

        dataAdmissao: "",

        dataDemissao: null,

      },

      search: "",

      dialogDelete: false,

      dialogEdit: false,

      dialogCreate: false,

      dialogDocuments: false,

      itemToDelete: null,

      newDocumentFiles: [],

      professorDocuments: [],

    };

  },

  methods: {

    ...mapActions("professores", [

      "fetchProfessores",

      "deleteProfessor",

      "updateProfessor",

      "createProfessor",

    ]),

    ...mapActions("documentosProfessores", [

      "fetchDocumentosProfessores",

      "deleteDocument",

      "uploadDocument",

      "downloadAllDocuments",

    ]),



    editItem(item) {

      this.editedProfessor = { ...item, dataAdmissao: item.dataAdmissao?.slice(0,10), dataDemissao: item.dataDemissao?.slice(0,10) };

      this.dialogEdit = true;

    },



    async saveEditedProfessorConfirm() {

      try {

      await this.updateProfessor(this.editedProfessor);

      await this.fetchProfessores();

      this.closeEdit();

      } catch (_) { /* The API notification describes the failed operation. */ }

    },



    deleteItem(item) {

      this.itemToDelete = item;

      this.dialogDelete = true;

    },



    async deleteProfessorConfirm() {

      try {

      await this.deleteProfessor(this.itemToDelete);

      await this.fetchProfessores();

      this.closeDelete();

      } catch (_) { /* The API notification describes the failed operation. */ }

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

      this.newProfessor = {};

    },



    async saveNewProfessorConfirm() {

      try {

      if (this.validarProfessor()) {

        await this.createProfessor(this.newProfessor);

        await this.fetchProfessores();

        this.closeCreate();

      } else {

        this.$store.commit("SET_ERROR", "Preencha os campos obrigatórios para cadastrar o professor.");

      }

      } catch (_) { /* The API notification describes the failed operation. */ }

    },



    formatDate(date) {

      if (!date) return null;



      const formattedDate = new Date(date.slice(0, 10) + "T12:00:00");

      const day = formattedDate.getDate().toString().padStart(2, "0");

      const month = (formattedDate.getMonth() + 1).toString().padStart(2, "0");

      const year = formattedDate.getFullYear();



      return `${day}/${month}/${year}`;

    },



    updateDate() {

      const parts = this.editedProfessor.dataAdmissao.split("/");

      if (parts.length === 3) {

        this.editedProfessor.dataAdmissao = `${parts[2]}-${parts[1]}-${parts[0]}`;

      }

    },



    async openDocuments(item) {

      try {

      this.editedProfessor = { ...item, dataAdmissao: item.dataAdmissao?.slice(0,10), dataDemissao: item.dataDemissao?.slice(0,10) };

      await this.fetchDocumentosProfessores(item.id);

      this.professorDocuments = this.documentosProfessores;

      this.dialogDocuments = true;

      } catch (_) { /* The API notification describes the failed operation. */ }

    },



    closeDocuments() {

      this.dialogDocuments = false;

      this.professorDocuments = [];

      this.newDocumentFiles = [];

    },



    async uploadDocumentsHandler() {

      try {

      if (this.newDocumentFiles.length > 0) {

        await this.uploadDocument({

          professorId: this.editedProfessor.id,

          files: this.newDocumentFiles,

        });

        this.newDocumentFiles = [];

        await this.fetchDocumentosProfessores(this.editedProfessor.id);

        this.professorDocuments = this.documentosProfessores;

      }

      } catch (_) { /* The API notification describes the failed operation. */ }

    },



    async downloadAllDocumentsHandler() {
      try { await this.downloadAllDocuments({ professorId: this.editedProfessor.id }); }
      catch (_) { /* The shared notification displays the API error. */ }
    },

    async deleteDocumentHandler(document) {

      try {

      await this.deleteDocument({

        professorId: this.editedProfessor.id,

        documentId: document.id,

      });

      await this.fetchDocumentosProfessores(this.editedProfessor.id);

      this.professorDocuments = this.documentosProfessores;

      } catch (_) { /* The API notification describes the failed operation. */ }

    },



    validarProfessor() {

      const { nome, rg, cpf, endereco, telefonePrincipal, dataAdmissao } =

        this.newProfessor;

      return nome && rg && cpf && endereco && telefonePrincipal && dataAdmissao;

    },

  },

  async created() {
    try {

    await this.fetchProfessores();

    } catch (_) {}
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
