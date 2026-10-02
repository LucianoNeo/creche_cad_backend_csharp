<template>
  <section class="login-grid">
    <div class="login-intro"><div class="eyebrow">CRECHECAD / GESTÃO ESCOLAR</div>
      <h1>Mais tempo para<br>cuidar de quem<br>está começando.</h1>
      <p>Alunos, turmas, professores e documentos organizados para a rotina da secretaria.</p>
      <div class="intro-decoration"><span>01</span> Pessoas primeiro. Cadastros em dia.</div>
    </div>
    <v-card class="login-card" flat>
      <div class="brand-symbol mb-8">c.</div><h2>Bem-vindo de volta</h2><p class="muted">Entre com a conta da secretaria.</p>
      <v-form @submit.prevent="submit">
        <v-text-field v-model="username" label="Usuário" outlined autocomplete="username" required />
        <v-text-field v-model="password" label="Senha" outlined type="password" autocomplete="current-password" required />
        <v-btn type="submit" color="primary" block large :loading="busy">Entrar</v-btn>
      </v-form><p class="login-footnote">Acesso restrito aos cadastros escolares.</p>
    </v-card>
  </section>
</template>
<script>
export default { data: () => ({ username: '', password: '', busy: false }), methods: {
  async submit() { this.busy = true; try { await this.$store.dispatch('auth/login', { username: this.username, password: this.password }); await this.$router.push('/welcome'); }
    catch (_) { /* API errors are displayed by the shared notification. */ } finally { this.busy = false; } }
} };
</script>
