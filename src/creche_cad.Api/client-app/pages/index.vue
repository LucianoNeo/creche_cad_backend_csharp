<template>
  <v-container fluid fill-height>
    <v-layout align-center justify-center>
      <v-card>
        <v-card-title class="headline">Login</v-card-title>
        <v-card-text>
          <v-form @submit.prevent="handleLogin">
            <v-text-field solo-filled v-model="username" label="Usuário" required></v-text-field>
            <v-text-field solo-filled v-model="password" label="Senha" type="password" required></v-text-field>
            <v-btn type="submit" color="primary">Entrar</v-btn>
          </v-form>
        </v-card-text>
      </v-card>
    </v-layout>
  </v-container>
</template>

<script>
import Vue from 'vue';
import { mapActions } from 'vuex';

export default Vue.extend({
  data() {
    return {
      username: '',
      password: ''
    };
  },
  methods: {
    ...mapActions('auth', ['login']),
    async handleLogin() {
      try {
        await this.login({ username: this.username, password: this.password });
        this.$router.push('/welcome');
      } catch (error) {
        alert('Usuário ou senha inválidos');
      }
    }
  }
});
</script>
