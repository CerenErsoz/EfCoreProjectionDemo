using AutoMapper;
using AutoMapper.QueryableExtensions;
using EfCoreProjectionDemo.Data;
using EfCoreProjectionDemo.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreProjectionDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(AppDbContext context, IMapper mapper, ILogger<OrdersController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// APPROACH A: Naive / Bad Practice
    /// Fetching full entities with Include, then mapping in memory.
    /// PROS: Easy to write, no AutoMapper complexity initially.
    /// CONS: Overfetching! We fetch columns we don't need (e.g., Customer Email, Product Description if it existed).
    /// </summary>
    [HttpGet("naive")]
    public async Task<ActionResult<IEnumerable<OrderListDto>>> GetNaive()
    {
        // We use Include to avoid N+1, but we are still fetching EVERY column from these tables.
        var ordersQuery = _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .AsNoTracking();

        _logger.LogInformation("Naive SQL:\n{Sql}", ordersQuery.ToQueryString());

        var orders = await ordersQuery.ToListAsync();
        
        // Mapping happens IN MEMORY after all data is fetched.
        var dtos = _mapper.Map<IEnumerable<OrderListDto>>(orders);

        return Ok(dtos);
    }

    /// <summary>
    /// APPROACH B: Manual Projection
    /// Using .Select() to project directly into DTOs at the database level.
    /// PROS: Optimized SQL, only fetches required columns.
    /// CONS: Maintenance nightmare. Every time DTO changes, you update this manual mapping.
    /// </summary>
    [HttpGet("manual-projection")]
    public async Task<ActionResult<IEnumerable<OrderListDto>>> GetManual()
    {
        var ordersQuery = _context.Orders
            .AsNoTracking()
            .Select(o => new OrderListDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                CustomerFullName = o.Customer.FirstName + " " + o.Customer.LastName,
                ProductCount = o.OrderItems.Sum(oi => oi.Quantity),
                TotalPrice = o.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
            });

        _logger.LogInformation("Manual Projection SQL:\n{Sql}", ordersQuery.ToQueryString());

        var dtos = await ordersQuery.ToListAsync();

        return Ok(dtos);
    }

    /// <summary>
    /// APPROACH C: Best Practice (AutoMapper ProjectTo)
    /// Using AutoMapper's ProjectTo<T>() directly on IQueryable.
    /// PROS: Optimized SQL (like manual), easy maintenance (mapping in Profile), clean controller.
    /// CONS: Requires understanding how AutoMapper translates expressions to SQL.
    /// </summary>
    [HttpGet("projectto")]
    public async Task<ActionResult<IEnumerable<OrderListDto>>> GetProjectTo()
    {
        // ProjectTo uses the configuration in OrderProfile to generate the .Select() statement automatically.
        var ordersQuery = _context.Orders
            .AsNoTracking()
            .ProjectTo<OrderListDto>(_mapper.ConfigurationProvider);

        _logger.LogInformation("ProjectTo SQL:\n{Sql}", ordersQuery.ToQueryString());

        var dtos = await ordersQuery.ToListAsync();

        return Ok(dtos);
    }

    /// <summary>
    /// Detailed view example using ProjectTo
    /// Demonstrates nested projection and complex aggregations.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrder(int id)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .ProjectTo<OrderDetailDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (order == null) return NotFound();

        return Ok(order);
    }
}
