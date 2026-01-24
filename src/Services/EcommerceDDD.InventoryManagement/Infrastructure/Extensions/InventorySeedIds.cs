namespace EcommerceDDD.InventoryManagement.Infrastructure.Extensions;

/// <summary>
/// Deterministic product IDs matching ProductCatalog seed data.
/// Used for seeding inventory without calling ProductCatalog service.
/// </summary>
public static class InventorySeedIds
{
	// Men's clothing
	public static readonly Guid FjallravenBackpack = new("a1b2c3d4-1111-4000-8000-000000000001");
	public static readonly Guid MensCasualTShirt = new("a1b2c3d4-1111-4000-8000-000000000002");
	public static readonly Guid MensCottonJacket = new("a1b2c3d4-1111-4000-8000-000000000003");
	public static readonly Guid MensCasualSlimFit = new("a1b2c3d4-1111-4000-8000-000000000004");

	// Jewelry
	public static readonly Guid JohnHardyBracelet = new("a1b2c3d4-2222-4000-8000-000000000001");
	public static readonly Guid SolidGoldMicropave = new("a1b2c3d4-2222-4000-8000-000000000002");
	public static readonly Guid WhiteGoldPrincess = new("a1b2c3d4-2222-4000-8000-000000000003");
	public static readonly Guid PiercedOwlEarrings = new("a1b2c3d4-2222-4000-8000-000000000004");

	// Electronics
	public static readonly Guid WD2TBHardDrive = new("a1b2c3d4-3333-4000-8000-000000000001");
	public static readonly Guid SanDiskSSD = new("a1b2c3d4-3333-4000-8000-000000000002");
	public static readonly Guid SiliconPowerSSD = new("a1b2c3d4-3333-4000-8000-000000000003");
	public static readonly Guid WD4TBGamingDrive = new("a1b2c3d4-3333-4000-8000-000000000004");
	public static readonly Guid AcerMonitor = new("a1b2c3d4-3333-4000-8000-000000000005");
	public static readonly Guid SamsungCurvedMonitor = new("a1b2c3d4-3333-4000-8000-000000000006");

	// Women's clothing
	public static readonly Guid WomensSnowboardJacket = new("a1b2c3d4-4444-4000-8000-000000000001");
	public static readonly Guid WomensFauxLeatherJacket = new("a1b2c3d4-4444-4000-8000-000000000002");
	public static readonly Guid WomensRainJacket = new("a1b2c3d4-4444-4000-8000-000000000003");
	public static readonly Guid WomensBoatNeckTop = new("a1b2c3d4-4444-4000-8000-000000000004");
	public static readonly Guid WomensMoistureTee = new("a1b2c3d4-4444-4000-8000-000000000005");
	public static readonly Guid WomensCottonTShirt = new("a1b2c3d4-4444-4000-8000-000000000006");

	/// <summary>
	/// Returns all product IDs with random initial quantities for inventory seeding.
	/// </summary>
	public static IReadOnlyList<(Guid ProductId, int InitialQuantity)> GetAllProductsWithQuantities()
	{
		var random = new Random(42); // Fixed seed for reproducible quantities
		return
		[
			(FjallravenBackpack, random.Next(10, 50)),
			(MensCasualTShirt, random.Next(10, 50)),
			(MensCottonJacket, random.Next(10, 50)),
			(MensCasualSlimFit, random.Next(10, 50)),
			(JohnHardyBracelet, random.Next(5, 20)),
			(SolidGoldMicropave, random.Next(5, 20)),
			(WhiteGoldPrincess, random.Next(5, 20)),
			(PiercedOwlEarrings, random.Next(5, 20)),
			(WD2TBHardDrive, random.Next(10, 50)),
			(SanDiskSSD, random.Next(10, 50)),
			(SiliconPowerSSD, random.Next(10, 50)),
			(WD4TBGamingDrive, random.Next(10, 50)),
			(AcerMonitor, random.Next(5, 30)),
			(SamsungCurvedMonitor, random.Next(5, 30)),
			(WomensSnowboardJacket, random.Next(10, 50)),
			(WomensFauxLeatherJacket, random.Next(10, 50)),
			(WomensRainJacket, random.Next(10, 50)),
			(WomensBoatNeckTop, random.Next(10, 50)),
			(WomensMoistureTee, random.Next(10, 50)),
			(WomensCottonTShirt, random.Next(10, 50))
		];
	}
}
