describe('Register page', () => {
  beforeEach(() => {
    cy.visit(`https://127.0.0.1:4200/#/register`);
  });

  it('should load the register form', () => {
    cy.visit(`https://127.0.0.1:4200/#/register`);
    cy.get('[data-testid="register-container"]').should('exist')
  })

  it('should allow a user to register', () => {
    // Mock backend response
    cy.intercept('POST', 'https://127.0.0.1:4200/api/account/register', {
      statusCode: 200,
      body: {
        token: 'fake-registration-token',
        user: { "name": "User", "picture": null, "notifications": false },
      },
    }).as('registerRequest');

    // Fill out the registration form
    cy.get('[data-testid="user-email"]').type('newuser@example.com');
    cy.get('[data-testid="user-password"]').type('StrongPassword123!');
    cy.get('[data-testid="confirm-password"]').type('StrongPassword123!');

    // Click the Register button
    cy.get('[data-testid="bt-register"]').click();

    // Wait for the request to complete
    cy.wait('@registerRequest', {
      timeout: 20000
    })

    // Verify the user is redirected after registration
    cy.url().should('not.include', '/register').should('include', '/login');

    // Verify that the token is stored in localStorage
    cy.window().then((win) => {
      const token = win.localStorage.getItem('authToken');
      expect(token).to.not.be.null;
      expect(token).to.have.length.greaterThan(0);
    });
  });
})

describe('User wants to login', () => {
  it('should show the login page to the user', () => {
    cy.visit(`https://127.0.0.1:4200/#/register`);
    cy.get('[data-testid="bt-login"]').should('exist')

    cy.get('[data-testid="bt-login"]').click()
    cy.url().should('include', '/login');
    cy.get('[data-testid="login-container"]').should('exist')
  })
})
