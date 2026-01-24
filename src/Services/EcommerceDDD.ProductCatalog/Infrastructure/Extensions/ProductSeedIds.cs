namespace EcommerceDDD.ProductCatalog.Infrastructure.Extensions;

/// <summary>
/// Deterministic product IDs for seeding.
/// These IDs are shared with InventoryManagement for consistent seed data.
/// </summary>
public static class ProductSeedIds
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
	/// Returns all product IDs for inventory seeding.
	/// </summary>
	public static IReadOnlyList<Guid> GetAllProductIds() =>
	[
		FjallravenBackpack, MensCasualTShirt, MensCottonJacket, MensCasualSlimFit,
		JohnHardyBracelet, SolidGoldMicropave, WhiteGoldPrincess, PiercedOwlEarrings,
		WD2TBHardDrive, SanDiskSSD, SiliconPowerSSD, WD4TBGamingDrive, AcerMonitor, SamsungCurvedMonitor,
		WomensSnowboardJacket, WomensFauxLeatherJacket, WomensRainJacket, WomensBoatNeckTop, WomensMoistureTee, WomensCottonTShirt
	];
}
