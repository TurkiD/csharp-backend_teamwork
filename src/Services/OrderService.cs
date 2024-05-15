using AutoMapper;
using Dtos.Order;
using Dtos.OrderDto;
using Dtos.Pagination;
using Dtos.Product;
using Dtos.User;
using EntityFramework;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private readonly AppDBContext _appDbContext;
    private readonly IMapper _mapper;
    public OrderService(AppDBContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }

    public async Task<PaginationResult<OrderDto>> GetAllOrdersService(int pageNumber, int pageSize)
    {
        var totalCount = _appDbContext.Orders.Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var page = await _appDbContext.Orders
            // .Include(o => o.User)
            // .Include(o => o.Products)
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.OrderId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => _mapper.Map<OrderDto>(p))
            .ToListAsync();

        return new PaginationResult<OrderDto>
        {
            Items = page,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<List<Order>> GetMyOrders(Guid userId)
    {
        return await _appDbContext.Orders.Include(o => o.Products).Where(o => o.UserId == userId).ToListAsync();
    }

    public async Task<Order?> GetOrderById(Guid orderId)
    {
        return await _appDbContext.Orders.Include(o => o.Products).Include(o => o.User).FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Guid> CreateOrderService(Guid userId, PaymentMethod paymentMethod)
    {
        // Create record
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Pending,
            Payment = paymentMethod,
            Amount = 0,
        };

        // Add the record to the context
        await _appDbContext.Orders.AddAsync(order);
        // Save to database
        await _appDbContext.SaveChangesAsync();

        return order.OrderId;
    }

    public async Task AddProductToOrder(Guid orderId, Guid productId)
    {
        var order = await _appDbContext.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.OrderId == orderId);
        var product = await _appDbContext.Products.FindAsync(productId);

        if (order != null && product != null)
        {
            if (product.Quantity == 0)
            {
                throw new InvalidOperationException("This product is unavailable");
            }

            order.Products.Add(product);
            product.Quantity--;
            order.Amount = (double)product.Price;
            await _appDbContext.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("This Product has already added to the Order");
        }
    }

    public async Task<bool> UpdateOrderService(Guid orderId, UpdateOrderDto updateOrder)
    {
        var existingOrder = _appDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
        if (existingOrder != null)
        {
            existingOrder.Status = updateOrder.Status;
            existingOrder.Payment = updateOrder.Payment;

            // Add the record to the context
            _appDbContext.Orders.Update(existingOrder);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateOrderService(Guid userId, Guid orderId, UpdateOrderDto updateOrder)
    {
        var existingOrder = _appDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
        if (existingOrder != null)
        {
            existingOrder.Payment = updateOrder.Payment;

            // Add the record to the context
            _appDbContext.Orders.Update(existingOrder);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteOrderService(Guid userId, Guid orderId)
    {
        var orderToRemove = _appDbContext.Orders.FirstOrDefault(order => order.OrderId == orderId && order.UserId == userId);
        if (orderToRemove != null)
        {
            // Use context to remove
            _appDbContext.Orders.Remove(orderToRemove);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteOrderService(Guid orderId)
    {
        var orderToRemove = _appDbContext.Orders.FirstOrDefault(order => order.OrderId == orderId);
        if (orderToRemove != null)
        {
            // Use context to remove
            _appDbContext.Orders.Remove(orderToRemove);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
