"> {x}" = "(new (x))"

```ruby
Test TestClass.GetTransactionsShould
    returnSavedTransactions async =>
        
        await client.postTransaction 
            transactionCandidate "1"

        await client.postTransaction
            transactionCandidate "2"

        result = await client.getTransactions transactionQuery

        assertEqual 2 result.Count
        assertEqual "1" result.0.Id
        assertEqual "2" result.1.Id

    filterByLabels async =>
        await client.post 
            transactionCandidate "1" 
            map "mood" "bar"
        
        await client.post
            transactionCandidate "2" 
            map "mood" "foo"

        result = await client.getTransactions
            transactionQuery 
                map "mood" "foo"

        assertEqual 1 result.Count
        assertEqual "2" result.0.Id

Test TestClass.PatchTransactionShould
    moveExpensesToOtherContextIfItExists TestMethod async =>
        aCallTo 
            => factory.Contexts.get 
                userId = "45" 
                contextQuery 0 1 name:"target"
            -> .returns
                contextCollection 1 "x"
                    [
                        context "99" "target" now
                    ]

        aCallTo 
            => contexts.delete user:"45" name:"source"
            -> .returns
                context user:"99" name:"source" created:now

        await client.patch
            transactionQuery
                tags = map "user" "45" "context" "source"
            transactionChanges 
                tags = map "context" "target"
        
        mustHaveHappened
            => factory.expenses.patch 
                userId = "45" 
                userExpenseseQuery contextId:"99"
                userExpensesChanges contextId:"11"
```