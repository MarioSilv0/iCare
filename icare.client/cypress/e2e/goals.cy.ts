describe('Goal page', () => {
  beforeEach(() => {
    cy.login();
    cy.visit("https://127.0.0.1:4200/#/goal"); // Ensure you visit after login
  });

  it('should load the goal page', () => {
    cy.get('[data-testid="goals-container"]').should('exist')
  })

  it('should display the help component', () => {
    cy.get('.help-container help').should('exist');
  });

  it('should allow manual goal creation when valid info exists', () => {
    cy.get('form.goal-form').within(() => {
      cy.get('#ipt-calorias-diarias').type('2000');

      cy.get('[data-testid="calendar"] input[name="ipt-start-date"]', { includeShadowDom: true }).type('2025-04-01');
      cy.get('[data-testid="calendar"] input[name="ipt-end-date"]', { includeShadowDom: true }).type('2025-04-30');
      cy.wait(500)
      cy.get('.bt-add-goal').click({ force: true });
    });
  });

  it('should allow automatic goal creation when valid info exists', () => {
    cy.get('form.goal-form').within(() => {
      cy.get('[data-testid="objective"]').select(1);

      cy.get('[data-testid="calendar"] input[name="ipt-start-date"]', { includeShadowDom: true }).type('2025-04-01');
      cy.get('[data-testid="calendar"] input[name="ipt-end-date"]', { includeShadowDom: true }).type('2025-04-30');
      cy.wait(500)
      cy.get('.bt-add-goal').click({ force: true });
    });
  });

  it('should open delete confirmation and delete the goal', () => {
    cy.contains('Excluir Meta').click();
    cy.get('#deleteModal').should('be.visible');
    cy.contains('Eliminar').click(); // This clicks the confirm delete button
  });

})
