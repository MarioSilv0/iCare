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

    cy.get('[data-testid="item-list"]').children('li').should('have.length.above', 1)

    cy.get('[data-testid="search-input"]')
      .clear()
      .type("Nhoque, batata, cozido")

    cy.get('[data-testid="item-list"]').children('li').should('have.length', 1)

    cy.get('[data-testid="item"]').click() // select the item

    cy.get('[data-testid="add-item-bt"]').click() // add to inventory

    cy.wait("@addItem").its('response.statusCode').should('eq', 200);

    //check inventory
    cy.get('[data-testid="inventory-item"]').should("exist")
  })

  // remove an item
  // update inventory
  // update quantity of item
  // change weight scale of an item
})
