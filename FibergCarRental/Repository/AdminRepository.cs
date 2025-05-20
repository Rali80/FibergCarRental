using FibergCarRental.Data;
using FibergCarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace FibergCarRental.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly CarRentalContext _context;

        public AdminRepository(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<List<Admin>> GetAllAsync()
        {
            return await _context.Admins.ToListAsync();
        }

        public async Task<Admin> GetByIdAsync(int id)
        {
            return await _context.Admins.FindAsync(id);
        }

        public async Task<Admin> GetByEmailAndPasswordAsync(string email, string password)
        {
            return await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email && a.Password == password);
        }

        public async Task AddAsync(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }
    }
}
