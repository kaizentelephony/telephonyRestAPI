using ClosedXML.Excel;
using CsvHelper;
using DocumentFormat.OpenXml.Packaging;
using PdfSharp.Pdf;
using System.Formats.Asn1;
using System.Globalization;

namespace Call_Details_API.Helpers
{
    public class FileDataReader
    {
        public static IEnumerable<string[]> ReadTable(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".xlsx" or ".xls" => ReadExcel(path),
                ".csv" or ".tsv" or ".txt" => ReadCsv(path),
               // ".pdf" => ReadPdf(path),
                ".docx" => ReadWord(path),
                _ => throw new NotSupportedException($"Unsupported file: {ext}")
            };
        }

        private static IEnumerable<string[]> ReadExcel(string path)
        {
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheets.First();
            foreach (var row in ws.RowsUsed())
                yield return row.Cells().Select(c => c.GetString()).ToArray();
        }

        private static IEnumerable<string[]> ReadCsv(string path)
        {
            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            foreach (var record in csv.GetRecords<dynamic>())
                yield return ((IDictionary<string, object>)record)
                    .Values.Select(v => v?.ToString() ?? "").ToArray();
        }     

        private static IEnumerable<string[]> ReadWord(string path)
        {
            using var doc = WordprocessingDocument.Open(path, false);
            var body = doc.MainDocumentPart?.Document.Body;
            if (body == null) yield break;
            foreach (var p in body.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
                yield return new[] { p.InnerText.Trim() };
        }
    }
}
