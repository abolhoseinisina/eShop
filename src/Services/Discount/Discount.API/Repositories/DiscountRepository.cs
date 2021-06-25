using Dapper;
using Discount.API.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration configuration;

        public DiscountRepository(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var createResult = await connection.ExecuteAsync("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount });
            if (createResult == 0) return false;
            return true;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var deleteResult = await connection.ExecuteAsync(
                $"DELETE FROM Coupon WHERE ProductName = {productName}");
            if (deleteResult == 0) return false;
            return true;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>($"SELECT * FROM Coupon WHERE ProductName = '{productName}'");
            if (coupon == null) return new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Desc" };
            return coupon;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var updateResult = await connection.ExecuteAsync("UPDATE Coupon SET ProductName=@ProductName, Description = @Description, Amount = @Amount WHERE Id = @Id",
                            new { ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount, Id = coupon.Id });
            if (updateResult == 0) return false;
            return true;
        }
    }
}
