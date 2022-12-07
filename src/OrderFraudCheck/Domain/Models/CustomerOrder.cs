namespace MotorwayPaymentsCodeTest.Domain.Models;

public class CustomerOrder
{
    public Guid CustomerGuid { get; set; }
    public decimal OrderAmount { get; set; }
    public Address? CustomerAddress { get; set; }
}