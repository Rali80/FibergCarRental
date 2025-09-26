using FibergCarRental.Data;
using FibergCarRental.Models;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibergCarRental.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CarRentalContext _context;

        public CustomerRepository(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer> GetByEmailAndPasswordAsync(string email, string password)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer != null && BCrypt.Net.BCrypt.Verify(password, customer.Password))
            {
                return customer;
            }
            return null;
        }

        public async Task<Customer> GetByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task AddAsync(Customer customer)
        {
            customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            if (!string.IsNullOrEmpty(customer.Password))
            {
                customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            }
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}