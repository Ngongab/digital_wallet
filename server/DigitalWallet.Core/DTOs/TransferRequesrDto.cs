public class TransferRequest
{
    public required string RecipientEmail { get; set; }
    public decimal Amount { get; set; }
    public required string Description { get; set; }
}