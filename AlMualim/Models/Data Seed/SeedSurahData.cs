using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AlMualim.Data;
using System;
using System.Collections.Generic;
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
                var list = new List<Surah>();
                list.Add(new Surah("Al-Fatihah", "The Opening"));
                list.Add(new Surah("Al-Baqarah", "The Cow"));
                list.Add(new Surah("Aali Imran", "The Family of Imran"));
                list.Add(new Surah("An-Nisa'", "The Women"));
                list.Add(new Surah("Al-Ma'idah", "The Table"));
                list.Add(new Surah("Al-An'am", "The Cattle"));
                list.Add(new Surah("Al-A'raf", "The Heights"));
                list.Add(new Surah("Al-Anfal", "The Spoils of War"));
                list.Add(new Surah("At-Taubah", "The Repentance"));
                list.Add(new Surah("Yunus", "Yunus"));
                list.Add(new Surah("Hud", "Hud"));
                list.Add(new Surah("Yusuf", "Yusuf"));
                list.Add(new Surah("Ar-Ra'd", "The Thunder"));
                list.Add(new Surah("Ibrahim", "Ibrahim"));
                list.Add(new Surah("Al-Hijr", "The Rocky Tract"));
                list.Add(new Surah("An-Nahl", "The Bees"));
                list.Add(new Surah("Al-Isra'", "The Night Journey"));
                list.Add(new Surah("Al-Kahf", "The Cave"));
                list.Add(new Surah("Maryam", "Maryam"));
                list.Add(new Surah("Ta-Ha", "Ta-Ha"));
                list.Add(new Surah("Al-Anbiya'", "The Prophets"));
                list.Add(new Surah("Al-Haj", "The Pilgrimage"));
                list.Add(new Surah("Al-Mu'minun", "The Believers"));
                list.Add(new Surah("An-Nur", "The Light"));
                list.Add(new Surah("Al-Furqan", "The Criterion"));
                list.Add(new Surah("Ash-Shu'ara'", "The Poets"));
                list.Add(new Surah("An-Naml", "The Ants"));
                list.Add(new Surah("Al-Qasas", "The Stories"));
                list.Add(new Surah("Al-Ankabut", "The Spider"));
                list.Add(new Surah("Ar-Rum", "The Romans"));
                list.Add(new Surah("Luqman", "Luqman"));
                list.Add(new Surah("As-Sajdah", "The Prostration"));
                list.Add(new Surah("Al-Ahzab", "The Combined Forces"));
                list.Add(new Surah("Saba'", "The Sabeans"));
                list.Add(new Surah("Al-Fatir", "The Originator"));
                list.Add(new Surah("Ya-Sin", "Ya-Sin"));
                list.Add(new Surah("As-Saffah", "Those Ranges in Ranks"));
                list.Add(new Surah("Sad", "Sad"));
                list.Add(new Surah("Az-Zumar", "The Groups"));
                list.Add(new Surah("Ghafar", "The Forgiver"));
                list.Add(new Surah("Fusilat", "Distinguished"));
                list.Add(new Surah("Ash-Shura", "The Consultation"));
                list.Add(new Surah("Az-Zukhruf", "The Gold"));
                list.Add(new Surah("Ad-Dukhan", "The Smoke"));
                list.Add(new Surah("Al-Jathiyah", "The Kneeling"));
                list.Add(new Surah("Al-Ahqaf", "The Vcontext.Surahey"));
                list.Add(new Surah("Muhammad", "Muhammad"));
                list.Add(new Surah("Al-Fat'h", "The Victory"));
                list.Add(new Surah("Al-Hujurat", "The Dwellings"));
                list.Add(new Surah("Qaf", "Qaf"));
                list.Add(new Surah("Adz-Dzariyah", "The Scatterers"));
                list.Add(new Surah("At-Tur", "The Mount"));
                list.Add(new Surah("An-Najm", "The Star"));
                list.Add(new Surah("Al-Qamar", "The Moon"));
                list.Add(new Surah("Ar-Rahman", "The Most Gracious"));
                list.Add(new Surah("Al-Waqi'ah", "The Event"));
                list.Add(new Surah("Al-Hadid", "The Iron"));
                list.Add(new Surah("Al-Mujadilah", "The Reasoning"));
                list.Add(new Surah("Al-Hashr", "The GaThering"));
                list.Add(new Surah("Al-Mumtahanah", "The Tested"));
                list.Add(new Surah("As-Saf", "The Row"));
                list.Add(new Surah("Al-Jum'ah", "Friday"));
                list.Add(new Surah("Al-Munafiqun", "The Hypocrites"));
                list.Add(new Surah("At-Taghabun", "The Loss & Gain"));
                list.Add(new Surah("At-Talaq", "The Divorce"));
                list.Add(new Surah("At-Tahrim", "The Prohibition"));
                list.Add(new Surah("Al-Mulk", "The Kingdom"));
                list.Add(new Surah("Al-Qalam", "The Pen"));
                list.Add(new Surah("Al-Haqqah", "The Inevitable"));
                list.Add(new Surah("Al-Ma'arij", "The Elevated Passages"));
                list.Add(new Surah("Nuh", "Nuh"));
                list.Add(new Surah("Al-Jinn", "The Jinn"));
                list.Add(new Surah("Al-Muzammil", "The Wrapped"));
                list.Add(new Surah("Al-Mudaththir", "The Cloaked"));
                list.Add(new Surah("Al-Qiyamah", "The Resurrection"));
                list.Add(new Surah("Al-Insan", "The Human"));
                list.Add(new Surah("Al-Mursalat", "Those Sent Forth"));
                list.Add(new Surah("An-Naba'", "The Great News"));
                list.Add(new Surah("An-Nazi'at", "Those Who Pull Out"));
                list.Add(new Surah("'Abasa", "He Frowned"));
                list.Add(new Surah("At-Takwir", "The Overthrowing"));
                list.Add(new Surah("Al-Infitar", "The Cleaving"));
                list.Add(new Surah("Al-Mutaffifin", "Those Who Deal in Fraud"));
                list.Add(new Surah("Al-Inshiqaq", "The Splitting Asunder"));
                list.Add(new Surah("Al-Buruj", "The Stars"));
                list.Add(new Surah("At-Tariq", "The Nightcomer"));
                list.Add(new Surah("Al-A'la", "The Most High"));
                list.Add(new Surah("Al-Ghashiyah", "The Overwhelming"));
                list.Add(new Surah("Al-Fajr", "The Dawn"));
                list.Add(new Surah("Al-Balad", "The City"));
                list.Add(new Surah("Ash-Shams", "The Sun"));
                list.Add(new Surah("Al-Layl", "The Night"));
                list.Add(new Surah("Adh-Dhuha", "The Forenoon"));
                list.Add(new Surah("Al-Inshirah", "The Opening Forth"));
                list.Add(new Surah("At-Tin", "The Fig"));
                list.Add(new Surah("Al-'Alaq", "The Clot"));
                list.Add(new Surah("Al-Qadar", "The Night of Decree"));
                list.Add(new Surah("Al-Bayinah", "The Proof"));
                list.Add(new Surah("Az-Zalzalah", "The Earthquake"));
                list.Add(new Surah("Al-'Adiyah", "The Runners"));
                list.Add(new Surah("Al-Qari'ah", "The Striking Hour"));
                list.Add(new Surah("At-Takathur", "The Piling Up"));
                list.Add(new Surah("Al-'Asr", "The Time"));
                list.Add(new Surah("Al-Humazah", "The Slanderer"));
                list.Add(new Surah("Al-Fil", "The Elephant"));
                list.Add(new Surah("Quraish", "Quraish"));
                list.Add(new Surah("Al-Ma'un", "The Assistance"));
                list.Add(new Surah("Al-Kauthar", "The River of Abundance"));
                list.Add(new Surah("Al-Kafirun", "The Disbelievers"));
                list.Add(new Surah("An-Nasr", "The Help"));
                list.Add(new Surah("Al-Masad", "The Palm Fiber"));
                list.Add(new Surah("Al-Ikhlas", "The Sincerity"));
                list.Add(new Surah("Al-Falaq", "The Daybreak"));
                list.Add(new Surah("An-Nas", "Mankind"));

                list.ForEach(s => {
                    context.Surah.Add(s);
                    context.SaveChanges();
                });
                context.SaveChanges();
            }
        }
    }
}