# CrecheCad · Interface

Interface em Vue 2.7, Nuxt 2 e Vuetify 2. Usa a API na mesma origem, pelo proxy Nginx, e uma sessão autenticada no servidor. A navegação restaura a sessão ao recarregar a página; erros da API aparecem na notificação da tela.

Para executar a aplicação completa, use o Docker Compose e siga o [README principal](../../../README.md).

As telas de alunos, turmas e professores preservam os componentes e fluxos do projeto original. A revisão acrescenta uma visão geral, reorganiza a navegação e corrige o tratamento das datas nos formulários. O Nuxt 2 não tem mais suporte oficial; a migração do front está documentada como trabalho pendente.
