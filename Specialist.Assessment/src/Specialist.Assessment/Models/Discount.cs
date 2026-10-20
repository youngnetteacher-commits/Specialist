namespace Specialist.Assessment.Models;

public class CourseDiscountInfo
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal OriginalPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal FinalPrice { get; set; }
    public string DiscountReason { get; set; } = string.Empty;
}

public class CartSummary
{
    public decimal TotalOriginalPrice { get; set; }
    public decimal TotalFinalPrice { get; set; }
    public decimal TotalSaved { get; set; }
    public List<CourseDiscountInfo> Courses { get; set; } = new();
}
