```ruby
EntityRepository_CountryRepository_CountryInfo_Alpha2CountryCode ICountryRepository CountryRepository

    get async => 
        id = @Alpha2CountryCode.id.value
        return readOnlyDatabase.Countries.where (=> c.CountryCode == id) -> .toList
```

```ruby
ExchangeController
    WithdrawalFunctions functions
    CustomErrorMessages errors

    execute async =>
        command = @WithdrawalEnrichedCommand
        request = command.Request

        withdrawalResult = await functions.withdrawOrder
            Long.command.Customer.CustomerId 
            command.Request.Amount
            request.Currency
            command.FinalAddress
            
        if no withdrawalResult
            return apiError errors.General_name errors.General
        
        if withdrawalResult.Status != STATUS_SUCCESS
            return mapWithdrawalError withdrawalResult
        
        uniqueList = withdrawalResult.uniqueId.split ',' -> $.0

        if no uniqueLink
            return apiError errors.BadRequest_name errors.BadRequest

        result = await functions.verify uniqueLink "Withdrawal"

        if no result
            return apiError errors.General_name errors.General

        if result.Status != STATUS_SUCCESS
            return mapWithdrawalConfirmationError result
```