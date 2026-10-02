<script setup>
const route=useRoute();
const user=ref(null),ready=ref(false),error=ref(''),notice=ref(''),busy=ref(false),menu=ref(false),csrf=ref('');
const username=ref(''),password=ref(''),recover=ref(false);
const rows=ref([]),total=ref(0),page=ref(1),search=ref(''),classOptions=ref([]),dashboard=ref(null),auditRows=ref([]),auditTotal=ref(0),auditPage=ref(1),recoveries=ref([]);
const dialog=ref(null),mode=ref(''),form=ref({}),documents=ref([]),owner=ref(null),files=ref([]),newPassword=ref(''),currentPassword=ref('');
const path=computed(()=>route.path.replace(/\/$/,'')||'/');
const section=computed(()=>({'/Alunos':'aluno','/Turmas':'turma','/Professores':'professor','/Usuarios':'users'})[path.value]);
const title=computed(()=>({'aluno':'Alunos','turma':'Turmas','professor':'Professores','users':'Usuários'})[section.value]||'');
const canWrite=computed(()=>user.value?.role!=='Viewer');
const admin=computed(()=>user.value?.role==='Administrator');
const navigation=computed(()=>[{to:'/welcome',name:'Visão geral',icon:'◫'},{to:'/Alunos',name:'Alunos',icon:'♧'},{to:'/Turmas',name:'Turmas',icon:'⌂'},{to:'/Professores',name:'Professores',icon:'◇'},...(admin.value?[{to:'/Usuarios',name:'Usuários',icon:'◎'},{to:'/Auditoria',name:'Auditoria',icon:'≡'},{to:'/Backup',name:'Backup',icon:'▤'}]:[]),{to:'/Conta',name:'Minha conta',icon:'○'}]);
async function api(url,options={}) {
 const method=options.method||'GET';
 if(!['GET','HEAD'].includes(method)&&!csrf.value)csrf.value=(await (await fetch('/api/auth/csrf')).json()).token;
 const headers={...options.headers};if(!['GET','HEAD'].includes(method))headers['X-CSRF-TOKEN']=csrf.value;
 if(options.body&&!(options.body instanceof FormData)){headers['Content-Type']='application/json';options.body=JSON.stringify(options.body);}
 const response=await fetch(url,{...options,method,headers});
 if(!response.ok){
  let data={};try{data=await response.json();}catch{}
  const message=Object.values(data.errors||{}).flat().join(' ')||data.message||'Não foi possível concluir a operação.';
  if(response.status===401&&!url.includes('/auth/')){user.value=null;await navigateTo('/');}
  throw new Error(message);
 }
 return response.status===204?null:response.json();
}
async function execute(fn){busy.value=true;error.value='';try{await fn();}catch(e){error.value=e.message;}finally{busy.value=false;}}
async function login(){await execute(async()=>{user.value=await api('/api/auth/login',{method:'POST',body:{username:username.value,password:password.value}});csrf.value='';password.value='';await navigateTo('/welcome');});}
async function recovery(){await execute(async()=>{await api('/api/auth/recovery',{method:'POST',body:{username:username.value}});notice.value='Solicitação registrada. Fale com o administrador da secretaria para receber uma nova senha.';recover.value=false;});}
async function logout(){await execute(async()=>{await api('/api/auth/logout',{method:'POST'});user.value=null;csrf.value='';await navigateTo('/');});}
let loadSequence=0;
async function load(){
 const sequence=++loadSequence;
 if(!user.value)return;
 if(section.value){const data=await api(`/api/${section.value}?page=${page.value}&pageSize=10&q=${encodeURIComponent(search.value)}`);if(sequence===loadSequence){rows.value=Array.isArray(data)?data:data.items;total.value=Array.isArray(data)?data.length:data.total;}}
 if(path.value==='/welcome')dashboard.value=await api('/api/dashboard');
 if(section.value==='aluno')classOptions.value=(await api('/api/turma?pageSize=100')).items;
 if(section.value==='users')recoveries.value=await api('/api/users/recoveries');
 if(path.value==='/Auditoria'){const data=await api(`/api/users/audit?page=${auditPage.value}`);auditRows.value=data.items;auditTotal.value=data.total;}
}
watch(path,async()=>{page.value=1;search.value='';menu.value=false;if(ready.value)await execute(load);});
let searchTimer;watch(search,()=>{clearTimeout(searchTimer);searchTimer=setTimeout(()=>{page.value=1;execute(load);},250);});
onMounted(async()=>{try{user.value=await api('/api/auth/me');}catch{}ready.value=true;if(!user.value&&path.value!=='/')await navigateTo('/');else if(user.value&&path.value==='/')await navigateTo('/welcome');else await execute(load);});
const fields=computed(()=>{
 if(section.value==='turma')return[{key:'nome',label:'Nome',required:true},{key:'metragem',label:'Metragem'}];
 if(section.value==='users')return[{key:'username',label:'Usuário',required:true},{key:'password',label:'Senha inicial',type:'password',required:true},{key:'role',label:'Perfil',options:[{value:'Secretary',text:'Secretaria'},{value:'Viewer',text:'Consulta'},{value:'Administrator',text:'Administrador'}]}];
 if(section.value==='aluno')return[{key:'turmaId',label:'Turma',required:true,options:classOptions.value.map(t=>({value:t.id,text:t.nome}))},{key:'nome',label:'Nome',required:true},{key:'dataNascimento',label:'Data de Nascimento',type:'date',required:true},{key:'nomePai',label:'Nome do Pai',required:true},{key:'nomeMae',label:'Nome da Mãe',required:true},{key:'endereco',label:'Endereço',required:true},{key:'telefone',label:'Telefone',required:true}];
 return[{key:'nome',label:'Nome',required:true},{key:'rg',label:'RG',required:true},{key:'cpf',label:'CPF',required:true},{key:'endereco',label:'Endereço',required:true},{key:'telefonePrincipal',label:'Telefone principal',required:true},{key:'telefoneCelular',label:'Celular'},{key:'telefoneSecundario',label:'Telefone secundário'},{key:'titulo',label:'Título'},{key:'carteiraTrabalho',label:'Carteira de trabalho'},{key:'dataAdmissao',label:'Data de admissão',type:'date',required:true},{key:'dataDemissao',label:'Data de demissão',type:'date'}];
});
const singular=computed(()=>({'aluno':'Aluno','turma':'Turma','professor':'Professor','users':'Usuário'})[section.value]);
function openEditor(item=null){mode.value=item?'edit':'create';form.value=item?{...item}:{role:'Secretary'};for(const key of ['dataNascimento','dataAdmissao','dataDemissao'])if(form.value[key])form.value[key]=form.value[key].slice(0,10);dialog.value.showModal();}
async function save(){await execute(async()=>{const body={...form.value};if(body.dataDemissao==='')body.dataDemissao=null;await api(`/api/${section.value}${mode.value==='edit'?'/'+form.value.id:''}`,{method:mode.value==='edit'?'PUT':'POST',body});dialog.value.close();await load();notice.value='Cadastro salvo.';});}
function askDelete(item){mode.value='delete';form.value=item;dialog.value.showModal();}
async function remove(){await execute(async()=>{await api(`/api/${section.value}/${form.value.id}`,{method:'DELETE'});dialog.value.close();await load();notice.value='Cadastro excluído.';});}
async function openDocs(item){await execute(async()=>{owner.value=item;documents.value=await api(`/api/documento/${section.value}/${item.id}/documentos`);files.value=[];mode.value='documents';dialog.value.showModal();});}
async function upload(){await execute(async()=>{const body=new FormData();for(const file of files.value)body.append('files',file);await api(`/api/documento/${section.value}/${owner.value.id}/upload`,{method:'POST',body});documents.value=await api(`/api/documento/${section.value}/${owner.value.id}/documentos`);files.value=[];});}
async function deleteDocument(id){await execute(async()=>{await api(`/api/documento/${id}`,{method:'DELETE'});documents.value=await api(`/api/documento/${section.value}/${owner.value.id}/documentos`);});}
function resetUser(item){form.value={...item};newPassword.value='';mode.value='reset';dialog.value.showModal();}
async function resetPassword(){await execute(async()=>{await api(`/api/users/${form.value.id}/reset-password`,{method:'POST',body:{password:newPassword.value}});dialog.value.close();await load();notice.value='Senha redefinida. As sessões anteriores foram encerradas.';});}
async function toggleUser(item){await execute(async()=>{await api(`/api/users/${item.id}`,{method:'PUT',body:{role:item.role,active:!item.active}});await load();});}
async function changePassword(){await execute(async()=>{await api('/api/auth/change-password',{method:'POST',body:{currentPassword:currentPassword.value,newPassword:newPassword.value}});currentPassword.value='';newPassword.value='';user.value=null;csrf.value='';notice.value='Senha alterada. Entre novamente.';await navigateTo('/');});}
function date(value){return value?new Date(value.slice(0,10)+'T12:00:00').toLocaleDateString('pt-BR'):'—';}
async function changePage(delta){page.value+=delta;await execute(load);}
</script>

