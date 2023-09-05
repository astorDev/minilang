namespace BackOffice.PaymentSystem.MiddleOffice.ExternalServices.LeanTech;

public record LeanTechEvent<T>(
    String Type,
    String Message,
    DateTime Timestamp,
    String EventId,
    T Payload
) : IRequest<Object>;

public record PaymentCreatedPayload(
    String Id,
    String CustomerId,
    String IntentId,
    String Status,
    Decimal Amount,
    String Currency,
    String BankTransactionReference
);

public record PaymentReconciliationUpdatedPayload(
    String PaymentId,
    String PaymentIntentId,
    String PaymentDescription,
    String Status,
    String Reference,
    Decimal Amount,
    String Currency
);

