```csharp
namespace BackOffice.PaymentSystem.MiddleOffice.ExternalServices.LeanTech;

public record LeanTechEvent<T>(
    String Type,
    String Message,
    DateTime Timestamp,
    String EventId,
    T Payload
) : IRequest<object>;

public record PaymentCreatedPayload(
    String Id,
    String CustomerId,
    String IntentId,
    String Status,
    Decimal Amount,
    String Currency,
    String BankTransactionReference
);
```