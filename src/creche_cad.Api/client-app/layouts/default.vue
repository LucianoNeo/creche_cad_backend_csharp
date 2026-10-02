<template>
  <v-app>
    <v-navigation-drawer v-if="isLoggedIn" v-model="drawer" app width="248" class="sidebar">
      <div class="brand"><div class="brand-symbol">c.</div><div>CrecheCad<small>Gestão escolar</small></div></div>
      <div class="nav-caption">SECRETARIA</div>
      <v-list nav>
        <v-list-item v-for="item in items" :key="item.to" :to="item.to" exact color="primary">
          <v-list-item-icon><v-icon>{{ item.icon }}</v-icon></v-list-item-icon>
          <v-list-item-title>{{ item.title }}</v-list-item-title>
        </v-list-item>
      </v-list>
      <template #append><div class="sidebar-note">Cadastros e documentos<br><strong>em um só lugar.</strong></div></template>
    </v-navigation-drawer>
    <v-app-bar v-if="isLoggedIn" app flat color="white" height="76">
      <v-app-bar-nav-icon class="d-lg-none" @click="drawer = !drawer" aria-label="Abrir menu" />
      <span class="toolbar-label">Organização para o dia a dia</span><v-spacer />
      <span class="account"><span class="account-dot" />{{ $store.state.auth.user }}</span>
      <v-btn text small class="ml-4" to="/logout">Sair</v-btn>
    </v-app-bar>
    <v-main><div :class="isLoggedIn ? 'content-shell' : 'login-shell'"><Nuxt /></div></v-main>
    <v-snackbar :value="!!$store.state.error" color="error" timeout="8000" @input="!$event && $store.commit('SET_ERROR', null)">
      {{ $store.state.error }}<template #action><v-btn text @click="$store.commit('SET_ERROR', null)">Fechar</v-btn></template>
    </v-snackbar>
  </v-app>
</template>
<script>
export default {
  data: () => ({ drawer: null }),
  computed: {
    isLoggedIn() { return this.$store.state.auth.isAuthenticated; },
    items() { return [
      { to: '/welcome', title: 'Visão geral', icon: 'mdi-view-dashboard-outline' },
      { to: '/Alunos', title: 'Alunos', icon: 'mdi-account-group-outline' },
      { to: '/Turmas', title: 'Turmas', icon: 'mdi-home-group' },
      { to: '/Professores', title: 'Professores', icon: 'mdi-school-outline' },
      { to: '/Backup', title: 'Backup', icon: 'mdi-database-outline' }
    ]; }
  }
};
</script>
