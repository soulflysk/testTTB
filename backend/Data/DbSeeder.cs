using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace DOTNETWEBAPI_DEV.Data
{
    public class DbSeeder
    {
        public static void SeedData(DbContextClass context)
        {
            context.Database.EnsureCreated();

            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product { ProductCode = "P001", ProductName = "Laptop Dell XPS 15", UnitPrice = 45000.00m },
                    new Product { ProductCode = "P002", ProductName = "iPhone 15 Pro", UnitPrice = 35000.00m },
                    new Product { ProductCode = "P003", ProductName = "iPad Air", UnitPrice = 18000.00m },
                    new Product { ProductCode = "P004", ProductName = "Samsung Galaxy Watch", UnitPrice = 8500.00m },
                    new Product { ProductCode = "P005", ProductName = "Sony WH-1000XM5 Headphones", UnitPrice = 12000.00m },
                    new Product { ProductCode = "P006", ProductName = "MacBook Air M2", UnitPrice = 42000.00m },
                    new Product { ProductCode = "P007", ProductName = "LG 27\" 4K Monitor", UnitPrice = 9500.00m },
                    new Product { ProductCode = "P008", ProductName = "Logitech MX Master 3 Mouse", UnitPrice = 3500.00m },
                    new Product { ProductCode = "P009", ProductName = "Mechanical Keyboard RGB", UnitPrice = 2800.00m },
                    new Product { ProductCode = "P010", ProductName = "USB-C Hub 7-in-1", UnitPrice = 1200.00m }
                };

                context.Products.AddRange(products);
                context.SaveChanges();

                var stocks = new List<Stock>
                {
                    new Stock { ProductCode = "P001", Quantity = 15 },
                    new Stock { ProductCode = "P002", Quantity = 25 },
                    new Stock { ProductCode = "P003", Quantity = 20 },
                    new Stock { ProductCode = "P004", Quantity = 30 },
                    new Stock { ProductCode = "P005", Quantity = 18 },
                    new Stock { ProductCode = "P006", Quantity = 12 },
                    new Stock { ProductCode = "P007", Quantity = 8 },
                    new Stock { ProductCode = "P008", Quantity = 35 },
                    new Stock { ProductCode = "P009", Quantity = 22 },
                    new Stock { ProductCode = "P010", Quantity = 40 }
                };

                context.Stocks.AddRange(stocks);
                context.SaveChanges();
            }
        }
    }
}
