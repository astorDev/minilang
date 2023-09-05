```csharp
using BackOffice.PaymentSystem;
using BackOffice.PaymentSystem.Core.Payments;
using BackOffice.PaymentSystem.External.Leantech;
using Moq;

namespace PaymentSystem.Tests.Infrastructure.Withdrawals;

public class LeanTechPaymentReconciliationUpdatedHandlerTest
{
    private readonly Mock<IDepositOrderProcessor> _processorMock = new();
    private readonly Mock<IFiatManualDepositRepository> _repositoryMock = new();
    private readonly LeanTechPaymentReconciliationUpdatedHandler _handler;

    public LeanTechPaymentReconciliationUpdatedHandlerTest()
    {
        _handler = new(_processorMock.Object, _repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReconciledStatus_CallsProcessorApproveAsync()
    {
        var deposit = KnownFiatManualDeposit.Pending;
        var @event = LeanTechEventFactory.CreatePaymentReconciliationUpdated(
            deposit.TransactionId,
            LeanTechPaymentReconciliationUpdatedHandler.ReconciledStatus);
        
        _repositoryMock.Setup(m => m.GetAsync(deposit.TransactionId)).ReturnsAsync(deposit);
        await _handler.Handle(@event, CancellationToken.None);

        _processorMock.Verify(m => m.ApproveAsync(deposit));
    }
}
```