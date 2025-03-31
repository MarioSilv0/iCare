describe("Profile", () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/profile"); // Ensure you visit after login
  });

  it("should load the profile page", () => {
    cy.get('[data-testid="profile-page"]').should('exist')
  });

  it("showld allow the user to add a profile picture", () => {
    cy.get('[data-testid="profile-image"]').should('exist')

    cy.get('[data-testid="profile-image"]', { includeShadowDom: true })
      .get('[data-testid="image"]')
      .should('exist') 
      .attachFile("cat-with-glasses.jpg", { force: true })

    
    cy.get('[data-testid="profile-image"]', { includeShadowDom: true })
      .find('img')
      .should("have.attr", "src")
      .and("include", "cat-with-glasses.jpg");
   
  })

  it("showld allow the user to change his name", () => {
    cy.get("#name").clear().type("New Name");
    cy.get(".submit-btn").click();
    cy.get("#name").should("have.value", "New Name");
  })

  it("showld allow the user to change his email", () => {
    cy.get("#email").clear().type("newemail@example.com");
    cy.get(".submit-btn").click();
    cy.get("#email").should("have.value", "newemail@example.com");
  })

  it("showld allow the user to change his birthdate", () => {
    cy.get("#birthdate").clear().type("2000-01-01");
    cy.get(".submit-btn").click();
    cy.get("#birthdate").should("have.value", "2000-01-01");
  })

  it("showld allow the user to change his height", () => {
    cy.get("#height").clear().type("1.75");
    cy.get(".submit-btn").click();
    cy.get("#height").should("have.value", "1.75");
  })

  it("showld allow the user to change his weigth", () => {
    cy.get("#weight").clear().type("70");
    cy.get(".submit-btn").click();
    cy.get("#weight").should("have.value", "70");
  })

  it("showld allow the user to change his gender", () => {
    cy.get("#genero").select("Masculino");
    cy.get(".submit-btn").click();
    cy.get("#genero").should("have.value", "Masculino");
  })

  it("showld allow the user to change his activity level", () => {
    cy.get("#activityLevel").select("Levemente Ativo");
    cy.get(".submit-btn").click();
    cy.get("#activityLevel").should("have.value", "Levemente Ativo");
  })

  it("showld allow the user to turn on/off notifications", () => {
    cy.get("#notifications").click();
    cy.get(".submit-btn").click();
    cy.get("#notifications").should("be.checked");
  })

  it("showld allow the user to change his preferences", () => {
    cy.get("#preferencesList").type("Bom{enter}");
    cy.get(".submit-btn").click();
    cy.get("#preferencesList").should("contain", "Bom");
  })

  it("showld allow the user to change his restritions", () => {
    cy.get("#restrictionsList").type("Mau{enter}");
    cy.get(".submit-btn").click();
    cy.get("#restrictionsList").should("contain", "Mau");
  })

  it("showld take the user to the 'change password' page", () => {
    cy.get(".change-password").click();
    cy.url().should('include', '/change-password');
  })
})
