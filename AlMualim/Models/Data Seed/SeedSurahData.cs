using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AlMualim.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace AlMualim.Models
{
    public static class SeedSurahData
    {
        public static void Initialize(IServiceProvider serviceProdiver)
        {
            var config = serviceProdiver.GetService<IConfiguration>();
            var fileLocation = config.GetValue<string>("SurahDataSeedFile");

            // Get raw data
            SurahJsonCollection input;
            using (StreamReader file = File.OpenText(fileLocation))
            {
                JsonSerializer serializer = new JsonSerializer();
                input = (SurahJsonCollection)serializer.Deserialize(file, typeof(SurahJsonCollection));
            }

            // Convert to model
            var converted = input.ToSurah();
            Surah.AllSurah = converted;
        }
    }
}