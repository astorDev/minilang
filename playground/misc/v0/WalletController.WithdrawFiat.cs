using BFF.Filters;
using HelperServices.Models;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using Venomex.Core.Customer;
using Venomex.MiddleOffice.Payments;

namespace BFF5.Controllers;

public partial class WalletController
{
    [TypeFilter(typeof(AuthenticateBearerToken), Arguments = new object[] { true })]
    [HttpPost]
    [Route("withdraw-fiat")]
    [ProducesResponseType(typeof(Response<string>), StatusCodes.Status200OK)]
    public async Task<Response<object>> WithdrawFiat([FromBody] WithdrawFiatCommand request)
    {
        try
        {
            var result = await BuildWithdrawFiatCommand(request)
                              .AndThen(Validate)
                              .AndThen(Execute);

            return result.Match(
                _ => new(StatusSuccess, nameof(_customErrorMessages.Success_General), "Request Approved."),
                fail => fail);
        }
        catch (Exception e)
        {
            return new(StatusError, nameof(_customErrorMessages.Exception_General), e.Message);
        }
    }

    protected async Task<OneOf<WithdrawFiatEnrichedCommand, Response<object>>> BuildWithdrawFiatCommand(WithdrawFiatCommand request)
    {
        var clientId = Helper.GetClientId(RouteData.Values);

        if (!await _dataGenerator.Is_Forced_KYC_Passed(clientId))
            return new Response<object>(
                StatusError,
                nameof(_customErrorMessages.KYC_Not_Approved),
                Convert.ToString(_customErrorMessages.KYC_Not_Approved));
        
        if (!await _dataGenerator.GetKycSettingStatus(clientId, "Withdrawal"))
            return new Response<object>(
                StatusError,
                nameof(_customErrorMessages.KYC_Not_Approved),
                Convert.ToString(_customErrorMessages.KYC_Not_Approved));
        
        if (string.IsNullOrWhiteSpace(request.email_token) || string.IsNullOrWhiteSpace(request.email_otp))
            return new Response<object>(
                StatusError, nameof(_customErrorMessages.Exception_BadRequest), Convert.ToString("Invalid OTP"));
        
        var currencyPreferences = await _cacheService.GetCurrencyAsync(request.CurrencyName);

        if (request.AccountId <= 0)
            return new WithdrawFiatEnrichedCommand(request, clientId, currencyPreferences);

        var requisites = await _bankPaymentRequisiteService.GetAsync(request.AccountId);

        var customerName = (await _singleton.GetCustomerDetails(clientId)).Name;
        request.CID = clientId;
        request.Comments = "";
        request.BankName = requisites.BankName;
        request.BeneficiaryName = customerName;
        request.AccountNumber = requisites.Iban;
        request.SwiftCode = requisites.BankSwift;

        return new WithdrawFiatEnrichedCommand(request, clientId, currencyPreferences);
    }

