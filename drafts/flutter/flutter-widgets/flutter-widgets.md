```ruby
compile
    flutter

centeredColumnPage widget
    return scaffold
        body = centeredColumn
            children = @List_Widget.children
            padding = @EdgeInsetsGeomerty.padding

centeredColumn widget
    return center
        child = container
            padding = @EdgeInsetsGeometry.padding
            child = column
                children = @List_Widget.children
                mainAxisAlignment = MAIN_AXIS_CENTER
                crossAxisAlignment = CROSS_AXIS_CENTER

commode widget
    hasTopShelfItems = has @List_Widget.topShelfItems
    children = list_widget
        commodeBody
            hasTopShelfItems = hasTopShelfItems
            mindNavbar = @Bool.mindNavbar
            drawers = @List_Widget.drawers

    if hasTopShelfItems
        children.add commodeTopShelf
            children = @topShelfItem

    return align
        alignment = ALIGNMENT_BOTTOM_CENTER
        child = stack
            children = children

has
    if no @List_Widget.widgets
        return false
    
    return @widgets.isNotEmpty

commodeTopShelf widget
    return padding
        passing = COMMON_HORIZONTAL_PADDING
        child = row
            children = @List_Widget.children
            mainAxisAlignment = MAIN_AXIS_END

commodeBody
    margin = null
    possibleTopMargin = divide TOP_SHELFT_ITEM_SIZE 2
    topPadding = 15

    if (@Bool.hasTopShelf)
        margin = edgeInsetOfOnly top:possibleTopMargin
        topPadding = 30

    height = 300
    if (@Bool.mindNavBar)
        height = 400

    radius = circularRadius 30
    theme = themeof @context

    return container
        margin = margin
        padding = edgetInsetsOnly top:topPadding
        height = height
        width = 120000000
        decoration = boxDecoration
            borderRadius = borderRadiusWithOnly topLeft:radius topRight:radius
            boxShadow = list_BoxShadow
                boxShadow
                    color = theme.backgroundColor
                    offset = offset 0 -1
                    blurRadius = 10
            color = theme.scaffoldBackgroundColor
        child = singleChildScrollView
            child = column
                mainAxisAlignment = MAIN_AXIS_END
                children @List_Widget.drawyers



```