<template>
 <div v-if="ready" class="application">
  <template v-if="!user">
   <main class="login-shell"><div class="login-grid"><section class="login-intro"><div class="eyebrow">CRECHECAD / GESTÃO ESCOLAR</div><h1>Mais tempo para<br>cuidar de quem<br>está começando.</h1><p>Alunos, turmas, professores e documentos organizados para a rotina da secretaria.</p></section>
    <section class="login-card"><div class="brand-symbol">c.</div><h2>{{recover?'Recuperar acesso':'Bem-vindo de volta'}}</h2><p class="muted">{{recover?'Solicite uma nova senha à secretaria.':'Entre com a conta da secretaria.'}}</p>
     <form @submit.prevent="recover?recovery():login()"><label>Usuário<input v-model="username" autocomplete="username" required maxlength="100"></label><label v-if="!recover">Senha<input v-model="password" type="password" autocomplete="current-password" required></label><button class="button primary" :disabled="busy">{{recover?'Solicitar recuperação':'Entrar'}}</button></form><button class="text-button" @click="recover=!recover">{{recover?'Voltar ao login':'Esqueci minha senha'}}</button>
    </section></div></main>
  </template>
  <template v-else>
   <aside :class="['sidebar',{open:menu}]"><div class="brand"><div class="brand-symbol">c.</div><div>CrecheCad<small>Gestão escolar</small></div></div><div class="nav-caption">SECRETARIA</div><nav><NuxtLink v-for="item in navigation" :key="item.to" :to="item.to" :class="{active:path===item.to}"><span aria-hidden="true">{{item.icon}}</span>{{item.name}}</NuxtLink></nav><div class="sidebar-note">Cadastros e documentos<br><strong>em um só lugar.</strong></div></aside>
   <header><button class="menu-button" aria-label="Abrir menu" @click="menu=!menu">☰</button><span class="muted toolbar-label">Organização para o dia a dia</span><span class="account">● {{user.username}}</span><button class="text-button" @click="logout">Sair</button></header>
   <main class="content-shell">
    <section v-if="path==='/welcome'&&dashboard"><div class="page-header"><div><div class="eyebrow">SECRETARIA / VISÃO GERAL</div><h1>Bom ter tudo em dia.</h1><p>Acompanhe os cadastros e encontre o que precisa.</p></div><span class="date-pill">{{new Date().toLocaleDateString('pt-BR',{day:'numeric',month:'long',year:'numeric'})}}</span></div>
     <p v-if="dashboard.demo" class="demo-note">Ambiente de demonstração · todos os dados são fictícios.</p><div class="stats-grid"><NuxtLink v-for="card in [{label:'Alunos matriculados',count:dashboard.students,to:'/Alunos'},{label:'Turmas cadastradas',count:dashboard.classes,to:'/Turmas'},{label:'Professores',count:dashboard.teachers,to:'/Professores'}]" :key="card.label" :to="card.to" class="stat-card"><span>{{card.label}}</span><strong>{{card.count}}</strong><small>Ver cadastros →</small></NuxtLink></div>
     <div class="overview-grid"><section class="panel"><div class="panel-heading"><h2>Turmas da escola</h2><NuxtLink to="/Turmas">Ver todas</NuxtLink></div><div v-for="turma in dashboard.turmas" :key="turma.id" class="class-row"><span class="class-symbol">✿</span><div><strong>{{turma.nome}}</strong><small>{{turma.metragem||'Metragem não informada'}}</small></div><span class="class-count">{{turma.count}} alunos</span></div><p v-if="!dashboard.turmas.length" class="muted">Cadastre a primeira turma para começar.</p></section><section class="panel routine-panel"><div class="eyebrow">ROTINA ORGANIZADA</div><h2>Um cadastro completo.<br>Uma informação fácil<br>de encontrar.</h2><p>Consulte os responsáveis, atualize telefones e mantenha os documentos junto ao cadastro.</p><NuxtLink to="/Alunos" class="button primary">Consultar alunos →</NuxtLink></section></div>
    </section>
    <section v-else-if="section"><div class="page-header"><div><div class="eyebrow">SECRETARIA / CADASTROS</div><h1>{{title}}</h1><p>{{section==='users'?'Acesso da equipe, permissões e recuperação de senhas.':'Cadastros, contatos e documentos organizados.'}}</p></div></div>
     <p v-if="section==='users'&&recoveries.length" class="demo-note">{{recoveries.length}} solicitação(ões) de recuperação. Usuários: {{recoveries.map(r=>r.username).join(', ')}}</p>
     <div class="records-tools"><label class="search">Buscar {{title.toLowerCase()}}<input v-model="search" :aria-label="'Buscar '+title.toLowerCase()" placeholder="Nome do cadastro"></label><button v-if="canWrite" class="button primary" @click="openEditor()">{{section==='turma'?'Nova Turma':'Novo '+singular}}</button></div>
     <div class="table-wrap"><table><thead><tr><th>Nome</th><template v-if="section==='aluno'"><th>Nascimento</th><th>Telefone</th><th>Turma</th></template><template v-if="section==='professor'"><th>Telefone</th><th>Admissão</th><th>Formação</th></template><th v-if="section==='turma'">Metragem</th><template v-if="section==='users'"><th>Perfil</th><th>Situação</th></template><th>Ações</th></tr></thead><tbody><tr v-for="item in rows" :key="item.id"><td>{{item.nome||item.username}}</td><template v-if="section==='aluno'"><td>{{date(item.dataNascimento)}}</td><td>{{item.telefone}}</td><td>{{item.turmaNome}}</td></template><template v-if="section==='professor'"><td>{{item.telefonePrincipal}}</td><td>{{date(item.dataAdmissao)}}</td><td>{{item.titulo||'—'}}</td></template><td v-if="section==='turma'">{{item.metragem||'—'}}</td><template v-if="section==='users'"><td>{{item.role}}</td><td>{{item.active?'Ativo':'Desativado'}}</td></template><td class="actions"><template v-if="section==='users'"><button title="Redefinir senha" @click="resetUser(item)">Redefinir senha</button><button v-if="item.username!==user.username" @click="toggleUser(item)">{{item.active?'Desativar':'Ativar'}}</button></template><template v-else><button v-if="canWrite" title="Editar" @click="openEditor(item)">Editar</button><button v-if="canWrite" title="Excluir" class="danger" @click="askDelete(item)">Excluir</button><button v-if="['aluno','professor'].includes(section)" title="Abrir Documentos" @click="openDocs(item)">Documentos</button></template></td></tr><tr v-if="!rows.length"><td colspan="6" class="empty">Nenhum cadastro encontrado.</td></tr></tbody></table><div v-if="section!=='users'" class="pagination"><span>{{total}} registros · página {{page}}</span><button :disabled="page===1" @click="changePage(-1)">Anterior</button><button :disabled="page*10>=total" @click="changePage(1)">Próxima</button></div></div>
    </section>
    <section v-else-if="path==='/Backup'"><div class="page-header"><div><div class="eyebrow">SECRETARIA / BACKUP</div><h1>Uma cópia para guardar.</h1><p>Cadastros, documentos e usuários no mesmo arquivo.</p></div></div><section class="panel"><h2>Backup completo do SQLite</h2><p class="muted">O snapshot pode ser aberto com ferramentas SQLite e inclui todos os dados da instalação.</p><a class="button primary" href="/api/database/backup" download>Baixar backup</a></section></section>
    <section v-else-if="path==='/Conta'"><div class="page-header"><div><div class="eyebrow">SECRETARIA / MINHA CONTA</div><h1>Alterar senha</h1><p>Usuário: {{user.username}} · perfil: {{user.role}}</p></div></div><form class="panel account-form" @submit.prevent="changePassword"><label>Senha atual<input v-model="currentPassword" type="password" required autocomplete="current-password"></label><label>Nova senha<input v-model="newPassword" type="password" minlength="12" maxlength="128" required autocomplete="new-password"></label><button class="button primary" :disabled="busy">Alterar senha</button></form></section>
    <section v-else-if="path==='/Auditoria'"><div class="page-header"><div><div class="eyebrow">SECRETARIA / HISTÓRICO</div><h1>Auditoria</h1><p>Alterações registradas com autor, data e cadastro.</p></div></div><div class="table-wrap"><table><thead><tr><th>Data</th><th>Usuário</th><th>Operação</th><th>Cadastro</th></tr></thead><tbody><tr v-for="entry in auditRows" :key="entry.id"><td>{{new Date(entry.at.endsWith('Z')?entry.at:entry.at+'Z').toLocaleString('pt-BR')}}</td><td>{{entry.actor}}</td><td>{{entry.action}}</td><td>{{entry.resource}}</td></tr></tbody></table><div class="pagination"><span>{{auditTotal}} alterações</span><button :disabled="auditPage===1" @click="auditPage--;execute(load)">Anterior</button><button :disabled="auditPage*25>=auditTotal" @click="auditPage++;execute(load)">Próxima</button></div></div></section>
    <section v-else class="panel"><h1>Página não encontrada</h1><NuxtLink to="/welcome">Voltar à visão geral</NuxtLink></section>
   </main>
  </template>
  <dialog ref="dialog" @close="files=[]"><p v-if="error" role="alert" class="demo-note">{{error}}</p><template v-if="mode==='documents'"><h2>Documentos do {{singular}}</h2><p class="muted">{{owner?.nome}}</p><ul class="document-list"><li v-for="document in documents" :key="document.id"><a :href="'/api/documento/'+document.id+'/download'" download>{{document.nomeArquivo}}</a><button v-if="canWrite" class="danger" title="Excluir documento" @click="deleteDocument(document.id)">Excluir</button></li></ul><template v-if="canWrite"><label>Adicionar documentos<input type="file" multiple accept=".pdf,.png,.jpg,.jpeg,.txt" @change="files=Array.from($event.target.files)"></label><p class="muted small">PDF, PNG, JPG ou TXT · até 3 arquivos de 5 MB por envio.</p></template><div class="dialog-actions"><a v-if="documents.length" :href="'/api/documento/'+section+'/'+owner.id+'/download'" download class="button primary">Baixar Docs</a><button @click="dialog.close()">Fechar</button><button v-if="canWrite" :disabled="!files.length||busy" class="button primary" @click="upload">Enviar</button></div></template>
   <template v-else-if="mode==='delete'"><h2>Excluir {{form.nome}}?</h2><p class="muted">Essa ação remove o cadastro e seus documentos.</p><div class="dialog-actions"><button @click="dialog.close()">Cancelar</button><button class="button danger-button" :disabled="busy" @click="remove">Confirmar</button></div></template>
   <form v-else-if="mode==='reset'" @submit.prevent="resetPassword"><h2>Redefinir senha</h2><p>{{form.username}}</p><label>Nova senha<input v-model="newPassword" type="password" minlength="12" maxlength="128" required></label><div class="dialog-actions"><button type="button" @click="dialog.close()">Cancelar</button><button class="button primary" :disabled="busy">Redefinir</button></div></form>
   <form v-else @submit.prevent="save"><h2>{{mode==='edit'?'Editar':'Novo'}} {{singular}}</h2><div class="form-grid"><label v-for="field in fields" :key="field.key">{{field.label}}<select v-if="field.options" v-model="form[field.key]" :required="field.required"><option value="">Selecione</option><option v-for="option in field.options" :key="option.value" :value="option.value">{{option.text}}</option></select><input v-else v-model="form[field.key]" :type="field.type||'text'" :required="field.required" :maxlength="field.key==='password'?128:200" :minlength="field.key==='password'?12:undefined"></label></div><div class="dialog-actions"><button type="button" @click="dialog.close()">Cancelar</button><button class="button primary" :disabled="busy">Salvar</button></div></form>
  </dialog>
  <div v-if="error" role="alert" class="toast error">{{error}}<button @click="error=''">Fechar</button></div><div v-else-if="notice" role="status" class="toast">{{notice}}<button @click="notice=''">Fechar</button></div>
 </div>
</template>
