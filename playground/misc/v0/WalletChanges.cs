using Finrir.Contexts.Protocol;
using Finrir.Transfers.Protocol;
using Income = Finrir.Incomes.Protocol.Income;

namespace Finrir.Analytics.Domain;

public class WalletChanges
{
    public readonly Transfer[] transfers;
    private readonly Expense[] expenses;
    private readonly Income[] incomes;
    
    public WalletChanges(IEnumerable<Expense> expenses, IEnumerable<Income> incomes, IEnumerable<Transfer> transfers)
    {
        this.transfers = transfers.ToArray();
        this.expenses = expenses.ToArray();
        this.incomes = incomes.ToArray();
    }

    public bool AreEmpty => !this.expenses.Any() && !this.incomes.Any() && !this.transfers.Any();

    public WalletExpenses? GetWalletExpenses(Distribution globalDistribution, Context[] contexts)
    {
        if (!this.expenses.Any())
        {
            return null;
        }

        var contextExpenses = new Dictionary<string, ContextExpenses>();
        foreach (var expensesInContext in this.expenses.GroupBy(e => e.ContextId))
        {
            var categories = CategoriesCollection.From(expensesInContext);
            var distribution = Distribution.From(categories, globalDistribution);
            var contextName = contexts.Single(c => c.Id == expensesInContext.Key).Name;
            
            contextExpenses.Add(contextName, distribution.ToContextExpenses());
        }

        var total = contextExpenses.Values.Sum(e => e.Total);
        
        var allFlatDistributions = contextExpenses
            .Values
            .SelectMany(v => v.Distribution);
        
        var summedDistributions = allFlatDistributions
            .GroupBy(d => d.Key)
            .Select(g => new 
            {
                Animal = g.Key,
                g.First().Value.Name,
                Sum = g.Sum(e => e.Value.Sum)
            });

        var allDistribution = summedDistributions.ToDictionary(
            d => d.Animal,
            d => new CategorySector(d.Name, d.Sum, d.Sum / total * 100)
        );


        return new(total, allDistribution, contextExpenses);
    }

    public IncomeCollection? GetIncomeCollection()
    {
        if (!this.incomes.Any())
        {
            return null;
        }
        
        return new(
            this.incomes.Sum(s => s.Sum),
            this.incomes.Select(i => new Analytics.Protocol.Models.Income(i.Sum, i.Title)).ToArray()
        );
    }

    public TransfersCollection? GetTransfersCollection()
    {
        IEnumerable<Protocol.Models.Transfer> GetTransfers()
        {
            foreach (var transfersGroup in this.transfers.GroupBy(t => t.Protocol.Counterparty))
            {
                var inSum = transfersGroup.Where(t => t.Protocol.Direction == Directions.In).Sum(t => t.Protocol.Sum);
                var outSum = transfersGroup.Where(t => t.Protocol.Direction == Directions.Out).Sum(t => t.Protocol.Sum);

                var sum = inSum - outSum;
                if (sum == Decimal.Zero) continue;

                var direction = sum < Decimal.Zero ? Directions.Out : Directions.In;
                yield return new(Math.Abs(sum), direction, null, transfersGroup.Key);
            }
        }

        var resultTransfers = GetTransfers().ToArray();
        
        if (!resultTransfers.Any()) return null;
        return new(resultTransfers.Length, resultTransfers);
    }
}