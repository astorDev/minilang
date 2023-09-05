```ruby
TransactionController
    IMongoCollection_Transaction collection
    IMongoCollection_Tag tagsCollection
    IContextsClient contexts
    IExpensesClient expenses

    post HttpPost async =>
        candidateTags = @FromBody.TransactionCandidate.candidate.Tags ?? empty
        settingTags = await tagsCollection.get
        missingTags = settingTags.exceptBy candidateTags.Keys => $.id
        missing = missingTags.map => keyValuePair<string, string> $.id $.defaultValue
        tagsToSave = candidateTags.union missing
        toPost = transaction candidate.Id (dictionary tagsToSave)
        
        return await collection.post toPost

    get HttpGet async =>
        transactions = await collection get @TransactionQuery.FromBody...tofilter
        return transactionCollection transaction.Count, transaction.ToArray

    patch HttpPatch async =>
        queryTags = @TransactionQuery.FromQuery...Tags
        changesTags = @TransactionChanges.FromBody...Tags

        user = queryTags.userTagName
        sourceContext = queryTags.contextTagName
        targetContext = changesTags.contextTagName

        contextsQuery = ContextQuery skip=0 limit=0 name=targetContext
        existing = await contexts.by userId contextsQuery
        if existing.Count > 0
            deleted = try
                await contexts.deleteByName userId targetContext
                with UnsuccessfulResponseException ure:
                    errorBody = ure.DeserializeBody~Nist.Errors.Error
                    if errorBody.reason == "ContextNotFound"
                        return
                    throw
```