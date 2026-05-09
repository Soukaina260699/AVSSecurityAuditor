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
        public string? Chapter { get; set; }
        public string? ChapterName { get; set; }
        public string? Section { get; set; }
        public string? SectionName { get; set; }
        public string? Req_id { get; set; }
        public string? Item { get; set; }
        public string? Description { get; set; }
        public string? L1 { get; set; }
        public string? L2 { get; set; }
        public string? L3 { get; set; }
        public string? CWE { get; set; }
        public string? NIST { get; set; }
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
                BadDataFound = null
            };

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<CsvImportRecord>().ToList();

            var chapterMap = new Dictionary<string, AsvsChapter>();

            foreach (var record in records)
            {
                if (string.IsNullOrWhiteSpace(record.Req_id)) continue;

                var chapterKey = record.Chapter ?? "0";
                if (!chapterMap.ContainsKey(chapterKey))
                {
                    var existing = await _context.AsvsChapters
                        .FirstOrDefaultAsync(c => c.ChapterNumber.ToString() == chapterKey);
                    if (existing == null)
                    {
                        existing = new AsvsChapter
                        {
                            ChapterNumber = int.TryParse(chapterKey, out var n) ? n : 0,
                            Name = record.ChapterName ?? $"Chapter {chapterKey}",
                            Description = record.SectionName ?? string.Empty
                        };
                        await _context.AsvsChapters.AddAsync(existing);
                        await _context.SaveChangesAsync();
                    }
                    chapterMap[chapterKey] = existing;
                }

                var chapter = chapterMap[chapterKey];
                var reqExists = await _context.AsvsRequirements
                    .AnyAsync(r => r.RequirementId == record.Req_id);
                if (!reqExists)
                {
                    var req = new AsvsRequirement
                    {
                        RequirementId = record.Req_id!,
                        ChapterId = chapter.Id,
                        Title = record.Item ?? string.Empty,
                        Description = record.Description ?? string.Empty,
                        Level1 = record.L1 ?? string.Empty,
                        Level2 = record.L2 ?? string.Empty,
                        Level3 = record.L3 ?? string.Empty,
                        Cwe = record.CWE ?? string.Empty,
                        Nist = record.NIST ?? string.Empty
                    };
                    await _context.AsvsRequirements.AddAsync(req);
                }
            }

            await _context.SaveChangesAsync();
            return (chapterMap.Count, records.Count(r => !string.IsNullOrWhiteSpace(r.Req_id)));
        }
    }
}
