export const appConstants = {
  storedUser: 'user',
  storedCustomer: 'customer',
  storedCurrency: 'currency',
  storedOpenQuote: 'openQuote',
  defaultCurrency: 'USD'
};

export const orderStatusCodes = {
  placed: 1,
  paid: 2,
  shipped: 3,
  completed: 4,
  canceled: 0
}

export const boundedContexts = {
  Customers: "Customers",
  Quotes: "Quotes",
  Orders: "Orders"
}
