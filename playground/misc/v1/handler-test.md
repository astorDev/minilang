```ruby
LeanTechPaymentReconciliationUpdatedHandlerTest
    Mock_DepositOrderProcessor _processorMock = mock_depositOrderProcessor
    Mock_FiatManualDepositRepository _repositoryMock = mock_fiatManualDepositRepository
    LeanTechPaymentReconciliationUpdatedHandler _handler = leanchTechPaymentReconciliationUpdatedHandler 
        _processorMock.Object 
        _repositoryMock.Object

    handleReconciledStatusCallsProcessorApprove async
        deposit = knownPendingFiatManualDeposit
        event = generatePaymentReconciliationUpdatedEvent deposit.TransactionId RECONCILED
        
        _repositoryMock.return deposit
            on => @.getAsync deposit.transactionId

        _handler.handle event

        _processorMock.hasCallOf
            => @.ApproveAsync deposit
```