# FIAP.FCG 🚀  
**“Construindo a plataforma de Cloud Gaming da FIAP”**

[![.NET](https://img.shields.io/badge/Framework-.NET-8.svg?style=for-the-badge&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/) 

---

## 🌟 Visão Geral e Objetivo  
O projeto **FIAP.FCG** (FCG = *FIAP Cloud Games*) é uma iniciativa acadêmica desenvolvida no contexto da instituição Faculdade de Informática e Administração Paulista – FIAP, com o intuito de criar uma plataforma completa de jogos em nuvem. A solução visa proporcionar uma experiência prática de arquitetura moderna (.NET, containers, microsserviços, nuvem) aliada à lógica de domínio de uma empresa real‑de‑jogos.

Este repositório concentra a API principal e as camadas de aplicação, domínio e infraestrutura, além de testes automatizados, configurados para execução local via Docker.  
> **Estado do Projeto:** Em Desenvolvimento (versão inicial funcional focada em backend)  

---

## ✨ Funcionalidades Principais  
- Gerenciamento de usuários (cadastro, login, roles de admin/usuário)  
- Autenticação via JWT e autorização por função  
- Gerenciamento de catálogo de jogos (CRUD de jogos, categorias, promoções)  
- Integração com ambiente de dados via infraestrutura (ORM, migrations)  
- Exposição de API REST com documentação Swagger (ou similar)  
- Contêinerização da aplicação (Docker / Docker Compose) para fácil orquestração local  
- Estrutura de testes unitários e de integração (domínio e infraestrutura)  
- Preparação para deploy em ambiente cloud (ex: Azure, AWS)  

---

## 🛠️ Tecnologias Utilizadas  
- **Linguagem:** C#  
- **Framework:** .NET 8 (.NET Core)  
- **Arquitetura:** Camadas (API → Application → Domain → Infra)  
- **ORM / Acesso a dados:** Entity Framework Core  
- **Documentação de API:** Swagger (OpenAPI)  
- **Testes:** xUnit
- **Banco de dados:** SQL Server
- **Controle de versão:** GitHub  

---

## 🚀 Instalação e Execução Local  
### Pré‑requisitos  
- [.NET 8 SDK](https://dotnet.microsoft.com/download)  
- Git (para clonar o repositório)  
- Opcional: IDE como Visual Studio 2022/2023 ou Visual Studio Code  

### Passos de Instalação  

1. Clone o repositório
```bash
git clone https://github.com/thon18valentim/FIAP.FCG.git
cd FIAP.FCG
```

2. Configure variáveis de ambiente (veja seção ⚙️)

### Comandos de Execução  
```bash
# Atualizar banco de dados via EF Core (executar no diretório da API ou solução)
dotnet ef database update

# Executar API localmente via dotnet
dotnet run --project src/FCG.API/FCG.API.csproj
```

---

## ⚙️ Configuração de Variáveis de Ambiente  
Crie um arquivo `appsetings.json` no projeto `FIAP.FCG.WebApi` (ou edite o exemplo) com as seguintes variáveis:  
```js
// Exemplo de appsetings.json
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection="Server=localhost;Database=FCG_DB;User Id=sa;Password=YourStrong@Passw0rd;"
Jwt__Key="SUA_CHAVE_JWT_SUPER_SECRETA"
Jwt__Issuer="FCG_Issuer"
Jwt__Audience="FCG_Audience"
```
---

## 🧪 Testes  
Para executar os testes automatizados localmente, execute o comando na raiz da solução:  
```bash
dotnet test
```
Se houver pastas específicas de teste como `Tests.Domain` ou `Tests.Infra`, você também pode navegar até elas e executar individualmente.  

---

## 👤 Autores

<div style="display: flex; gap: 10px;">
  <a href="https://github.com/olszewskioc">
    <img src="https://github.com/olszewskioc.png" alt="Thiago Olszewski" style="border-radius: 50%; width: 60px; height: 60px; margin: 10%">
  </a>
  <a href="https://github.com/thon18valentim">
    <img src="https://github.com/thon18valentim.png" alt="Othon Valentim" style="border-radius: 50%; width: 60px; height: 60px; margin: 10%">
  </a>
  <a href="https://github.com/lug7n ">
    <img src="https://github.com/lug7n.png" alt="Luiz Fonseca" style="border-radius: 50%; width: 60px; height: 60px; margin: 10%">
  </a>
  <a href="https://github.com/2dsant">
    <img src="https://github.com/2dsant.png" alt="" style="border-radius: 50%; width: 60px; height: 60px; margin: 10%">
  </a>
  <a href="https://github.com/gilmarpedretti">
    <img src="https://github.com/gilmarpedretti.png" alt="Gilmar Pedretti" style="border-radius: 50%; width: 60px; height: 60px; margin: 10%">
  </a>
</div>

---

*Agradecemos o seu interesse e colaboração! Vamos construir juntos a próxima geração de Cloud Games.* 🎮  
