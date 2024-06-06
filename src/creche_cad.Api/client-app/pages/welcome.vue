<template>
  <v-row class="mt-8">
    <v-col class="text-center">
      <v-icon size="230" color="blue">mdi-book-account</v-icon>
      <blockquote class="blockquote">
        &#8220;Seja bem vindo(a) ao CrecheCad, acesse as seções no menu à
        esquerda!&#8221;
      </blockquote>
      <div class="mt-10 text-left">
        <span>Status da API: </span>
        <v-icon :color="apiStatusColor">{{ apiStatusIcon }}</v-icon>
      </div>
      <div class="text-left">
        <span>Status do Banco de Dados: </span>
        <v-icon :color="dbStatusColor">{{ dbStatusIcon }}</v-icon>
      </div>
      <div class="text-left">
        <span>Caminho do Banco de Dados: </span>
        <span>{{ dbPath }}</span>
      </div>
    </v-col>
  </v-row>
</template>

<script>
export default {
  name: "WelcomePage",
  data() {
    return {
      apiStatus: null,
      dbStatus: null,
      dbPath: null,
    };
  },
  created() {
    if (!this.$store.getters["auth/isAuthenticated"]) {
      this.$router.push("/");
    }
    this.checkApiStatus();
    this.checkDatabaseStatus();
  },
  methods: {
    async checkApiStatus() {
      try {
        const response = await fetch("http://localhost:5210/api/turma");
        if (response.ok) {
          this.apiStatus = "OK";
        } else {
          this.apiStatus = "Falha";
        }
      } catch (error) {
        console.error("Erro ao verificar o status da API:", error);
        this.apiStatus = "Falha";
      }
    },
    async checkDatabaseStatus() {
      try {
        // Realize uma chamada para verificar se o banco de dados está configurado
        // Substitua "sua_url_da_api" pela URL real da sua API
        const response = await fetch(
          "http://localhost:5210/api/database/check-database"
        );
        if (response.ok) {
          const data = await response.json();
          this.dbStatus = "OK";
          this.dbPath = data.dbPath;
        } else {
          this.dbStatus = "Falha";
        }
      } catch (error) {
        console.error("Erro ao verificar o status do banco de dados:", error);
        this.dbStatus = "Falha";
      }
    },
  },
  computed: {
    apiStatusColor() {
      return this.apiStatus === "OK" ? "green" : "red";
    },
    apiStatusIcon() {
      return this.apiStatus === "OK" ? "mdi-check-circle" : "mdi-alert-circle";
    },
    dbStatusColor() {
      return this.dbStatus === "OK" ? "green" : "red";
    },
    dbStatusIcon() {
      return this.dbStatus === "OK" ? "mdi-check-circle" : "mdi-alert-circle";
    },
  },
};
</script>
