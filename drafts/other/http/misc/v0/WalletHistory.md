```ruby
Distribution
    Decimal                     total
    IEnumerable_NamedCategory   namedCategories
    Others                      otherCategories

    searchAnimal => namedCategories.search @String => @.name
    toContextExpenses => contextExpenses total toDictionary

    toDictionary
        

distributionFrom
    total = @IEnumerable_Category.categories.sum => @.sum
    named = namedCategoriesExtractedFrom @categories +extra
    return distribution 
        total 
        named 
        others extra

distributionFrom
    categories = @IEnumerable_Category...toArray
    total = categories.sum => .sum

    named others = categories.fork
        *@Distribution...searchAnimal
        => namedCategory @Category @Animal
        => category @Category

    return distribution total named others
```