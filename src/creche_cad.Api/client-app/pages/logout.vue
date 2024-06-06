<template>
  <v-container fluid fill-height>
    <v-layout align-center justify-center>
      <v-card>
        <v-card-title class="headline">Logout</v-card-title>
        <v-card-text>
          <p>Você está prestes a sair da sua conta. Tem certeza que deseja fazer logout?</p>
          <v-btn @click="handleLogout" color="red">Logout</v-btn>
        </v-card-text>
      </v-card>
    </v-layout>
  </v-container>
</template>

<script lang="ts">
import Vue from 'vue';
import { mapActions } from 'vuex';

export default Vue.extend({
  methods: {
    ...mapActions('auth', ['logout']),
     handleLogout() {
      try {
        this.logout();
        this.$router.push('/');
      } catch (error) {
        console.error('Erro ao fazer logout:', error);
      }
    }
  }
});
</script>

<style scoped>
/* Adicione estilos personalizados aqui */
</style>
