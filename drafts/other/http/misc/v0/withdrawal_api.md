```ruby
#ApiController
#Route="~Withdrawals"
ControllerBase WithdrawalRepositoryApi
    IMediator _mediator

    #HttpGet="GetById/{id}"
    #ProducesResponseType _Withdrawal HttpStatus.Ok
    #ProducesResponseType HttpStatus.BadRequest
    getById async =>
        result = await _mediator send (getWithdrawalByIdQuery @Long.id)
        return ok result

    #HttpPost
    #ProducesResponseType _Withdrawal HttpStatus.Ok 
    #ProducesResponseType HttpStatus.BadRequest
    create async=>
        result = await _mediator send (createWithdrawalCommand @WithdrawalApiModel.FromBody)
        return ok result

    #HttpDelete "DeleteUnconfirmed" 
    #ProducesResponseType HttpStatus.Ok 
    #ProducesResponseType HttpStatus.BadRequest
    deleteUnconfirmed async =>
        await _mediator send (deleteUnconfirmedWithdrawalCommand @Long.FromQuery.customerId @String.FromQuery.currency)
        return ok

    #HttpPut "Confirm"
    #ProducesResponseType HttpStatus.Ok
    #ProducesResponseType HttpStatus.BadRequest
    confirm async =>
        await _mediator send (confirmWithdrawalCommand @WithdrawalConfirmationApiModel.FromBody)
        return ok
```

```ruby
WithdrawalApiModel #data
    Long Id
    Long CustomerId
    Decimal Amount
    Decimal Fee
    String Currency
    String TransactionId
    Bool Confirmed
    ?DateTime ConfirmationTime
    DateTime RequestTime

    toWithdrawal impilicitConverter =>
        return withdrawal 
            Id 
            CustomerId 
            Amount 
            Fee 
            Currency 
            TransactionId 
            Confirmed 
            ConfirmationTime 
            RequestTime
    
    fromWithdrawal Withdrawal.withdrawal impilicitConverter =>
        return withdrawalApiModel 
            @Withdrawal.withdrawal.Id.Value 
            @withdrawal.CustomerId 
            @withdrawal.Amount 
            @withdrawal.Fee 
            @withdrawal.Currency 
            @withdrawal.TransactionId 
            @withdrawal.Confirmed 
            @withdrawal.ConfirmationTime 
            @withdrawal.RequestTime
```