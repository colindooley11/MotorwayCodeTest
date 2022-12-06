using MotorwayPaymentsCodeTest.Domain.Models;

namespace OrderFraudCheck.UnitTests;

public static class TestData
{
    public static CustomerOrder DefaultCustomer(Guid customerGuid) =>
        new()
        {
            CustomerGuid = customerGuid,
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