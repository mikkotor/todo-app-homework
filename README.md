# A really simple Todo app running in a browser

## Features

* Can create, modify and delete multiple todo lists
* New todo items in lists can be inserted with Enter-key and deleted with Backspace- or Delete-key when description is empty
* Todo items can be checked as done or undone

## Technical details

### TodoApi

* Implemented in .NET 10, as an ASP.NET API
* Database of choice is [LiteDB](https://www.litedb.org/)
* The API also has Swagger endpoint running providing a simple OpenAPI spec frontend for quick interaction with the API and underlying DB
* Tests implemented using [xUnit](https://xunit.net/) and [Moq](https://github.com/Moq)

### todo-app

* Implemented in TypeScript and React
* Just a single component app, everything running in App-component
* Interface to TodoApi implemented as it's own TS-module
* Tests implemented using [Cypress](https://www.cypress.io/)
  * A single E2E test, TodoApi stubbed out using `intercept` to avoid the need to have the API running during testing

## Dev machine requirements

* .NET 10
* Node.js v22
* NPM v10

## How to run and test

### TodoApi

* `dotnet run` to run project, `dotnet test` to run tests
* Requires ports `3001` free on your machine
* Swagger portal runs in http://localhost:3001/swagger/index.html

### todo-app

* `npm run start` to launch the React app
* After server started, type `npm run cypress:e2e` to run automated tests (Or if you prefer, `npm run cypress:open` to run tests in different browsers using Cypress UI)
* Requires port `3000` to be free on your machine
