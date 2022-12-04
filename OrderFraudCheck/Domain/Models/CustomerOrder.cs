namespace MotorwayPaymentsCodeTest.Domain.Models;

public class CustomerOrder
{
    public Guid CustomerGuid { get; set; }
    public decimal OrderAmount { get; set; }
    public Address CustomerAddress { get; set; }
}

public class Address
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Line1 { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string PostalCode { get; set; }
}