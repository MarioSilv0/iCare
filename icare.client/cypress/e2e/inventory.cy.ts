describe('Inventory page', () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/inventory"); // Ensure you visit after login

    cy.get('[data-testid="search-input"]').should('exist')
    cy.get('[data-testid="add-item-bt"]').should('exist')
    cy.get('[data-testid="item-list"]').should('exist')
  });

  it('should load the inventory page', () => {
    cy.visit(`https://127.0.0.1:4200/#/inventory`);
    cy.get('[data-testid="inventory-container"]').should('exist')
  })

  // search for an item
  it("should filter the item list", () => {

    cy.get('[data-testid="item-list"]').children('li').should('have.length.above', 1)

    cy.get('[data-testid="search-input"]')
      .clear()
      .type("nhoque")

    cy.get('[data-testid="item-list"]').children('li').should('have.length', 1)
  })

  // add an item
  it("should allow the user to add items on his inventory", () => {
    cy.intercept("PUT", "https://127.0.0.1:4200/api/Inventory", {
      statusCode: 200,
      body: {
        name: "Nhoque, batata, cozido",
        quantity: 1,
        unit:""
      }
    }).as("addItem")

    cy.get('[data-testid="item-list"]').children('li').should('have.length.above', 0)

    cy.get('[data-testid="search-input"]')
      .clear()
      .type("Nhoque, batata, cozido")

    cy.get('[data-testid="item-list"]').children('[data-testid="item"]').should('have.length', 1)

    cy.get('[data-testid="item"]').click() // select the item
    cy.wait(100)
    cy.get('[data-testid="add-item-bt"]').click() // add to inventory

    cy.wait("@addItem", {
      timeout: 10_000
    }).its('response.statusCode').should('eq', 200);

    cy.get('[data-testid="inventory-table"]').should("exist")
    cy.get('[data-testid="empty-row"]').should("not.exist")
    cy.get('[data-testid="inventory-item"]').should("exist")
  })

  // remove an item
  it("should allow the user to remove selected items of his inventory", () => {
    cy.intercept("DELETE", "https://127.0.0.1:4200/api/Inventory", {
      statusCode: 200,
      body: {
        names: ["Batata"],
      }
    }).as("removeItem")

    cy.get('[data-testid="item-list"]').children('li').should('have.length.above', 0) // make sure list isnt empty
    cy.get('[data-testid="check-item"]').first().click()
    cy.wait(500)
    cy.get('[data-testid="delete-modal"]').click({ force: true })
    cy.wait(500)
    cy.get('[data-testid="delete-bt"]').click({ force: true })
    cy.wait("@removeItem", {
        timeout: 10_000
    }).its('response.statusCode').should('eq', 200);
  })

  // update quantity of item
  it("should allow the user to change the quantity of an item", () => {
    cy.intercept("PUT", "https://127.0.0.1:4200/api/Inventory", {
      statusCode: 200,
      body: {
        names: "Batata",
        quantity: 7,
        unit: "g"
      }
    }).as("updateItem")

    cy.get('[data-testid="item-quantity"]').first().clear().type("07")
    cy.wait(500)
    cy.get('.update-button').click({ force: true })

    cy.wait("@updateItem", {
      timeout: 10_000
    }).its('response.statusCode').should('eq', 200);
  })
})
