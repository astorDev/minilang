```ruby
#TestClass
Test ExpensesReportContextShould
    changeAdditionContext async TestMethod
        arrangeServices 403 
        -> toHaveToggles context:true 
        -> toHaveContexts "Europe" "Asia"

        await start 4031
        .send REPORT_MESSAGE
        .send EXPENSES_REPORT_SWITCH_CONTEXT_CALLBACK
        .send expensesReportSetContext "Asia" -> .Data
        .send ADDITION_MESSAGE
        .assetReceived expenseAdditionReply "Asia"
        .run
```