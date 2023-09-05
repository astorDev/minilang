```ruby
WalletController partial
    ICustomErrorMessages _errors
    Whatever _dataGenerator

    #TypeFilter _AuthenticateBearerToken Arguments=Object[true]
    #HttpPost
    #Route "withdraw-fiat"
    #ProducesResponseType _Response_string StatusCodes>Status200OK
    withdrawFiat async
        try
            result = await buildWithdrawFiatCommand(@WithdrawFiatCommand.FromBody) -> andThen validate -> andThen execute

            return result match
                => Response_object _errors.Success_General._name "Request Approved"
                => @fail
        catch Exception.ex
            return Response_object _errors.Failure_General._name @ex.Message

    buildWithdrawalFiatCommand async returns:OneOf_WithdrawalFiatEnrichedCommand_Response~object
        request = @WithdrawalFiatCommand
        clientId = _helper.GetClientId routeData.values

        if no await _dataGenerator.isForcedKycPassed clientId
            return response_Object STATUS_ERROR _errors.kycNotApproved._name _errors.kycNotApproved

        if no await _dataGenerator.getKycSettingStatus clientId "Withdrawal"
            return response_Object STATUS_ERROR _errors.kycNotApproved._name _errors.kycNotApproved

        if isNullOrWhiteSpace r.emailToken or isNullOrWhiteSpace r.emailOtp
            return response_Object STATUS_ERROR _error.badRequest "Invalid OTP"

        currencyPreferences = await _cacheService.getCurrencyAsync r.currencyName

        if r.accountId <= 0
            return withdrawFiatEnrichedCommand request clientId currencyPreferences

        requisites = await _bankPaymentRequisiteService.getAsync r.accountId
        customerName = await _singleton.getCustomerDetails clientId -> .name

        r.cid = clientId
        r.comments = ""
        r.bankName = requisites.bankName
        r.beneficiaryName = customerName
        r.accountNumber = requisites.iban
        r.swiftCode = requisites.bankSwift

        return withdrawFiatEnrichedCommand request clientId currencyPreferences


```