    protected async Task<OneOf<WithdrawFiatEnrichedCommand, Response<object>>> Validate(WithdrawFiatEnrichedCommand command)
    {
        var (request, clientId, currencyPreferences) = command;


        if (string.IsNullOrWhiteSpace(request.CurrencyName) || request.CurrencyName != currencyPreferences.Asset ||
            currencyPreferences is not { EnableWithdrawal: true } || request.RequestAmount <= 0 ||
            string.IsNullOrWhiteSpace(request.BankName) || request.BankName.Length < 2 ||
            string.IsNullOrWhiteSpace(request.BeneficiaryName) || string.IsNullOrWhiteSpace(request.AccountNumber) ||
            request.AccountNumber.Length < 2)
            return new Response<object>(
                StatusError,
                nameof(_customErrorMessages.Exception_BadRequest),
                Convert.ToString("Invalid Beneficiary or Currency"));

        var cus = (await _customerService.SearchCustomerByIdAsync(clientId))!;

        await _withdrawalLimitsValidator.ValidateAsync(request.CurrencyName, cus.KycLevel, request.RequestAmount);

        if (cus.IsAuthenticationRequired)
        {
            if (string.IsNullOrWhiteSpace(request.gauth_code))
                return new Response<object>(
                    StatusError,
                    nameof(_customErrorMessages.Exception_BadRequest),
                    Convert.ToString(_customErrorMessages.Exception_BadRequest));
            var faValidate = new FA_Validate { GoogleAuthKey = cus.GoogleAuthKey, gauth_code = request.gauth_code };
            {
                if (!await _dataGenerator.ValidateTwoFactorPin(faValidate))
                    return new Response<object>(
                        StatusError,
                        nameof(_customErrorMessages.GAuth_Two_Factor_Error),
                        Convert.ToString(_customErrorMessages.GAuth_Two_Factor_Error));
            }
        }

        var isValid = await _tokenService.Validate("Withdraw", clientId, request.email_token, request.email_otp);

        if (isValid == false)
            return new Response<object>(
                StatusError,
                nameof(_customErrorMessages.Exception_Invalid_Token_or_Expired),
                Convert.ToString(_customErrorMessages.Exception_Invalid_Token_or_Expired));

        if (cus.IsMobileVerified)
        {
            if (string.IsNullOrWhiteSpace(request.sms_token) || string.IsNullOrWhiteSpace(request.sms_otp))
                return new Response<object>(
                    StatusError,
                    nameof(_customErrorMessages.Exception_BadRequest),
                    Convert.ToString(_customErrorMessages.Exception_BadRequest));
            var isValid1 = await _tokenService.Validate("Withdraw-SMS", clientId, request.sms_token, request.sms_otp);

            if (isValid1 == false)
                return new Response<object>(
                    StatusError,
                    nameof(_customErrorMessages.Exception_Invalid_Token_or_Expired),
                    Convert.ToString(_customErrorMessages.Exception_Invalid_Token_or_Expired));
            await _tokenService.Remove("Withdraw-SMS", request.sms_token);
        }

        await _tokenService.Remove("Withdraw", request.email_token);
        var response1 = await _dataGenerator.CheckWithdrawalLimitExclusive(
            clientId,
            request.CurrencyName.Trim().ToUpper(),
            request.RequestAmount,
            cus.KycLevel,
            isFiat: true);

        if (response1.Status == "error")
            return new Response<object>(
                StatusError,
                nameof(_customErrorMessages.WithdrawalLimitReachedExclusive),
                response1.Message);
        
        var response = await _singleton.CheckWithdrawalLimitAggregate(
                           clientId,
                           request.CurrencyName.Trim().ToUpper(),
                           request.RequestAmount,
                           cus.KycLevel);

        if (response.Status == "error")
            return new Response<object>(
                StatusError,
                nameof(_customErrorMessages.WithdrawalLimitReachedAggregate),
                response.Message);

        return command;
    }

    protected async Task<OneOf<None, Response<object>>> Execute(WithdrawFiatEnrichedCommand command)
    {
        var (request, clientId, currencyPreferences) = command;

        var result = await _withdrawal.CreateFiatWithdrawal(
                         clientId,
                         request.RequestAmount,
                         request.CurrencyName,
                         request.BankName,
                         request.BeneficiaryName,
                         request.AccountNumber,
                         "",
                         request.SwiftCode);

        if (result.Status.ToLower() != "success")
        {
            return _withdrawalErrorsMapper.MapWithdrawalFailedResult(result).ToResponse(StatusError);
        }

        var spVerifyWithdrawal = await _withdrawal.VerifyWithdrawal(result.UniqueID, "Withdrawal");

        if (spVerifyWithdrawal.Status != StatusSuccess)
            return _withdrawalErrorsMapper.MapWithdrawalConfirmationFailedResult(result).ToResponse(StatusError);

        var telegramMessage =
            $"CID {clientId} has requested for a {request.RequestAmount} {currencyPreferences.Asset.Value}" +
            $" fiat-withdraw, please take action from the Admin panel.";
        await _singleton.SendTelegramNotification(telegramMessage);

        return new None();
    }
}