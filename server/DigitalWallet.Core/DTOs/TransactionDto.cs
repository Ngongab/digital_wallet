public class TransactionDto
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public required string Type { get; set; }
    public required string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string RecipientEmail { get; set; }
    public required string SenderEmail { get; set; }
}