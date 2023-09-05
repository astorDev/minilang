```ruby
IRequest_Object LeanTechEvent_T #data
    String Type
    String Message
    DateTime Timestamp
    String EventId
    T Payload

PaymentCreatedPayload #data
    String Id
    String CustomerId
    String IntentId
    String Status
    Decimal Amount
    
    String Currency
    String BankTransactionReference

    TYPE = "payment.created"
```

```ruby
ControllerBase Route="leantech".LeanTechWebhooksApi
    IMediator _mediator

    _SERIALIZER_OPTIONS = jsonSerializerOptions PropertyNamingPolicy:jsonSnakeCaseNamingPolicy
    _TYPED_EVENT_FACTORIES = dictionary PAYMENT_CREATED_PAYLOAD_TYPE *typedEvent_PaymentCreatePayload

    handle HttpPost="events" async =>
        body = @JsonElement.FromBody

        if no body.tryGetProperty "type" typeProperty
            return badRequest "Missing type property"
        
        if _TYPED_EVENT_FACTORIES.tryGetValue typeProperty.getString eventFactory
            typedEvent = eventFactory body
            response = await _mediator.send typedEvent

            return ok response

        return ok
    
    typedEvent_T JsonElement json =>
        return json.deserialize_LEANTECHEVENT~T _SERIALIZER_OPTIONS
```

```ruby
IRequestHandler_LeanTechEvent~PaymentCreatedPayload LeanTechPaymentCreatedHandler
    handle async =>
        return @LeanTechEvent_PaymentCreatedPayload
```