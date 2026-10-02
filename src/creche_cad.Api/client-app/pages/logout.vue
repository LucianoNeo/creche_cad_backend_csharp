<template><v-card flat class="panel"><h1>Sair da conta?</h1><p class="muted">Será necessário entrar novamente para acessar os cadastros.</p><v-btn color="primary" @click="leave">Sair</v-btn><v-btn text to="/welcome">Continuar aqui</v-btn></v-card></template>
<script>export default { methods: { async leave() { await this.$store.dispatch('auth/logout'); this.$router.push('/'); } } };</script>
