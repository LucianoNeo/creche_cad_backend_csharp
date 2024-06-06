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
        <v-btn color="primary" @click="openCreateDialog">Nova Turma</v-btn>
      </v-col>
    </v-row>

    <v-dialog v-model="dialogDelete" max-width="500px">
      <v-card>
        <v-card-title class="text-h5"
          >Tem certeza que quer excluir esta turma?</v-card-title
        >
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="primary" variant="text" @click="closeDelete"
            >Cancelar</v-btn
          >
          <v-btn color="error" variant="text" @click="deleteItemConfirm"
            >OK</v-btn
          >
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogEdit" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">Editar Turma</v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-text-field
              v-model="editedItem.nome"
              label="Nome"
              required
            ></v-text-field>
            <v-text-field
              v-model="editedItem.metragem"
              label="Metragem"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" variant="text" @click="closeEdit"
            >Cancelar</v-btn
          >
          <v-btn color="primary" variant="text" @click="saveItemConfirm"
            >Salvar</v-btn
          >
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="dialogCreate" max-width="500px">
      <v-card>
        <v-card-title class="text-h5">Nova Turma</v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-text-field
              v-model="newItem.nome"
              label="Nome"
              required
            ></v-text-field>
            <v-text-field
              v-model="newItem.metragem"
              label="Metragem"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="error" variant="text" @click="closeCreate"
            >Cancelar</v-btn
          >
          <v-btn color="primary" variant="text" @click="saveNewItemConfirm"
            >Salvar</v-btn
          >
          <v-spacer></v-spacer>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-row>
      <v-col>
        <v-data-table
          :headers="headers"
          :items="turmas"
          :search="search"
          :loading="loading"
          item-key="id"
          :footer-props="{
            'items-per-page-text': 'Linhas por página'
          }"
          no-data-text="Nenhum registro cadastrado"
        >
          <template v-slot:[`item.nome`]="{ item }">{{ item.nome }}</template>
          <template v-slot:[`item.metragem`]="{ item }">{{ item.metragem }}</template>
          <template v-slot:item.actions="{ item }">
            <v-icon class="me-2" size="medium" @click="editItem(item)">
              mdi-pencil
            </v-icon>
            <v-icon size="medium" color="red" @click="deleteItem(item)">
              mdi-delete
            </v-icon>
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
    ...mapGetters("turmas", ["turmas", "loading"]),
    headers() {
      return [
        { text: "Nome", align: "start", sortable: true, value: "nome" },
        { text: "Metragem", value: "metragem", sortable: true },
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
      itemToDelete: null,
      editedItem: {
        id: null,
        nome: "",
        metragem: "",
      },
      newItem: {
        nome: "",
        metragem: "",
      },
    };
  },
  methods: {
    ...mapActions("turmas", ["fetchTurmas", "deleteTurma", "updateTurma", "createTurma"]),

    editItem(item) {
      this.editedItem = { ...item };
      this.dialogEdit = true;
    },

    async saveItemConfirm() {
      await this.updateTurma(this.editedItem);
      this.fetchTurmas();
      this.closeEdit();
    },

    deleteItem(item) {
      this.itemToDelete = item;
      this.dialogDelete = true;
    },

    async deleteItemConfirm() {
      await this.deleteTurma(this.itemToDelete);
      this.fetchTurmas();
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
      this.newItem = {
        nome: "",
        metragem: "",
      };
    },

    async saveNewItemConfirm() {
      await this.createTurma(this.newItem);
      this.fetchTurmas();
      this.closeCreate();
    },
  },
  created() {
    this.fetchTurmas();
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


