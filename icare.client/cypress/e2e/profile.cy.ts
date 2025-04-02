describe("Profile", () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/profile"); // Ensure you visit after login
  });

  it("should load the profile page", () => {
    cy.get('[data-testid="profile-page"]').should('exist')
  });

  //it("should allow the user to add a profile picture", () => {
  //  cy.get('[data-testid="profile-image"]').should('exist')

  //  cy.get('[data-testid="profile-image"]', { includeShadowDom: true })
  //    .get('[data-testid="image"]')
  //    .should('exist') 
  //    .attachFile("cat-with-glasses.jpg", { force: true })

    
  //  cy.get('[data-testid="profile-image"]', { includeShadowDom: true })
  //    .find('img')
  //    .should("have.attr", "src")
  //    .and("include", "cat-with-glasses.jpg");
   
  //})

  it("should allow the user to change his name", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='name-input'] input")
      .clear()
      .type("New Name");

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  });

  it("should allow the user to change his email", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='email-input'] input")
      .clear()
      .type("john@doe.com");

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to change his birthdate", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='birthday-input'] input")
      .clear()
      .type("2001-04-23");

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to change his height", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='height-input'] input")
      .clear()
      .type("1.53");

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to change his weigth", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='weight-input'] input")
      .clear()
      .type("80");

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to change his gender", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='gender-input']")
      .select("Feminino")

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to change his activity level", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='activity-input']")
      .select("Sedentário")
      
    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to turn on/off notifications", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='notifications-input'] input")
      .should("not.be.checked")
      .click()
      .should("be.checked")

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to change his preferences", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='preferences-input'] datalist")
      .find("option")
      .first()
      .click({ force: true })

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should allow the user to change his restritions", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='restrictions-input'] datalist")
      .find("option")
      .first()
      .click({ force: true })

    cy.get("[data-testid='submit-input']").click();

    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  })

  it("should take the user to the 'change password' page", () => {
    cy.get(".change-password").click();
    cy.url().should('include', '/change-password');
  })
})
