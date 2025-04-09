describe("Profile", () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/profile"); // Ensure you visit after login
  });

  it("should load the profile page", () => {
    cy.get('[data-testid="profile-page"]').should('exist')
  });

  it("should allow the user to add a profile picture", () => {
    // Ensure the profile image container exists
    cy.get('[data-testid="profile-image"]').should('exist');

    // Load image fixture and simulate file selection
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

    // Wait for the image to update (handling async FileReader)
    cy.get('[data-testid="image"]', { timeout: 10000 }) // Wait up to 10 seconds
      .should("have.attr", "src")
      .then((src) => {
        // Ensure the src is not the default image anymore
        expect(src).to.not.contain('default.jpg'); // Adjust based on your default image
      });
  });

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
      .should("be.checked")
      .click()
      .should("not.be.checked")

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
