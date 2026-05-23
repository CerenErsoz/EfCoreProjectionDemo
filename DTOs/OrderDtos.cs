namespace EfCoreProjectionDemo.DTOs;

public class OrderListDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerFullName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderDetailDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public CustomerDto Customer { get; set; } = null!;
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;
}

public class CustomerDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
