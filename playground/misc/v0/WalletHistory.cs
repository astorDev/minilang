using Astor.Linq;
using Finrir.Expenses.Protocol;

namespace Finrir.Analytics.Domain;

public class Distribution
{
    public decimal Total { get; }
    
    public IEnumerable<NamedCategory> NamedCategories { get; }
    
    public Others OtherCategories { get; }

    private Distribution(decimal total, IEnumerable<NamedCategory> namedCategories, Others others)
    {
        this.Total = total;
        this.NamedCategories = namedCategories;
        this.OtherCategories = others;
    }

    public static Distribution From(IEnumerable<Category> categories)
    {
        var total = categories.Sum(c => c.Sum);
        var named = Domain.NamedCategories.Extracted(categories, out var extra);
        var others = new Others(extra);

        return new(total, named, others);
    }

    public static Distribution From(IEnumerable<Category> categories, Distribution globalDistribution)
    {
        var cats = categories as Category[] ?? categories.ToArray();
        var total = cats.Sum(c => c.Sum);

        var namedCategories = new List<NamedCategory>();
        var otherCategories = new List<Category>();
        
        foreach (var category in cats)
        {
            if (globalDistribution.TryGetAnimal(category.Name, out var animal))
            {
                namedCategories.Add(new(animal!.Value, category));
            }
            else
            {
                otherCategories.Add(category);
            }
        }

        return new(total, namedCategories, new(otherCategories));

    }

    public bool TryGetAnimal(string categoryName, out Animal? animal)
    {
        var c = this.NamedCategories.FirstOrDefault(c => c.Category.Name == categoryName);
        animal = c?.Animal;
        return c != null;
    }

    public ContextExpenses ToContextExpenses()
    {
        return new(this.Total, this.ToDictionary());
    }
    
    public Dictionary<Animal, CategorySector> ToDictionary(IEnumerable<Expense>? expenses = null)
    {
        var remaining = expenses?.ToArray();
        
        DistributionExpense[]? PopExpenses(string categoryName)
        {
            if (remaining != null)
            {
                var (picked, different) = remaining!.Fork(e => e.Category.Name == categoryName);
                remaining = different.ToArray();
                return picked.Select(p => p.ToDistributionExpense()).ToArray();
            }

            return null;
        }

        Dictionary<Animal, CategorySector> result = new();

        foreach (var namedCategory in this.NamedCategories)
        {
            var categoryExpenses = PopExpenses(namedCategory.Category.Name);
            
            result.Add(namedCategory.Animal, new(
                namedCategory.Category.Name,
                namedCategory.Category.Sum,
                namedCategory.Category.Sum / this.Total * 100,
                categoryExpenses));
        }

        if (this.OtherCategories.Any)
        {
            result.Add(Others.Totem, new(
                Others.Name, 
                this.OtherCategories.Sum,
                this.OtherCategories.Sum / this.Total * 100,
                remaining?.Select(e => e.ToDistributionExpense()).ToArray()
            ));
        }

        return result;
    }

    public Protocol.Models.Category Enrich(string category)
    {
        var namedCategory = this.NamedCategories.FirstOrDefault(c => c.Category.Name == category);
        if (namedCategory == null)
        {
            return new(category, Others.Totem);
        }
        
        return new(category, namedCategory.Animal);
    }
}