using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using AVSSecurityAuditor.Data;
using AVSSecurityAuditor.Models;
using Microsoft.EntityFrameworkCore;

namespace AVSSecurityAuditor.Services
{
    public class CsvImportRecord
    {
        public string? chapter_id { get; set; }
        public string? chapter_name { get; set; }
        public string? section_id { get; set; }
        public string? section_name { get; set; }
        public string? req_id { get; set; }
        public string? req_description { get; set; }
        public string? level1 { get; set; }
        public string? level2 { get; set; }
        public string? level3 { get; set; }
        public string? cwe { get; set; }
        public string? nist { get; set; }
    }

    public class CsvImportService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CsvImportService> _logger;

        public CsvImportService(AppDbContext context, ILogger<CsvImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(int chapters, int requirements)> ImportAsync(Stream csvStream)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null
            };

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<CsvImportRecord>().ToList();

            var chapterMap = new Dictionary<string, AsvsChapter>();
            int reqCount = 0;

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.req_id)) continue;

                var chapterKey = record.chapter_id ?? "0";
                if (!chapterMap.ContainsKey(chapterKey))
                {
                    var existing = await _context.AsvsChapters
                        .FirstOrDefaultAsync(c => c.ChapterNumber.ToString() == chapterKey);
                    if (existing == null)
                    {
                        existing = new AsvsChapter
                        {
                            ChapterNumber = int.TryParse(chapterKey, out var n) ? n : 0,
                            Name = record.chapter_name ?? $"Chapter {chapterKey}",
                            Description = record.section_name ?? string.Empty
                        };
                        await _context.AsvsChapters.AddAsync(existing);
                        await _context.SaveChangesAsync();
                    }
                    chapterMap[chapterKey] = existing;
                }

                var chapter = chapterMap[chapterKey];
                var reqExists = await _context.AsvsRequirements
                    .AnyAsync(r => r.RequirementId == record.req_id);

                if (!reqExists)
                {
                    var req = new AsvsRequirement
                    {
                        RequirementId = record.req_id!,
                        ChapterId = chapter.Id,
                        Title = record.req_id!,
                        Description = record.req_description ?? string.Empty,
                        Level1 = record.level1 ?? string.Empty,
                        Level2 = record.level2 ?? string.Empty,
                        Level3 = record.level3 ?? string.Empty,
                        Cwe = record.cwe ?? string.Empty,
                        Nist = record.nist ?? string.Empty
                    };
                    await _context.AsvsRequirements.AddAsync(req);
                    reqCount++;
                }
            }

            await _context.SaveChangesAsync();
            return (chapterMap.Count, reqCount);
        }
    }
}
