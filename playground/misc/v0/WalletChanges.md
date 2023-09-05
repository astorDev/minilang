```ruby
WalletChanges
    Array_Transfer transfers
    Array_Expenses expenses
    Array_Income incomes

    empty => expenses.empty && incomed.empty && transfers.empty

    getWalletExpenses
        if expenses.empty return null
        
        contextExpenses = dictionary_String_ContextExpenses

        for expenseInContext in expenses.groupBy (=> @.contextId)
            categories = categoriesCollectionFrom expenseInContext
            distribution = distributionFrom categories @Distribution
            contextName = @Array_Contexts.single => @.id == expenseInContext.key -> .name

            contextExpenses.add contextName, distribution.toContextExpenses

        total = contextExpenses.values.sum => @.total

        allFlatDistributions = contextExpenses.values.selectMany => @.distributions

        summedDistribution = allFlatDistributions.groupBy => @.Key 
            -> .select => anon 
                @.key 
                @.first.value.name 
                @.sum => @.value.sum

        allDistribution = summedDistribution.toMap
            => @.animal
            => categorySector .name .sum (.sum / total * 100)

        return walletExpenses total allDistribution contextExpenses
    
    getIncomeCollection
        if incomes.empty return null

        return incomeCollection
            incomes.sum => @.sum
            incomes.arr => protocolIncome @.sum @.title

    getTransfersCollection
        result = getTransfersEnumerable -> .toArray
        if result.empty return null
        return result.length result
    
    getTransfersEnumerable enumerator
        for group in transfers.groupBy => .protocol.counterparty
            inSum = group.where => .protocol.direction is IN -> .sum => .protocol.sum
            outSum = group.where => .protocol.direction is OUT -> .sum => .protocol.sum

            balance = inSum - outSum
            if sum == 0 continue

            direction = IN
            if sum < 0
                direction = OUT
        
            yield protocolTransfer
                abs sum
                direction
                null
                group.key
```