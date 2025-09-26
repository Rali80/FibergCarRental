using FibergCarRental.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FibergCarRental.Repository
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> GetByEmailAndPasswordAsync(string email, string password);
        Task<Customer> GetByEmailAsync(string email);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}