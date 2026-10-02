# CrecheCad

Cadastro de alunos, turmas, professores e documentos, com API em C# e interface em Vue/Nuxt. O projeto começou em 2024; esta revisão organiza a execução com Docker, substitui o login feito no navegador por autenticação no servidor e corrige problemas nos cadastros e no backup.

## Experimentar

É necessário ter Docker com Compose. Não precisa instalar .NET, Node ou SQLite.

```bash
git clone https://github.com/LucianoNeo/creche_cad_backend_csharp.git
cd creche_cad_backend_csharp
cp .env.example .env
docker compose up --build --detach --wait
```

No PowerShell, use `Copy-Item .env.example .env` no lugar de `cp`.

Abra **http://localhost:8082**. A demonstração usa o usuário `secretaria` e a senha `CrecheCad-Demo-2026!`. Os registros são fictícios e só são criados quando `DEMO_ENABLED=true` e o banco está vazio.

Para parar, execute `docker compose down`. Os cadastros e as chaves de sessão ficam no volume `school-data` e continuam disponíveis ao iniciar novamente. `docker compose down --volumes` apaga esse volume: use apenas quando quiser descartar a demonstração.

## O que avaliar

- Entre e consulte a visão geral da escola.
- Crie uma turma e cadastre um aluno nela. Uma turma com alunos não pode ser excluída sem transferi-los primeiro.
- Edite um cadastro e anexe um PDF, PNG, JPG ou TXT: até três arquivos por envio, com limite de 5 MB por arquivo.
- Consulte os documentos pelo cadastro e baixe um ZIP.
- Baixe um backup do SQLite pelo menu Backup.

## Decisões técnicas

| Parte | Implementação |
| --- | --- |
| API | ASP.NET Core / .NET 10, controllers e validação de entrada |
| Dados | EF Core, SQLite e migrations existentes |
| Interface | Vue 2.7, Nuxt 2 e Vuetify 2, preservando o front original |
| Sessão | Cookie HttpOnly, expiração de duas horas, CSRF nas operações de escrita e limitação de tentativas de login |
| Execução | API e Nginx em containers separados, mesma origem e volume persistente |
| Verificação | Playwright contra a aplicação completa no GitHub Actions |

O SQLite mantém a instalação simples para uma secretaria e permite levar o projeto para outro ambiente sem configurar um servidor de banco. Documentos pequenos ficam no banco para serem incluídos no mesmo backup. A listagem de alunos projeta os dados e o nome da turma em uma única consulta; o backup usa a API de snapshot do SQLite, sem copiar o arquivo enquanto ele pode estar sendo alterado.

`src/creche_cad.Api` é o ponto de entrada utilizado pelo Docker. O projeto de API na raiz é histórico e não participa dessa execução.

## Limites desta versão

Esta distribuição é uma demonstração para avaliação técnica. Tem uma conta administrativa configurada por ambiente; não implementa múltiplas escolas, perfis por funcionário, trilha de auditoria ou recuperação de senha. Arquivos têm limites e checagem básica de formato, sem serviço de antivírus. Listagens atendem um conjunto pequeno de cadastros; paginação no servidor e armazenamento externo de documentos são próximos passos para maior volume.

O front preserva [Nuxt 2, que encerrou o suporte oficial](https://nuxt.com/blog/nuxt2-eol). A migração para Vue/Nuxt atuais está pendente. Antes de usar dados reais, também são necessários HTTPS, gestão de usuários, proteção de backups e revisão das regras de acesso e retenção. Defina uma senha própria e desative `DEMO_ENABLED`. Fora do modo demo, o cookie de sessão exige HTTPS.

O banco que estava versionado foi retirado desta revisão e é excluído do contexto Docker. As capturas e verificações usam um banco novo com dados fictícios. Esta alteração não reescreve o histórico Git.

## Verificação remota

O [workflow](.github/workflows/crechecad-ci.yml) constrói os containers, executa os testes de API e navegação e gera capturas reais da interface como artefatos. Para repetir em seu ambiente de avaliação:

```bash
docker compose up --build --detach --wait
npm ci
npx playwright install chromium
npm test
```

Os testes usam a demonstração e modificam registros. Execute em uma instalação descartável.

---

## English

CrecheCad manages students, classes, teachers and their documents. It pairs a C#/.NET 10 API with the original Vue/Nuxt frontend. This revision adds server-side sessions, CSRF protection, input and upload validation, Docker Compose and remote end-to-end verification.

To review it, clone the repository, copy `.env.example` to `.env` and run `docker compose up --build --detach --wait`. Open **http://localhost:8082** and use `secretaria` / `CrecheCad-Demo-2026!`. Demo data is fictional and is only seeded into an empty database when explicitly enabled.

SQLite and session keys persist in the Docker volume. The backup endpoint uses SQLite's online backup API. The student list retrieves class names in a single projected query. Uploads support PDF, PNG, JPG and TXT, with up to three files per request and 5 MB per file.

This is a reviewable demo with one administrative account, not a multi-school service. Staff roles, audit history, password recovery, server-side pagination and malware scanning are not implemented. The original Nuxt 2 frontend is beyond its official support period; migration is pending. Real deployment also requires HTTPS, a private password, demo mode disabled, protected backups and a review of access and retention rules. The legacy database is excluded from this revision and all container images; Git history has not been rewritten.
