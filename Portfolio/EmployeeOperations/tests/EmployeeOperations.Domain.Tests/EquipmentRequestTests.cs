using EmployeeOperations.Domain;
namespace EmployeeOperations.Domain.Tests;

[TestClass]
public sealed class EquipmentRequestTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 2, 0, 0, 0, TimeSpan.Zero);
    [TestMethod]
    [DataRow("submit", EquipmentRequestStatus.Submitted)]
    [DataRow("approve", EquipmentRequestStatus.Approved)]
    [DataRow("reject", EquipmentRequestStatus.Rejected)]
    [DataRow("complete", EquipmentRequestStatus.Completed)]
    [DataRow("cancel-draft", EquipmentRequestStatus.Cancelled)]
    [DataRow("cancel-submitted", EquipmentRequestStatus.Cancelled)]
    public void Every_valid_transition_records_history(string scenario, EquipmentRequestStatus expected)
    {
        var r = EquipmentRequest.Create("employee-1", "Laptop", "Replacement", Now);
        if (scenario is "submit" or "approve" or "reject" or "complete" or "cancel-submitted") r.Submit("employee-1", Now.AddMinutes(1));
        if (scenario is "approve" or "complete") r.Approve("manager-1", "Approved", Now.AddMinutes(2));
        if (scenario == "reject") r.Reject("manager-1", "Rejected", Now.AddMinutes(2));
        if (scenario == "complete") r.Complete("ops-1", "Issued", Now.AddMinutes(3));
        if (scenario.StartsWith("cancel")) r.Cancel("employee-1", "Changed mind", Now.AddMinutes(4));
        Assert.AreEqual(expected, r.Status); Assert.AreEqual(expected, r.Transitions.Last().NewStatus);
        Assert.IsFalse(string.IsNullOrWhiteSpace(r.Transitions.Last().ActorId)); Assert.IsTrue(r.Transitions.Last().OccurredAt > Now);
    }

    [TestMethod]
    [DataRow(EquipmentRequestStatus.Draft, "approve")]
    [DataRow(EquipmentRequestStatus.Draft, "reject")]
    [DataRow(EquipmentRequestStatus.Draft, "complete")]
    [DataRow(EquipmentRequestStatus.Submitted, "submit")]
    [DataRow(EquipmentRequestStatus.Submitted, "complete")]
    [DataRow(EquipmentRequestStatus.Approved, "submit")]
    [DataRow(EquipmentRequestStatus.Approved, "approve")]
    [DataRow(EquipmentRequestStatus.Approved, "reject")]
    [DataRow(EquipmentRequestStatus.Approved, "cancel")]
    [DataRow(EquipmentRequestStatus.Rejected, "submit")]
    [DataRow(EquipmentRequestStatus.Rejected, "approve")]
    [DataRow(EquipmentRequestStatus.Rejected, "complete")]
    [DataRow(EquipmentRequestStatus.Rejected, "cancel")]
    [DataRow(EquipmentRequestStatus.Completed, "submit")]
    [DataRow(EquipmentRequestStatus.Completed, "approve")]
    [DataRow(EquipmentRequestStatus.Completed, "cancel")]
    [DataRow(EquipmentRequestStatus.Cancelled, "submit")]
    [DataRow(EquipmentRequestStatus.Cancelled, "approve")]
    [DataRow(EquipmentRequestStatus.Cancelled, "complete")]
    public void Invalid_transition_does_not_mutate(EquipmentRequestStatus current, string operation)
    {
        var r = At(current); var version = r.Version; var count = r.Transitions.Count;
        Assert.ThrowsExactly<InvalidRequestTransitionException>(() => Apply(r, operation));
        Assert.AreEqual(current, r.Status); Assert.AreEqual(version, r.Version); Assert.HasCount(count, r.Transitions);
    }
    private static EquipmentRequest At(EquipmentRequestStatus s)
    {
        var r = EquipmentRequest.Create("employee-1", "Laptop", "Needed", Now);
        if (s != EquipmentRequestStatus.Draft) r.Submit("employee-1", Now.AddMinutes(1));
        if (s is EquipmentRequestStatus.Approved or EquipmentRequestStatus.Completed) r.Approve("manager-1", null, Now.AddMinutes(2));
        if (s == EquipmentRequestStatus.Rejected) r.Reject("manager-1", null, Now.AddMinutes(2));
        if (s == EquipmentRequestStatus.Completed) r.Complete("ops-1", null, Now.AddMinutes(3));
        if (s == EquipmentRequestStatus.Cancelled) r.Cancel("employee-1", null, Now.AddMinutes(2));
        return r;
    }
    private static void Apply(EquipmentRequest r, string op)
    { if (op == "submit") r.Submit("actor", Now.AddHours(1)); else if (op == "approve") r.Approve("actor", null, Now.AddHours(1)); else if (op == "reject") r.Reject("actor", null, Now.AddHours(1)); else if (op == "complete") r.Complete("actor", null, Now.AddHours(1)); else r.Cancel("actor", null, Now.AddHours(1)); }
}
