<h1 align="center"> Cálculo de CDB </h1>

# Visão geral

Este projeto implementa um sistema para cálculo de CDB (Certificado de Depósito Bancário), utilizando as tecnologias Angular, .NET e SQL Server em um ambiente conteinerizado.
O projeto possui testes unitários automatizados com o XUnit para verificar o comportamento das classes da camada de aplicação.

# Requisitos do Sistema

* [Node.js 20.11.0](https://nodejs.org/en/downloadangular)
* [Angular CLI 17](https://angular.io/cli)
* [.NET Runtime 8](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
* [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/vs/)
* [Docker](https://www.docker.com/products/docker-desktop/) instalado e configurado (a considerar o sistema operacional)

# Para executar o projeto:

- Clone o repositório:

	```
	git clone https://github.com/hernaneorlando/calculo_cdb.git
	```

- Acesse o diretório do projeto:

	```
	cd calculo_cdb
	```

- Construa as imagens Docker e inicice os containers:

	```
	docker compose up -d --build
	```

- Acesse a aplicação em seu navegador web:

	```
	http://localhost:8080
	```

Os testes unitários podem ser executados através do Visual Studio 2022, ou por outra IDE de sua escolha, como por exemplo o [Visual Studio Code](https://code.visualstudio.com/download), ou pelo terminal do seu sistema operacional através do commando:
```
dotnet test
```

# Arquitetura

O projeto segue uma arquitetura em camadas, com separação de responsabilidades entre:

Interface do Usuário (UI): Desenvolvida em Angular, utilizando Angular CLI, para fornecer uma experiência amigável ao usuário.
Camada de Aplicação: Implementada em .NET 8, responsável pelo processamento das regras de negócio e pela comunicação com o banco de dados.
Banco de Dados: Utiliza SQL Server para armazenar as informações dos CDBs e dos cálculos realizados (a implementar) com o auxílio do Entity Framework Core: ORM (Object-Relational Mapping) para mapeamento entre objetos e tabelas do banco de dados.

A utilização da biblioteca MediatR em uma solução .NET pode ser vantajosa, inclusive para microserviços, especialmente quando se busca aderir aos princípios SOLID e manter um código limpo. A MediatR ajuda a desacoplar componentes, o que é um aspecto central da arquitetura limpa, permitindo que o núcleo da aplicação permaneça independente de detalhes como bibliotecas e frameworks externos
É claro que no caso de sistemas críticos, como geralmente acontece em ambientes de microserviços, é crucial avaliar se a complexidade adicionada pela MediatR é justificada pelo tamanho e pela complexidade do seu microserviço. Se o microserviço for relativamente simples, talvez a MediatR não seja necessária. Por outro lado, para sistemas mais complexos, a MediatR pode trazer benefícios significativos ao facilitar a manutenção e a extensibilidade do código

# Atenção

Falta de testes de integração:

O projeto ainda não possui testes de integração automatizados para verificar a interação entre as diferentes camadas da aplicação. É recomendável a implementação de testes de integração utilizando ferramentas BDD (Behavior Driven Development), como Cucumber e/ou SpecFlow. Assim como também o sistema ainda não possui uma devida aplicação de padrões de _logging_, autenticação e autorização (a serem implementados no futuro).