/// <reference types="cypress" />

declare namespace Cypress {
  interface Chainable {
    login(email?: string, password?: string): Chainable<void>;
    addProductToCart(productName: string): Chainable<void>;
    removeFromCart(productName: string): Chainable<void>;
    placeOrder(): Chainable<void>;
  }
}

Cypress.Commands.add('login', (email = 'user@test.com', password = 'Password123!') => {
  cy.visit('/login');
  cy.get('#email').type(email);
  cy.get('#password').type(password);
  cy.get('input[type="submit"]').click();
  cy.wait('@loginRequest');
  cy.visit('/home');
  cy.url().should('include', '/home');
});

Cypress.Commands.add('addProductToCart', (productName: string) => {
  cy.contains('.product-container', productName).within(() => {
    cy.contains('button', 'Add to Cart').click();
  });
});

Cypress.Commands.add('removeFromCart', (productName: string) => {
  cy.contains('.cart-details-container', productName).within(() => {
    cy.get('button.remove-item').click();
  });
  cy.contains('.modal-footer button', 'OK').click();
});

Cypress.Commands.add('placeOrder', () => {
  cy.contains('button', 'Place Order').click();
  cy.contains('.modal-footer button', 'OK').click();
});
