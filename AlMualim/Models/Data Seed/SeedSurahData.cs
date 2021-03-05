using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AlMualim.Data;
using System;
using System.Linq;

namespace AlMualim.Models
{
    public static class SeedSurahData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AlMualimDbContext(serviceProvider.GetRequiredService<DbContextOptions<AlMualimDbContext>>()))
            {
                // Delete existing data
                context.Database.ExecuteSqlRaw("TRUNCATE TABLE Surah");
                
                // Populate new data
                context.Surah.Add(new Surah("Al-Fatihah", "The Opening"));
                context.Surah.Add(new Surah("Al-Baqarah", "The Cow"));
                context.Surah.Add(new Surah("Aali Imran", "The Family of Imran"));
                context.Surah.Add(new Surah("An-Nisa'", "The Women"));
                context.Surah.Add(new Surah("Al-Ma'idah", "The Table"));
                context.Surah.Add(new Surah("Al-An'am", "The Cattle"));
                context.Surah.Add(new Surah("Al-A'raf", "The Heights"));
                context.Surah.Add(new Surah("Al-Anfal", "The Spoils of War"));
                context.Surah.Add(new Surah("At-Taubah", "The Repentance"));
                context.Surah.Add(new Surah("Yunus", "Yunus"));
                context.Surah.Add(new Surah("Hud", "Hud"));
                context.Surah.Add(new Surah("Yusuf", "Yusuf"));
                context.Surah.Add(new Surah("Ar-Ra'd", "The Thunder"));
                context.Surah.Add(new Surah("Ibrahim", "Ibrahim"));
                context.Surah.Add(new Surah("Al-Hijr", "The Rocky Tract"));
                context.Surah.Add(new Surah("An-Nahl", "The Bees"));
                context.Surah.Add(new Surah("Al-Isra'", "The Night Journey"));
                context.Surah.Add(new Surah("Al-Kahf", "The Cave"));
                context.Surah.Add(new Surah("Maryam", "Maryam"));
                context.Surah.Add(new Surah("Ta-Ha", "Ta-Ha"));
                context.Surah.Add(new Surah("Al-Anbiya'", "The Prophets"));
                context.Surah.Add(new Surah("Al-Haj", "The Pilgrimage"));
                context.Surah.Add(new Surah("Al-Mu'minun", "The Believers"));
                context.Surah.Add(new Surah("An-Nur", "The Light"));
                context.Surah.Add(new Surah("Al-Furqan", "The Criterion"));
                context.Surah.Add(new Surah("Ash-Shu'ara'", "The Poets"));
                context.Surah.Add(new Surah("An-Naml", "The Ants"));
                context.Surah.Add(new Surah("Al-Qasas", "The Stories"));
                context.Surah.Add(new Surah("Al-Ankabut", "The Spider"));
                context.Surah.Add(new Surah("Ar-Rum", "The Romans"));
                context.Surah.Add(new Surah("Luqman", "Luqman"));
                context.Surah.Add(new Surah("As-Sajdah", "The Prostration"));
                context.Surah.Add(new Surah("Al-Ahzab", "The Combined Forces"));
                context.Surah.Add(new Surah("Saba'", "The Sabeans"));
                context.Surah.Add(new Surah("Al-Fatir", "The Originator"));
                context.Surah.Add(new Surah("Ya-Sin", "Ya-Sin"));
                context.Surah.Add(new Surah("As-Saffah", "Those Ranges in Ranks"));
                context.Surah.Add(new Surah("Sad", "Sad"));
                context.Surah.Add(new Surah("Az-Zumar", "The Groups"));
                context.Surah.Add(new Surah("Ghafar", "The Forgiver"));
                context.Surah.Add(new Surah("Fusilat", "Distinguished"));
                context.Surah.Add(new Surah("Ash-Shura", "The Consultation"));
                context.Surah.Add(new Surah("Az-Zukhruf", "The Gold"));
                context.Surah.Add(new Surah("Ad-Dukhan", "The Smoke"));
                context.Surah.Add(new Surah("Al-Jathiyah", "The Kneeling"));
                context.Surah.Add(new Surah("Al-Ahqaf", "The Vcontext.Surahey"));
                context.Surah.Add(new Surah("Muhammad", "Muhammad"));
                context.Surah.Add(new Surah("Al-Fat'h", "The Victory"));
                context.Surah.Add(new Surah("Al-Hujurat", "The Dwellings"));
                context.Surah.Add(new Surah("Qaf", "Qaf"));
                context.Surah.Add(new Surah("Adz-Dzariyah", "The Scatterers"));
                context.Surah.Add(new Surah("At-Tur", "The Mount"));
                context.Surah.Add(new Surah("An-Najm", "The Star"));
                context.Surah.Add(new Surah("Al-Qamar", "The Moon"));
                context.Surah.Add(new Surah("Ar-Rahman", "The Most Gracious"));
                context.Surah.Add(new Surah("Al-Waqi'ah", "The Event"));
                context.Surah.Add(new Surah("Al-Hadid", "The Iron"));
                context.Surah.Add(new Surah("Al-Mujadilah", "The Reasoning"));
                context.Surah.Add(new Surah("Al-Hashr", "The GaThering"));
                context.Surah.Add(new Surah("Al-Mumtahanah", "The Tested"));
                context.Surah.Add(new Surah("As-Saf", "The Row"));
                context.Surah.Add(new Surah("Al-Jum'ah", "Friday"));
                context.Surah.Add(new Surah("Al-Munafiqun", "The Hypocrites"));
                context.Surah.Add(new Surah("At-Taghabun", "The Loss & Gain"));
                context.Surah.Add(new Surah("At-Talaq", "The Divorce"));
                context.Surah.Add(new Surah("At-Tahrim", "The Prohibition"));
                context.Surah.Add(new Surah("Al-Mulk", "The Kingdom"));
                context.Surah.Add(new Surah("Al-Qalam", "The Pen"));
                context.Surah.Add(new Surah("Al-Haqqah", "The Inevitable"));
                context.Surah.Add(new Surah("Al-Ma'arij", "The Elevated Passages"));
                context.Surah.Add(new Surah("Nuh", "Nuh"));
                context.Surah.Add(new Surah("Al-Jinn", "The Jinn"));
                context.Surah.Add(new Surah("Al-Muzammil", "The Wrapped"));
                context.Surah.Add(new Surah("Al-Mudaththir", "The Cloaked"));
                context.Surah.Add(new Surah("Al-Qiyamah", "The Resurrection"));
                context.Surah.Add(new Surah("Al-Insan", "The Human"));
                context.Surah.Add(new Surah("Al-Mursalat", "Those Sent Forth"));
                context.Surah.Add(new Surah("An-Naba'", "The Great News"));
                context.Surah.Add(new Surah("An-Nazi'at", "Those Who Pull Out"));
                context.Surah.Add(new Surah("'Abasa", "He Frowned"));
                context.Surah.Add(new Surah("At-Takwir", "The Overthrowing"));
                context.Surah.Add(new Surah("Al-Infitar", "The Cleaving"));
                context.Surah.Add(new Surah("Al-Mutaffifin", "Those Who Deal in Fraud"));
                context.Surah.Add(new Surah("Al-Inshiqaq", "The Splitting Asunder"));
                context.Surah.Add(new Surah("Al-Buruj", "The Stars"));
                context.Surah.Add(new Surah("At-Tariq", "The Nightcomer"));
                context.Surah.Add(new Surah("Al-A'la", "The Most High"));
                context.Surah.Add(new Surah("Al-Ghashiyah", "The Overwhelming"));
                context.Surah.Add(new Surah("Al-Fajr", "The Dawn"));
                context.Surah.Add(new Surah("Al-Balad", "The City"));
                context.Surah.Add(new Surah("Ash-Shams", "The Sun"));
                context.Surah.Add(new Surah("Al-Layl", "The Night"));
                context.Surah.Add(new Surah("Adh-Dhuha", "The Forenoon"));
                context.Surah.Add(new Surah("Al-Inshirah", "The Opening Forth"));
                context.Surah.Add(new Surah("At-Tin", "The Fig"));
                context.Surah.Add(new Surah("Al-'Alaq", "The Clot"));
                context.Surah.Add(new Surah("Al-Qadar", "The Night of Decree"));
                context.Surah.Add(new Surah("Al-Bayinah", "The Proof"));
                context.Surah.Add(new Surah("Az-Zalzalah", "The Earthquake"));
                context.Surah.Add(new Surah("Al-'Adiyah", "The Runners"));
                context.Surah.Add(new Surah("Al-Qari'ah", "The Striking Hour"));
                context.Surah.Add(new Surah("At-Takathur", "The Piling Up"));
                context.Surah.Add(new Surah("Al-'Asr", "The Time"));
                context.Surah.Add(new Surah("Al-Humazah", "The Slanderer"));
                context.Surah.Add(new Surah("Al-Fil", "The Elephant"));
                context.Surah.Add(new Surah("Quraish", "Quraish"));
                context.Surah.Add(new Surah("Al-Ma'un", "The Assistance"));
                context.Surah.Add(new Surah("Al-Kauthar", "The River of Abundance"));
                context.Surah.Add(new Surah("Al-Kafirun", "The Disbelievers"));
                context.Surah.Add(new Surah("An-Nasr", "The Help"));
                context.Surah.Add(new Surah("Al-Masad", "The Palm Fiber"));
                context.Surah.Add(new Surah("Al-Ikhlas", "The Sincerity"));
                context.Surah.Add(new Surah("Al-Falaq", "The Daybreak"));
                context.Surah.Add(new Surah("An-Nas", "Mankind"));

                context.SaveChanges();
            }
        }
    }
}