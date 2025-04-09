describe("Profile", () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/profile"); // Ensure you visit after login
  });

  function updateUserField(selector: string, value: string | number) {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get(selector)
      .then($el => {
        if ($el.is('select')) {
          cy.wrap($el).select(value.toString());
        } else {
          cy.wrap($el).clear().type(value.toString());
        }
      });

    cy.get("[data-testid='submit-input']").click({force: true});
    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  }

  function selectFirstOptionInDatalist(selector: string) {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get(selector)
      .find("option")
      .first()
      .click({ force: true });

    cy.get("[data-testid='submit-input']").click();
    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  }

  it("should load the profile page", () => {
    cy.get('[data-testid="profile-page"]').should('exist');
  });

  it("should allow the user to add a profile picture", () => {
    cy.get('[data-testid="profile-image"]').should('exist');

    cy.fixture('cat-with-glasses.jpg', 'base64').then((fileContent) => {
      cy.get('[data-testid="file-input"]').selectFile(
        {
          contents: Cypress.Buffer.from(fileContent, 'base64'),
          fileName: 'cat-with-glasses.jpg',
          mimeType: 'image/jpeg',
          lastModified: Date.now(),
        },
        { force: true }
      );
    });

    cy.get('[data-testid="image"]', { timeout: 10000 })
      .should("have.attr", "src")
      .then((src) => {
        expect(src).to.not.contain('default.jpg');
      });
  });

  it("should allow the user to change his name", () => {
    updateUserField("[data-testid='name-input'] input", "New Name");
  });

  it("should allow the user to change his email", () => {
    updateUserField("[data-testid='email-input'] input", "john@doe.com");
  });

  it("should allow the user to change his birthdate", () => {
    updateUserField("[data-testid='birthday-input'] input", "2001-04-23");
  });

  it("should allow the user to change his height", () => {
    updateUserField("[data-testid='height-input'] input", "1.53");
  });

  it("should allow the user to change his weight", () => {
    updateUserField("[data-testid='weight-input'] input", "80");
  });

  it("should allow the user to change his gender", () => {
    updateUserField("[data-testid='gender-input']", "Feminino");
  });

  it("should allow the user to change his activity level", () => {
    updateUserField("[data-testid='activity-input']", "Sedentário");
  });

  it("should allow the user to turn on/off notifications", () => {
    cy.intercept('PUT', `https://127.0.0.1:4200/api/User`, {
      statusCode: 200,
    }).as('updateUser');

    cy.get("[data-testid='notifications-input'] input")
      .should("not.be.checked")
      .click()
      .should("be.checked");

    cy.get("[data-testid='submit-input']").click();
    cy.wait('@updateUser').its('response.statusCode').should('eq', 200);
  });

  it("should allow the user to change his preferences", () => {
    selectFirstOptionInDatalist("[data-testid='preferences-input'] datalist");
  });

  it("should allow the user to change his restrictions", () => {
    selectFirstOptionInDatalist("[data-testid='restrictions-input'] datalist");
  });

  it("should take the user to the 'change password' page", () => {
    cy.get(".change-password").click();
    cy.url().should('include', '/change-password');
  });
});
