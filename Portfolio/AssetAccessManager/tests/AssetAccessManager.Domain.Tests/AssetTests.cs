using AssetAccessManager.Domain;
namespace AssetAccessManager.Domain.Tests;

[TestClass]
public sealed class AssetTests
{
    [TestMethod] public void Assignment_and_return_follow_lifecycle() { var asset = Asset.Register("LT-1", "SN-1", "Laptop"); asset.MarkAssigned(); Assert.AreEqual(AssetStatus.Assigned, asset.Status); asset.MarkReturned(); Assert.AreEqual(AssetStatus.Available, asset.Status); Assert.AreEqual(2, asset.Version); }
    [TestMethod] public void Duplicate_assignment_does_not_change_version() { var asset = Asset.Register("LT-1", "SN-1", "Laptop"); asset.MarkAssigned(); Assert.ThrowsExactly<InvalidOperationException>(asset.MarkAssigned); Assert.AreEqual(1, asset.Version); }
}
