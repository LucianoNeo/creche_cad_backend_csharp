export default defineNuxtConfig({ssr:false,devtools:{enabled:false},css:['~/assets/app.css'],
 app:{head:{title:'CrecheCad · Gestão escolar',htmlAttrs:{lang:'pt-BR'},meta:[{name:'viewport',content:'width=device-width, initial-scale=1'}]}},
 nitro:{prerender:{routes:['/'],crawlLinks:false}},compatibilityDate:'2026-10-02'});
