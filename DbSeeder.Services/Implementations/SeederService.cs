using System;
using System.Text.Json;
using System.Threading.Tasks;
using DbSeeder.Model.Models;
using DbSeeder.Services.Interfaces;

namespace DbSeeder.Services.Implementations
{
    public class SeederService : ISeederService
    {
        private readonly SeedDetails SeedDetail;


        public SeederService(SeedDetails seedDetail)
        {
            SeedDetail = seedDetail;
        }

        public async Task<SeedDetails> Seed()
        {
            // Validate SeedDetail
            if (SeedDetail.CheckMe())
            {
                Console.WriteLine("Invalid parameters found:");
                Console.WriteLine("Method:{0,20}\nSeparator:{1,20}\nEntries:{3,20}", SeedDetail.Method, SeedDetail.Separator, SeedDetail.SeedItems.Count);
                Environment.Exit(160);
            }

            // Trigger URL with each entry - and store response
            for (var i = 0; i < SeedDetail.SeedItems.Count; i++)
            {
                var seedItem = SeedDetail.SeedItems[i];
                if (!seedItem.CheckUrl())
                {
                    Console.WriteLine("Invalid URL for given line at below node - {0}", seedItem.Url);
                    seedItem.ResponseMessage = new System.Net.Http.HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }
                   // Serialize content into Json
                var jsonContent = JsonSerializer.SerializeToUtf8Bytes(seedItem.JsonParameters);
                // trigger endpoint with given content
                seedItem.ResponseMessage = await HttpClientService.SendRequestAsync(seedItem.Url, jsonContent, SeedDetail.Method);
            }

            return SeedDetail;
        }
    }
}
