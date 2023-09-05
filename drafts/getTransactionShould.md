```ruby
Test GetTransactionsShould.TestClass
    returnSavedTransactions.TestMethod
        await client.post
            transactionCandidate '1'
        await client.post
            transactionCandidate '2'
    
        result = await client.get 
            transactionQuery

        assertEqual 2 result.count
        assertEqual '1' result.items.0.id
        assertEqual '2' result.items.1.id
    
    filterByLabels.TestMethod
        await client.post
            transactionCandidate '1' 
                map 'mood' 'bar'

        await client.post
            transactionCandidate '2' 
                map 'mood' 'foo'
    
        result = await client.get 
            transactionQuery 
                tags = dictionary 'mood' 'foo'

        assertEqual 1 result.count
        assertEqual '2' result.items.0.id

    filterById.TestMethod
        await client.post
            transactionCandidate 'a'

        await client.post
            transactionCandidate 'b'

        await client.post
            transactionCandidate 'c'

        result = await client.get 
            transactionQuery ids:'a,b'

        assertEqual 2 result.count
        assertEqual 'a' result.items.0.id
        assertEqual 'b' result.items.1.id

Test PatchTransactionsShould:TestClass
    moveExpensesToOtherContextIfTargetExists
        a 
            callTo => factory.contexts.get
                userId = "45"
                contextsQuery 0 1 'target'
            returns = contextCollectionOf
                context '11' 'target' now

        a
            callTo => factory.contexts.deleteByName
                userId = "45"
                name = 'source'
            returns = context '99' 'source' now

        await client.patch
            transactionQuery
                tags = map 'context' 'source' 'user' '45'
            transactionChanges
                tags = map 'context' 'target'

        mustHappened
            => factory.expenses.patch
                user = "45"
                expensesQuery contextId:'99'
                expenseChanges contextId:'11'
    
    renameContextIfTargetDoesntExist async
        aCallTo 
            => factory.contexts.get
                userId = "46"
                contextsQuery 0 1 'target'
        -> .returns contextCollection 0 'x' array

        aCallTo
            => factory.contexts.patch
                userId = "45"
                contextQuery name:'source'
                contextChanges name:'target'


> a : A.CallTo(@callTo).Returns(@returns)
> mustHappened : A.CallTo(@).MustHaveHappened()
> print : Console.WriteLine(@)
> assertEqual : Assert.AreEqual(@1, @2)
> list_T : new List<T>() { @+, @++  }
> map : new Dictionary<@+.type, @++.type>() { { @+, @++ }, { @+++, @++++} }
```