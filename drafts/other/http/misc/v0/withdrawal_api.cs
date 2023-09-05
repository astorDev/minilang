public record WithdrawalApiModel(long Id,
                                 long CustomerId,
                                 decimal Amount,
                                 decimal Fee,
                                 string Currency,
                                 string TransactionId,
                                 bool Confirmed,
                                 DateTime? ConfirmationDate,
                                 DateTime RequestDate,
                                 string Status,
                                 string? Particulars,
                                 string UserConfirmationId,
                                 decimal AmountUsd,
                                 decimal CarrierFee,
                                 string SourceAddress,
                                 string DestinationAddress
                                 )
{
    public static implicit operator Withdrawal(WithdrawalApiModel apiModel)
    {
        return new(
            Id: apiModel.Id,
            CustomerId: apiModel.CustomerId,
            Amount: apiModel.Amount,
            Fee: apiModel.Fee,
            Currency: apiModel.Currency!,
            WithdrawalTransactionId: apiModel.TransactionId,
            Confirmed: apiModel.Confirmed,
            ConfirmDate: apiModel.ConfirmationDate,
            RequestDate: apiModel.RequestDate,
            Status: Enum.Parse<WithdrawalStatus>(apiModel.Status),
            Particulars: apiModel.Particulars!,
            UserConfirmationId: apiModel.UserConfirmationId,
            AmountUsd: apiModel.AmountUsd,
            CarrierFee: apiModel.CarrierFee,
            SourceAddress: apiModel.SourceAddress,
            DestinationAddress: apiModel.DestinationAddress
        );
    }

    public static implicit operator WithdrawalApiModel(Withdrawal entity)
    {
        return new(
            Id: entity.Id!.Value,
            CustomerId: entity.CustomerId.Value,
            Amount: entity.Amount.Value,
            Fee: entity.Fee.Value,
            Currency: entity.Currency!,
            TransactionId: entity.WithdrawalTransactionId,
            Confirmed: entity.Confirmed,
            ConfirmationDate: entity.ConfirmDate,
            RequestDate: entity.RequestDate,
            Status: entity.Status.ToString(),
            Particulars: entity.Particulars,
            UserConfirmationId: entity.UserConfirmationId!,
            AmountUsd: entity.AmountUsd!.Value,
            CarrierFee: entity.CarrierFee!.Value,
            SourceAddress: entity.SourceAddress!,
            DestinationAddress: entity.DestinationAddress!
        );
    }
}