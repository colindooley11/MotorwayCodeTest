using MotorwayPaymentsCodeTest.Domain.Models;

namespace OrderFraudCheck.UnitTests;

public class FraudTestsBase
{
    protected CustomerOrder _customerOrder = null!;
    protected void A_Customer_Order(Guid customerId)
    {
        _customerOrder = new CustomerOrder
        {
            CustomerGuid = customerId,
            OrderAmount = 1500.55M,
            CustomerAddress = new Address
            {
                FirstName = "John",
                LastName = "Doe",
                Line1 = "10 High Street",
                City = "London",
                Region = "Greater London",
                PostalCode = "W1T 3HE"
            }
        };
    }

    protected void A_Customer_Order(Guid customerId, decimal orderAmount)
    {
        _customerOrder =  new CustomerOrder
        {
            CustomerGuid = customerId,
            OrderAmount = orderAmount,
            CustomerAddress = new Address
            {
                FirstName = "John",
                LastName = "Doe",
                Line1 = "10 High Street",
                City = "London",
                Region = "Greater London",
                PostalCode = "W1T 3HE"
            }
        };
    }
}