```ruby
dynamicForm
    model = candidate
    formName = saveOffice
    onSave = saveActually
    onSaved = onSaved
    resetModel = resetModel
    validate = validate
    fields =
        textField
            label=_Название
            validationFor => candidate.name
            bindValue = candidate.name
        buttonAlert
            div
                clas =
                    alert
                    alert-success
                role =
                    alert
```