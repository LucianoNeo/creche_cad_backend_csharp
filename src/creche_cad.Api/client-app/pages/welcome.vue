<template>
  <section>
    <div class="page-header"><div><div class="eyebrow">SECRETARIA / VISÃO GERAL</div><h1>Bom ter tudo em dia.</h1><p>Acompanhe os cadastros e encontre o que precisa.</p></div><div class="date-pill">{{ today }}</div></div>
    <v-alert v-if="demo" text dense color="primary" icon="mdi-information-outline">Ambiente de demonstração · todos os dados são fictícios.</v-alert>
    <div class="stats-grid"><nuxt-link v-for="card in cards" :key="card.label" :to="card.to" class="stat-card"><div class="stat-label">{{ card.label }}<v-icon color="primary">{{ card.icon }}</v-icon></div><strong>{{ card.count }}</strong><span>Ver cadastros <v-icon small>mdi-arrow-right</v-icon></span></nuxt-link></div>
    <div class="overview-grid">
      <v-card flat class="panel"><div class="panel-heading"><h2>Turmas da escola</h2><nuxt-link to="/Turmas">Ver todas</nuxt-link></div>
        <div v-for="turma in turmas" :key="turma.id" class="class-row"><div class="class-symbol"><v-icon color="primary">mdi-flower-outline</v-icon></div><div><strong>{{ turma.nome }}</strong><small>{{ turma.metragem || 'Metragem não informada' }}</small></div><span class="class-count">{{ alunos.filter(a => a.turmaId === turma.id).length }} alunos</span></div>
        <p v-if="!turmas.length" class="muted">Cadastre a primeira turma para começar.</p>
      </v-card>
      <v-card flat class="panel routine-panel"><div class="eyebrow">ROTINA ORGANIZADA</div><h2>Um cadastro completo.<br>Uma informação fácil<br>de encontrar.</h2><p>Consulte os responsáveis, atualize telefones e mantenha os documentos junto ao cadastro.</p><v-btn to="/Alunos" color="primary" large>Consultar alunos <v-icon right small>mdi-arrow-right</v-icon></v-btn><div class="routine-mark">c.</div></v-card>
    </div>
  </section>
</template>
<script>
import api from '~/utils/api';
export default { data: () => ({ alunos: [], turmas: [], professores: [], demo: false }),
  computed: {
    today() { return new Date().toLocaleDateString('pt-BR', { day: 'numeric', month: 'long', year: 'numeric' }); },
    cards() { return [ { label: 'Alunos matriculados', count: this.alunos.length, to: '/Alunos', icon: 'mdi-account-group-outline' }, { label: 'Turmas cadastradas', count: this.turmas.length, to: '/Turmas', icon: 'mdi-home-group' }, { label: 'Professores', count: this.professores.length, to: '/Professores', icon: 'mdi-school-outline' } ]; }
  }, async mounted() { try { const [a,t,p,status] = await Promise.all(['/api/aluno','/api/turma','/api/professor','/api/database/check-database'].map(url => api.get(url))); this.alunos=a.data; this.turmas=t.data; this.professores=p.data; this.demo=status.data.demo; } catch (_) {} }
};
</script>
