namespace EcommerceDDD.CustomerManagement.Application.GettingStoreCredit;

public record class StoreCreditModel
(
	Guid CustomerId,
	decimal StoreCredit
);