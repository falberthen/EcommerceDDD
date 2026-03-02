const USER_EMAIL = 'user@test.com';
const USER_PASSWORD = 'Password123!';

const QUOTE_ID = '00000000-0000-0000-0000-000000000001';
const ORDER_ID = '00000000-0000-0000-0000-000000000002';
const CUSTOMER_ID = '00000000-0000-0000-0000-000000000003';

const PRODUCTS = [
  { productId: '00000000-0000-0000-0000-000000000101', name: 'MacBook Pro', price: 1999.99, currencySymbol: '$', quantityInStock: 100 },
  { productId: '00000000-0000-0000-0000-000000000102', name: 'iPhone 15', price: 999.99, currencySymbol: '$', quantityInStock: 50 }
];

function setupApiMocks(): void {
  let quoteItems: Array<{
    productId: string;
    productName: string;
    quantity: number;
    unitPrice: number;
    currencySymbol: string;
  }> = [];

  cy.intercept('POST', '**/api/v2/accounts/login*', {
    statusCode: 200,
    body: { accessToken: 'token' }
  }).as('loginRequest');

  cy.intercept('GET', '**/customerManagement/api/v2/customers/details*', {
    statusCode: 200,
    body: { customerId: CUSTOMER_ID, name: 'Test User', email: USER_EMAIL }
  }).as('getCustomerDetails');

  cy.intercept('GET', '**/quoteManagement/api/v2/quotes*', request => {
    request.reply({
      statusCode: 200,
      body: {
        quoteId: QUOTE_ID,
        customerId: CUSTOMER_ID,
        currencyCode: 'USD',
        currencySymbol: '$',
        totalPrice: quoteItems.reduce((total, item) => total + item.unitPrice * item.quantity, 0),
        items: quoteItems
      }
    });
  }).as('getOpenQuote');

  cy.intercept('POST', '**/productCatalog/api/v2/products*', request => {
    request.reply({
      statusCode: 200,
      body: PRODUCTS.map(product => ({
        ...product,
        quantityAddedToCart: quoteItems.find(item => item.productId === product.productId)?.quantity ?? 0
      }))
    });
  }).as('getProducts');

  cy.intercept('POST', '**/quoteManagement/api/v2/quotes*', {
    statusCode: 201,
    body: { quoteId: QUOTE_ID }
  }).as('createQuote');

  cy.intercept('PUT', '**/quoteManagement/api/v2/quotes/*/items*', request => {
    const { productId, quantity } = request.body as { productId: string; quantity: number };
    const product = PRODUCTS.find(item => item.productId === productId);

    if (!product) {
      request.reply({ statusCode: 400, body: { error: 'Unknown productId' } });
      return;
    }

    quoteItems = quoteItems.filter(item => item.productId !== productId);
    quoteItems.push({
      productId,
      productName: product.name,
      quantity,
      unitPrice: product.price,
      currencySymbol: product.currencySymbol
    });
    request.reply({ statusCode: 204 });
  }).as('upsertQuoteItem');

  cy.intercept('DELETE', '**/quoteManagement/api/v2/quotes/*/items/*', request => {
    const productId = request.url.split('/').pop() ?? '';
    quoteItems = quoteItems.filter(item => item.productId !== productId);
    request.reply({ statusCode: 204 });
  }).as('removeQuoteItem');

  cy.intercept('POST', '**/orderProcessing/api/v2/orders/quote/*', request => {
    quoteItems = [];
    request.reply({ statusCode: 201, body: { orderId: ORDER_ID } });
  }).as('placeOrder');

  cy.intercept('GET', '**/orderProcessing/api/v2/orders*', {
    statusCode: 200,
    body: []
  }).as('getOrders');
}

describe('EcommerceDDD E2E Flow', () => {
  beforeEach(() => {
    setupApiMocks();
  });

  it('should login and navigate to home', () => {
    cy.visit('/login');
    cy.get('#email').type(USER_EMAIL);
    cy.get('#password').type(USER_PASSWORD);
    cy.get('input[type="submit"]').click();
    cy.wait('@loginRequest');
    cy.visit('/home');
    cy.url().should('include', '/home');
    cy.contains('h1', 'Welcome to Ecommerce DDD!').should('be.visible');
  });

  it('should add products to cart and place an order', () => {
    cy.login(USER_EMAIL, USER_PASSWORD);
    cy.visit('/products');
    cy.wait('@getProducts');

    cy.addProductToCart('MacBook Pro');
    cy.wait('@upsertQuoteItem');
    cy.contains('.cart-details-container', 'MacBook Pro').should('exist');

    cy.addProductToCart('iPhone 15');
    cy.wait('@upsertQuoteItem');
    cy.contains('.cart-details-container', 'iPhone 15').should('exist');

    cy.placeOrder();
    cy.wait('@placeOrder');
    cy.url().should('include', '/orders');
  });

  it('should remove items from the cart', () => {
    cy.login(USER_EMAIL, USER_PASSWORD);
    cy.visit('/products');
    cy.wait('@getProducts');

    cy.addProductToCart('MacBook Pro');
    cy.wait('@upsertQuoteItem');
    cy.contains('.cart-details-container', 'MacBook Pro').should('exist');

    cy.removeFromCart('MacBook Pro');
    cy.wait('@removeQuoteItem');
    cy.contains('.cart-details-container', 'MacBook Pro').should('not.exist');
    cy.contains('.empty-cart-message', 'Your cart is empty').should('exist');
  });
});
