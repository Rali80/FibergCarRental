using FibergCarRental.Models;

namespace FibergCarRental.Repository
{
    public interface IAdminRepository
    {
        Task<List<Admin>> GetAllAsync();
        Task<Admin> GetByIdAsync(int id);
        Task<Admin> GetByEmailAndPasswordAsync(string email, string password);
        Task AddAsync(Admin admin);
        Task UpdateAsync(Admin admin);
        Task DeleteAsync(int id);
    }
}
