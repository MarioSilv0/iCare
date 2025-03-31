describe("Recipes", () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/home"); // Ensure you visit after login
  });

  it("should load the recipes page", () => {
    cy.get('[data-testid="recipes-page"]').should('exist')
  });

  it("should allow the user to add and remove filters", () => { })

  it("should disable filters that the user cannot apply", () => { })

  it("should allow the user to search recipes by given words", () => { })

  it("should allow the user to mark a given recipe as favorite or unfavorite a recipe", () => {
    // Select the first recipe article
    cy.get('[data-testid="recipe"]').first().as("firstRecipe");

    // Click the favorite button inside the first recipe
    cy.get("@firstRecipe").find('[data-testid="bt-toggle-favorite"]').click();

    // Check if the solid heart (favorited) is displayed
    cy.get("@firstRecipe")
      .find('[data-testid="bt-toggle-favorite"] img')
      .should("have.attr", "src", "../../assets/svgs/heart-solid.svg");

    // Click again to unfavorite
    cy.get("@firstRecipe").find('[data-testid="bt-toggle-favorite"]').click();

    // Check if the regular heart (unfavorited) is displayed
    cy.get("@firstRecipe")
      .find('[data-testid="bt-toggle-favorite"] img')
      .should("have.attr", "src", "../../assets/svgs/heart-regular.svg");
  })

  it("should allow the user to check the details of a given recipe", () => {

  })
});
