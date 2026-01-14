using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public class ExpenseRequest : BaseRequest
{
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool ReceiptAttached { get; set; }

    public ExpenseRequest()
    {
        RequestType = RequestType.Expense;
    }
}