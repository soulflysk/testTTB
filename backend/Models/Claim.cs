using System.ComponentModel.DataAnnotations;

public class Claim
{
    public int ClaimId { get; set; }

    [Required]
    public string PolicyNo { get; set; }

    [Required]
    public string CustomerName { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ClaimAmount { get; set; }

    [Required]
    public string Status { get; set; } // Pending, Approved, Rejected
}