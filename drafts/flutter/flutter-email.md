```python
emailForm widget
    theme = themeOf @context

    return container
        padding = edgeInsetsAll 20
        decoration = boxDecoration
            borderRadius = circularBorderRadius 20
            boxShadow = list
                buttonShadow @context
            border = allBorder
                color = LAMP_PURPLE_END
                width = 3
            color = theme.colorScheme.surface
        child = emailFormContent

emailFormContext widget
    theme = themeOf @context

    return column
        crossAxisAlignment = ALIGN_CROSS_AXIS_TO_START
        children = list
            row
                children = list
                    icon
                        ICON_EMAIL
                        color = theme.colorScheme.onSurface
                    sizedBox width:10
                    expanded
                        child = textField
                            keyboardType = KEYBOARD_EMAIL
                            onChanged => 
                                setState 
                                    => @String.input = value
                            decoration = inputDecoration
                                hintText = 'Email
            sizedBox height:20
            actionButtonChip
                capture = 'Get_in
                isActive = isEmail @input
                onClick => move @context
                    to => processEmail @input

emailUnexpectedErrorPage widget
    return unexpectedErrorPage
        actions = list
            emailForm
            blocksSpace
            telegramLoginActionButton
```