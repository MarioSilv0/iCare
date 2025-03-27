describe('Login page', () => {
  it('should load the login form', () => {
    cy.visit(`https://127.0.0.1:4200/#/login`);
    cy.get('[data-testid="login-container"]').should('exist')
  })
})

describe('User wants to login', () => {
  beforeEach(() => {
    cy.intercept('POST', `https://127.0.0.1:4200/api/account/login`, {
      statusCode: 200,
      body: {
        token: 'fake-login-token', 
        user: { "name": "User", "picture": null, "notifications": false }, 
      },
    }).as('loginRequest');
  })

  it('should allow a user to log in with email and password', () => {
      cy.visit(`https://127.0.0.1:4200/#/login`)
      cy.get('[data-testid="user-email"]').type("user@example.com")
      cy.get('[data-testid="user-password"]').type("UserPass123!")

      cy.get('[data-testid="bt-login"]').click();

      cy.wait('@loginRequest').its('response.statusCode').should('eq', 200);

      cy.url().should('not.include', '/login').should('include', '/home');

      cy.window().then((win) => {
        const token = win.localStorage.getItem('authToken');
        expect(token).to.not.be.null;
        expect(token).to.have.length.greaterThan(0); // Check if token is not empty
      });

  })
})

/*
describe('Google Login', () => {
  it('should allow user to login via Google', () => {
    cy.intercept('POST', `https://127.0.0.1:4200/#/api/account/google-login`, {
      statusCode: 200,
      body: {
        token: 'fake-google-token',
        user: { name: 'Google User', picture: null, notifications: false },
      },
    }).as('googleLoginRequest');

    cy.visit(`https://127.0.0.1:4200/#/login`); 

    cy.get('[data-testid="bt-google"]').click();

    cy.wait('@googleLoginRequest').its('response.statusCode').should('eq', 200);

    cy.url().should('include', '/home');

    cy.window().then((win) => {
      const token = win.localStorage.getItem('authToken');
      expect(token).to.not.be.null;
    });
  });
});
*/

describe('User wants to create account', () => {
  it('should show the register page to the user', () => {
    cy.visit(`https://127.0.0.1:4200/#/login`);
    cy.get('[data-testid="bt-register"]').should('exist')

    cy.get('[data-testid="bt-register"]').click()
    cy.url().should('include', '/register');
    cy.get('[data-testid="register-container"]').should('exist')
  })
})

describe('User wants to recover his password', () => {
  it('should show the recover page to the user', () => {
    cy.visit(`https://127.0.0.1:4200/#/login`);
    cy.get('[data-testid="bt-recover"]').should('exist')

    cy.get('[data-testid="bt-recover"]').click()
    cy.url().should('include', '/recover-password');
    cy.get('[data-testid="recover-container"]').should('exist')
  })
})
