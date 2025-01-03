using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.EntityFrameworkCore;

namespace Elearning_Test.Services
{
    public class LeconService : ILeconService
    {
        private readonly ApplicationDbContext _context;

        public LeconService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Lecon> CreateLeconAsync(Lecon lecon)
        {
            _context.Lecons.Add(lecon);
            await _context.SaveChangesAsync();
            return lecon;
        }

        public async Task UpdateLeconAsync(Lecon lecon)
        {
            _context.Entry(lecon).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLeconAsync(int leconId)
        {
            var lecon = await _context.Lecons.FindAsync(leconId);
            if (lecon != null)
            {
                _context.Lecons.Remove(lecon);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Lecon> GetLeconByIdAsync(int leconId)
        {
            return await _context.Lecons
                .Include(l => l.Cours)
                .FirstOrDefaultAsync(l => l.Id == leconId);
        }

        public async Task<List<Lecon>> GetLeconsByCoursIdAsync(int coursId)
        {
            return await _context.Lecons
                .Where(l => l.CoursId == coursId)
                .ToListAsync();
        }
    }

    public interface ILeconService
    {
        Task<Lecon> CreateLeconAsync(Lecon lecon);
        Task UpdateLeconAsync(Lecon lecon);
        Task DeleteLeconAsync(int leconId);
        Task<Lecon> GetLeconByIdAsync(int leconId);
        Task<List<Lecon>> GetLeconsByCoursIdAsync(int coursId);
    }
}
