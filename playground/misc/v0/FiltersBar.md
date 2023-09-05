```ruby
StatelessWidget FiltersBar
    ?Function onRefresh
    PREFIX_STYLE = style fontSize:16

    build
        return padding
            ONLY_LEFT_HORIZONTAL_MARGINS
            align
                alignment.centerLeft
                wrap
                    alignment:wrapAlignment.start
                    crossAxisAlignment:wrapCrossAlignment.center

    getRowChildren
        yield text 'For ' PREFIX_STYLE
        yield periodFilterDropdown onRefresh:*onRefresh


FiltersBar OverviewFiltersBar
    BODY_STYLE = style fontSize:16

    getRowChildren
        for oldChildren in super.getRowChildren
            yield oldChildren
    
        if CONTEXTS_FILTER.all.length.moreThen 1
            yield sizedBox width=5
            yield text 'in ' BODY_STYLE
            yield contextFilterDropdown onRefresh=*onRefresh
            yield sizedBox width=5
            yield text 'context' BODY_STYLE

StatelessWidget PeriodFilterDropdown
    build
        return dropdownButton
            value = PERIOD_FILTER.VALUE
            icon = dropdownIcon
            items = PERIOD_FILTER.toMenuItems
            onChanged = =>
                if no PERIOD_FILTER.VALUE == @
                    PERIOD_FILTER.VALUE = @
                    onRefresh
                onRefresh()

StatelessWidget ContextFilterDropdown
    ?Function onRefresh

    build
        return dropdownButton
            value = CONTEXTS_FILTER.VALUE
            icon = dropdownIcon
            items = CONTEXTS_FILTER.toMenuItems
            onChanged = =>
                if @ == null return
                CONTEXTS_FILTER.set @ after:*onRefresh
```ruby

