const { test, expect } = require('@playwright/test');
const fs = require('node:fs');
const path = require('node:path');
const username = process.env.ADMIN_USERNAME || 'secretaria';
const password = process.env.ADMIN_PASSWORD || 'CrecheCad-Demo-2026!';
async function login(request) {
  const token = (await (await request.get('/api/auth/csrf')).json()).token;
  expect((await request.post('/api/auth/login', { data: { username, password }, headers: { 'X-CSRF-TOKEN': token } })).status()).toBe(200);
  return (await (await request.get('/api/auth/csrf')).json()).token;
}
test('API requires a session for school records, documents and backups', async ({ request }) => {
  for (const url of ['/api/aluno','/api/professor','/api/turma','/api/database/backup',`/api/documento/aluno/${crypto.randomUUID()}/documentos`]) expect((await request.get(url)).status()).toBe(401);
});
test('login checks credentials and CSRF, session ends on logout', async ({ request }) => {
  expect((await request.post('/api/auth/login', { data: { username, password } })).status()).toBe(400);
  const token = (await (await request.get('/api/auth/csrf')).json()).token;
  expect((await request.post('/api/auth/login', { data: { username, password: 'wrong-password' }, headers: { 'X-CSRF-TOKEN': token } })).status()).toBe(401);
  const csrf = await login(request);
  expect((await request.get('/api/auth/me')).status()).toBe(200);
  expect((await request.post('/api/auth/logout', { headers: { 'X-CSRF-TOKEN': csrf } })).status()).toBe(204);
  expect((await request.get('/api/auth/me')).status()).toBe(401);
});
test('student CRUD validates dates and class references, protects occupied classes', async ({ request }) => {
  const token = await login(request); const headers = { 'X-CSRF-TOKEN': token };
  const turma = await (await request.post('/api/turma', { data: { nome: 'Turma de verificação', metragem: '40 m²' }, headers })).json();
  const data = { nome: 'Aluno de verificação', turmaId: turma.id, dataNascimento: '2022-03-01', nomePai: 'A', nomeMae: 'B', endereco: 'Rua fictícia', telefone: '0000000000' };
  expect((await request.post('/api/aluno', { data: { ...data, turmaId: crypto.randomUUID() }, headers })).status()).toBe(400);
  expect((await request.post('/api/aluno', { data: { ...data, dataNascimento: '2099-01-01' }, headers })).status()).toBe(400);
  expect((await request.post('/api/aluno', { data })).status()).toBe(400);
  const created = await request.post('/api/aluno', { data, headers }); expect(created.status()).toBe(201);
  const aluno = await created.json(); expect(aluno.turmaNome).toBe('Turma de verificação');
  expect((await request.delete(`/api/turma/${turma.id}`, { headers })).status()).toBe(409);
  expect((await request.put(`/api/aluno/${aluno.id}`, { data: { ...data, nome: 'Aluno atualizado' }, headers })).status()).toBe(200);
  expect((await (await request.get(`/api/aluno/${aluno.id}`)).json()).nome).toBe('Aluno atualizado');
  expect((await request.delete(`/api/aluno/${aluno.id}`, { headers })).status()).toBe(204);
  expect((await request.get(`/api/aluno/${aluno.id}`)).status()).toBe(404);
  expect((await request.delete(`/api/turma/${turma.id}`, { headers })).status()).toBe(200);
});
test('document upload rejects invalid owners, formats and oversized files', async ({ request }) => {
  const token = await login(request); const headers = { 'X-CSRF-TOKEN': token };
  const alunos = await (await request.get('/api/aluno')).json(); const id = alunos[0].id;
  const upload = (owner, name, buffer) => request.post(`/api/documento/aluno/${owner}/upload`, { headers, multipart: { files: { name, mimeType: 'application/octet-stream', buffer } } });
  expect((await upload(crypto.randomUUID(), 'notas.txt', Buffer.from('dados fictícios'))).status()).toBe(404);
  expect((await upload(id, 'malware.exe', Buffer.from('executable'))).status()).toBe(400);
  expect((await upload(id, 'notas.pdf', Buffer.from('not a pdf'))).status()).toBe(400);
  expect((await upload(id, 'notas.txt', Buffer.alloc(5*1024*1024+1, 65))).status()).toBe(400);
  expect((await upload(id, '../declaracao-demo.txt', Buffer.from('Documento fictício para demonstração.'))).status()).toBe(200);
  const docs = await (await request.get(`/api/documento/aluno/${id}/documentos`)).json();
  expect(docs[0].nomeArquivo).toBe('declaracao-demo.txt');
  const zip = await request.get(`/api/documento/aluno/${id}/download`); expect(zip.status()).toBe(200); expect(zip.headers()['content-type']).toContain('application/zip');
});
test('teacher creation preserves separate mobile and secondary phone numbers', async ({ request }) => {
  const token = await login(request); const headers = { 'X-CSRF-TOKEN': token };
  const response = await request.post('/api/professor', { headers, data: { nome: 'Professor de verificação', rg: 'DEMO', cpf: '00000000000', endereco: 'Rua fictícia', telefonePrincipal: '000', telefoneCelular: '111', telefoneSecundario: '222', dataAdmissao: '2024-01-01' } });
  expect(response.status()).toBe(201); const professor = await response.json();
  expect(professor.telefoneCelular).toBe('111'); expect(professor.telefoneSecundario).toBe('222');
  expect((await request.delete(`/api/professor/${professor.id}`, { headers })).status()).toBe(200);
});
test('backup is a valid SQLite snapshot and does not expose server paths', async ({ request }) => {
  await login(request); const status = await (await request.get('/api/database/check-database')).json(); expect(status).not.toHaveProperty('dbPath');
  const response = await request.get('/api/database/backup'); expect(response.status()).toBe(200);
  const bytes = await response.body(); expect(bytes.subarray(0,16).toString()).toBe('SQLite format 3\0');
});
test('original Vue UI supports login, navigation, edits, documents and real screenshots', async ({ page }) => {
  const errors = []; page.on('pageerror', e => errors.push(e.message));
  await page.goto('/'); await page.getByLabel('Usuário', { exact: true }).fill(username); await page.getByLabel('Senha', { exact: true }).fill(password);
  await page.getByRole('button', { name: 'Entrar', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Bom ter tudo em dia.' })).toBeVisible();
  await expect(page.getByText('12', { exact: true }).first()).toBeVisible();
  await page.evaluate(() => document.fonts.ready);
  fs.mkdirSync('captures', { recursive: true });
  await page.screenshot({ path: 'captures/overview.png', fullPage: true });
  await page.getByRole('link', { name: 'Alunos', exact: true }).click();
  await expect(page.getByText('Alice Martins', { exact: true })).toBeVisible();
  await page.screenshot({ path: 'captures/students.png', fullPage: true });
  const row = page.getByRole('row').filter({ hasText: 'Alice Martins' });
  await row.locator('[title="Editar"]').click();
  const dialog = page.getByRole('dialog'); await dialog.getByLabel('Nome', { exact: true }).fill('Alice Martins');
  await dialog.getByRole('button', { name: 'Salvar', exact: true }).click(); await expect(page.getByText('Editar Aluno', { exact: true })).not.toBeVisible();
  await row.locator('[title="Abrir Documentos"]').click();
  await expect(page.getByText('declaracao-demo.txt', { exact: true })).toBeVisible();
  await page.screenshot({ path: 'captures/documents.png', fullPage: true });
  await page.getByRole('button', { name: 'Fechar', exact: true }).click();
  await page.getByRole('link', { name: 'Turmas', exact: true }).click(); await expect(page.getByText('Maternal · Girassol', { exact: true })).toBeVisible();
  await page.screenshot({ path: 'captures/classes.png', fullPage: true });
  await page.getByRole('link', { name: 'Professores', exact: true }).click(); await expect(page.getByText('Ana Ferreira', { exact: true })).toBeVisible();
  await page.screenshot({ path: 'captures/teachers.png', fullPage: true });
  await page.reload(); await expect(page.getByText('Ana Ferreira', { exact: true })).toBeVisible();
  await page.setViewportSize({ width: 390, height: 844 }); await page.goto('/welcome'); await expect(page.getByRole('heading', { name: 'Bom ter tudo em dia.' })).toBeVisible();
  await expect.poll(() => page.evaluate(() => document.documentElement.scrollWidth <= innerWidth)).toBe(true);
  await page.screenshot({ path: 'captures/mobile.png', fullPage: true });
  expect(errors).toEqual([]);
});
