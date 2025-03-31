describe("Recipes", () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/home"); // Ensure you visit after login
  });

  it("should load the recipes page", () => {
    cy.get('[data-testid="recipes-page"]').should('exist')
  });

  // add/remove filters
  // search by keyword
  // favorite/unfavorite recipe
  // check specified recipe
});
