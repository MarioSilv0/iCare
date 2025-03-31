/// <reference types="cypress" />
// ***********************************************
// This example commands.ts shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
//
import 'cypress-file-upload';

declare global {
   namespace Cypress {
     interface Chainable {
       login(): Chainable<void>;
     }
   }
}


Cypress.Commands.add("login", () => {
  return cy.request({
    method: "POST",
    url: "https://127.0.0.1:4200/api/account/login",
    body: {
      email: "user@example.com",
      password: "UserPass123!"
    },
  }).then((r) => {
    window.localStorage.setItem("authToken", r.body.token);
    window.localStorage.setItem("user", JSON.stringify(r.body.user));
  });
});

