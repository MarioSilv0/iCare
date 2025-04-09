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
        email: "newuser@example.com",
        password: 'StrongPassword123!'
      },
    }).as('registerRequest');

    // Fill out the registration form
    cy.get('[data-testid="user-email"]').type('newuser@example.com');
    cy.get('[data-testid="user-password"]').type('StrongPassword123!');
    cy.get('[data-testid="confirm-password"]').type('StrongPassword123!');
    cy.wait(500);

    // Click the Register button
    cy.get('[data-testid="bt-register"]').click({ force: true });

    // Wait for the request to complete
    cy.wait('@registerRequest')

    // Verify the user is redirected after registration
    cy.url().should('not.include', '/register').should('include', '/login');
  });
})
