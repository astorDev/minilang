using Finrir.Bots.Telegram.Domain.Additions;
using Finrir.Bots.Telegram.Domain.Reports;

namespace Finrir.Bots.Telegram.Tests.Reports;

[TestClass]
public class ExpenseReportContextShould : Test
{
    [TestMethod]
    public async Task ChangeAdditionContext()
    {
        this.ArrangeServices(403)
            .ToHaveToggles(context: true)
            .ToHaveContexts("Europe", "Asia");

        await this.Start(4031)
            .Send(ReportCommand.Message)
            .SendCallback(ExpensesReport.Contexts.BeginSwitch.Callback)
            .SendCallback(ExpensesReport.Contexts.Button(new("2", "Asia", DateTime.Now)).Data)
            .Send(AdditionCommand.Message)
            .AssertReceived(ExpenseAddition.Start.Reply("Asia"))
            .RunAsync();
    }
}