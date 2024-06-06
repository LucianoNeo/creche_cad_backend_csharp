<template>
  <v-app>
    <v-navigation-drawer
      v-model="drawer"
      :mini-variant="miniVariant"
      :clipped="clipped"
      fixed
      app
    >
      <v-list>
        <v-list-item
          v-for="(item, i) in items"
          :key="i"
          v-if="isLoggedIn || item.title === 'Login'"
          :to="item.to"
          router
          exact
        >
          <v-list-item-action>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>{{ item.title }}</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <v-app-bar :clipped-left="clipped" fixed app>
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-btn icon @click.stop="miniVariant = !miniVariant">
        <v-icon>mdi-{{ `chevron-${miniVariant ? "right" : "left"}` }}</v-icon>
      </v-btn>
      <v-toolbar-title>
        <v-icon size="36">mdi-book-account</v-icon>
        {{ title }}
      </v-toolbar-title>
    </v-app-bar>
    <v-main>
      <v-container v-if="isLoggedIn">
        <Nuxt />
      </v-container>
      <v-container v-else>
        <v-container>
          <router-view></router-view>
        </v-container>
      </v-container>
    </v-main>
    <v-footer :absolute="!fixed" app>
      <v-row no-gutters>
        <v-col cols="12" class="d-flex justify-space-between">
          <span
            >Desenvolvido por LucianoNeo &copy;
            {{ new Date().getFullYear() }}</span
          >
          <small class="ml-6">
            <em>
              Licenciado para Centro Vicentino Educação Infantil São Vicente de
              Paulo</em
            >
          </small>
        </v-col>
      </v-row>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  name: "DefaultLayout",
  data() {
    return {
      clipped: true,
      drawer: true,
      fixed: true,

      miniVariant: false,
      right: true,
      rightDrawer: false,
      title: "CrecheCad",
    };
  },
  computed: {
    isLoggedIn() {
      return this.$store.state.auth.isAuthenticated;
    },
    items() {
      const result = [
        {
          icon: "mdi-home",
          title: "Home",
          to: "/welcome",
        },
        {
          icon: "mdi-home-group",
          title: "Turmas",
          to: "/Turmas",
        },
        {
          icon: "mdi-account-group",
          title: "Alunos",
          to: "/Alunos",
        },
        {
          icon: "mdi-account-school",
          title: "Professores",
          to: "/Professores",
        },
        {
          icon: "mdi-database",
          title: "Backup",
          to: "/Backup",
        },
        {
          icon: "mdi-account-check",
          title: this.isLoggedIn ? "Logout" : "Login",
          to: this.isLoggedIn ? "/logout" : "/",
        },
      ];
      return result;
    },
  },
};
</script>
