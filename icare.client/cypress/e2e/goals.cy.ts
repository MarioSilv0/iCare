describe('Goal page', () => {
  beforeEach(() => {
    cy.login();

    cy.intercept("GET", "https://127.0.0.1:4200/api/User", {
      statusCode: 200,
    }).as("getUser")

    cy.intercept("GET", "https://127.0.0.1:4200/api/goal", {
      statusCode: 204,
    }).as("getUserGoal")

    cy.visit("https://127.0.0.1:4200/#/goal"); // Ensure you visit after login
  });

  it('should load the goal page', () => {
    cy.get('[data-testid="goals-container"]').should('exist')
  })

  it('should display the help component', () => {
    cy.get('.help-container help').should('exist');
  });

  it("should display the information form if the user has no goal set", () => {
    cy.wait("@getUser").should(({ response: res }) => {
      expect(res?.statusCode).to.eq(200)
    })

    cy.wait("@getUserGoal").should(({ response: res }) => {
      expect(res?.statusCode).to.eq(204)
      expect(res?.body).to.eq("")
    })

    cy.get("[data-testid='info-form']")
      .should("exist")
      .should("be.visible")
  })

  it('should allow manual goal creation when valid info exists', () => {
    cy.intercept("GET", "https://127.0.0.1:4200/api/User", {
      statusCode: 200,
      body: {
        birthdate: '1990-01-01',
        height: 175,
        weight: 70,
        gender: 'Male',
        activityLevel: 'Moderately'
      }
    }).as("getUser");

    // Also return no goal, to trigger the "create goal" view
    cy.intercept("GET", "https://127.0.0.1:4200/api/goal", {
      statusCode: 204,
      body: null
    }).as("getUserGoal");

    cy.intercept("POST", "https://127.0.0.1:4200/api/goal", {
      statusCode: 201
    }).as("createGoal")

    // Visit after intercepts
    cy.visit("https://127.0.0.1:4200/#/goal");
    cy.wait("@getUser");
    cy.wait("@getUserGoal");

    // Now fill the form
    const calories = '8050';
    const startDate = '2025-04-01';
    const endDate = '2025-04-30';

    cy.get('form.goal-form').within(() => {
      cy.get('#ipt-calorias-diarias').type(calories, { force: true });

      cy.get('[data-testid="calendar"] input[name="ipt-start-date"]', { includeShadowDom: true })
        .type(startDate, { force: true });
      cy.get('[data-testid="calendar"] input[name="ipt-end-date"]', { includeShadowDom: true })
        .type(endDate, { force: true });
      cy.wait(500)
      cy.get('.bt-add-goal').click({ force: true });
      cy.wait("@createGoal").should(({ response: res }) => {
        expect(res?.statusCode).to.eq(201)
      })
    });
  });

  it('should allow automatic goal creation when valid info exists', () => {
    cy.intercept("GET", "https://127.0.0.1:4200/api/User", {
      statusCode: 200,
      body: {
        birthdate: '1990-01-01',
        height: 175,
        weight: 70,
        gender: 'Male',
        activityLevel: 'Moderately'
      }
    }).as("getUser");

    // Also return no goal, to trigger the "create goal" view
    cy.intercept("GET", "https://127.0.0.1:4200/api/goal", {
      statusCode: 204,
      body: null
    }).as("getUserGoal");

    cy.intercept("POST", "https://127.0.0.1:4200/api/goal", {
      statusCode: 201,
      body: {
        autoGoalType: "Perder Peso",
        calories: 2000,
        endDate: "2025-04-08",
        goalType: "Automática",
        startDate: "2025-04-08"
      }
    }).as("createGoal")

    // Visit after intercepts
    cy.visit("https://127.0.0.1:4200/#/goal");
    cy.wait("@getUser");
    cy.wait("@getUserGoal");
    cy.wait(500)
    cy.get('.bt-add-goal').click({ force: true });
    cy.wait("@createGoal").should(({ response: res }) => {
      expect(res?.statusCode).to.eq(201)
    });
  });
})
