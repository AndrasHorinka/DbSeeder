using System.Text.Json;
using System.Threading.Tasks;
using DbSeeder.Model.Models;

namespace DbSeeder.Services.Implementations
{
    public class SeederService
    {
        private readonly SeedDetails SeedDetail;


        public SeederService(SeedDetails seedDetail)
        {
            SeedDetail = seedDetail;
        }

        public async Task<SeedDetails> Seed()
        {
            // Trigger URL with each entry - and store response
            for (var i = 0; i < SeedDetail.SeedItems.Count; i++)
            {
                var seedItem = SeedDetail.SeedItems[i];
                // Serialize content into Json
                var jsonContent = JsonSerializer.SerializeToUtf8Bytes(seedItem.JsonParameters);
                // trigger endpoint with given content
                seedItem.ResponseMessage = await HttpClientService.SendRequestAsync(seedItem.Url, jsonContent, SeedDetail.Method);
            }

            return SeedDetail;
        }
    }
}
