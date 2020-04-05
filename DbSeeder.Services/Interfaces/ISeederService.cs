using DbSeeder.Model.Models;
using System.Threading.Tasks;

namespace DbSeeder.Services.Interfaces
{
    public interface ISeederService
    {
        public Task<SeedDetails> Seed();

    }
}
