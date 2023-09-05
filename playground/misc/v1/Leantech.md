```ruby
compile
    csharp namespace:'BackOffice.PaymentSystem.MiddleOffice.ExternalServices.LeanTech'

LeanTechEvent_T IRequest_Object data
    String Type
    String Message
    DateTime Timestamp
    String EventId
    T Payload

PaymentCreatedPayload data
    String Id
    String CustomerId
    String IntentId
    String Status
    Decimal Amount
    String Currency
    String BankTransactionReference

PaymentReconciliationUpdatedPayload data
    String PaymentId
    String PaymentIntentId
    String PaymentDescription
    String Status
    String Reference
    Decimal Amount
    String Currency
```
