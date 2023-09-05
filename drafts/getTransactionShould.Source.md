```csharp
namespace Finrir.Transactions.Tests;

[TestClass]
public class GetTransactionsShould : Test {
    [TestMethod]
    public async Task ReturnSavedTransactions() {
        await this.Client.PostTransaction(new ("1", new ()));
        await this.Client.PostTransaction(new ("2", new ()));

        var result = await this.Client.GetTransactions(new ());

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("1", result.Items[0].Id);
        Assert.AreEqual("2", result.Items[1].Id);
    }

    [TestMethod]
    public async Task FilterByLabels() {
        await this.Client.PostTransaction(new ("1", new() { { "mood", "bar" } }));
        await this.Client.PostTransaction(new ("2", new() { { "mood", "foo" } }));

        var result = await this.Client.GetTransactions(new (new () { { "mood", "foo" } }));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("2", result.Items[0].Id);
    }

    [TestMethod]
    public async Task FilterById() {
        await this.Client.PostTransaction(new ("a", new ()));
        await this.Client.PostTransaction(new ("b", new ()));
        await this.Client.PostTransaction(new ("c", new ()));

        var result = await this.Client.GetTransactions(new (null, "b"));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("b", result.Items[0].Id);
    }

    [TestMethod]
    public async Task FilterByIds() {
        await this.Client.PostTransaction(new ("a", new ()));
        await this.Client.PostTransaction(new ("b", new ()));
        await this.Client.PostTransaction(new ("c", new ()));

        var result = await this.Client.GetTransactions(new (null, "a,b"));

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("a", result.Items[0].Id);
        Assert.AreEqual("b", result.Items[1].Id);
    }

    [TestMethod]
    public async Task FilterByLabelsAndIds() {
        await this.Client.PostTransaction(new ("a", new (new Dictionary<string, string> { { "mood", "bar" } })));
        await this.Client.PostTransaction(new("b"));
        await this.Client.PostTransaction(new ("c", new (new Dictionary<string, string> { { "mood", "foo" } })));

        var result = await this.Client.GetTransactions(new (new () { { "mood", "foo" } }, "b,c"));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("c", result.Items[0].Id);
    }
}

[TestClass]
public class PatchTransactionsShould : Test
{
    [TestMethod]
    public async Task MoveExpensesToOtherContextIfTargetExists()
    {
        A.CallTo(() => this.Factory.Contexts.Get("45", new(0, 1, "target")))
            .Returns(new ContextCollection(1, "x", new[]
            {
                new Context("11", "target", DateTime.Now)
            }));
        
        A.CallTo(() => this.Factory.Contexts.DeleteByName("45", "source")).Returns(new Context("99", "source", DateTime.Now));
        
        await this.Client.PatchTransactions(new(new()
            {
                { "context", "source" },
                { "user", "45" }
            }),
            new(new()
            {
                { "context", "target" }
            }));
        
        var expensesQuery = new UserExpensesQuery(ContextId: "99");
        var expensesChanges = new ExpenseChanges(ContextId: "11");
        A.CallTo(() => this.Factory.Expenses.PatchExpenses("45", expensesQuery, expensesChanges)).MustHaveHappened();
    }

    [TestMethod]
    public async Task RenameContextIfTargetDoesntExist()
    {
        A.CallTo(() => this.Factory.Contexts.Get("46", new(0, 1, "target"))).Returns(new ContextCollection(0, "x", Array.Empty<Context>()));

        await this.Client.PatchTransactions(new(new()
            {
                { "context", "source" },
                { "user", "46" }
            }),
            new(new()
            {
                { "context", "target" }
            }));

        A.CallTo(() => this.Factory.Contexts.PatchByName("46", "source", new("target"))).MustHaveHappened();
    }
}
```