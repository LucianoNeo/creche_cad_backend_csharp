export default {
  ssr: false, target: 'static',
  head: { title: 'CrecheCad · Gestão escolar', htmlAttrs: { lang: 'pt-BR' },
    meta: [{ charset: 'utf-8' }, { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    { name: 'description', content: 'Cadastro de alunos, turmas, professores e documentos.' }] },
  css: ['~/assets/app.css'], components: true, plugins: ['~/plugins/api.js'],
  router: { middleware: ['auth'] },
  buildModules: ['@nuxtjs/vuetify'],
  vuetify: { defaultAssets: false, treeShake: true, theme: { dark: false, themes: { light: {
    primary: '#17655c', secondary: '#eab363', accent: '#17655c', error: '#bd4242', success: '#17655c'
  } } } },
  generate: { crawler: false, routes: [], fallback: '200.html' },
  build: { transpile: ['vuetify'] }
};
