using ParsedCvData.DataFetcher;
using ParsedCvData.JsonProcessor;
using ParsedCvData.PdfGenerator;

class Program
{
    private static readonly string ConnectionString = "Server=SIDDIQUILAP\\SA; Database=Import&ExportData; TrustServerCertificate=true; Trusted_Connection=True;";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter comma-separated IDs for bulk PDF generation:");
        var input = Console.ReadLine();
        var ids = ParseIds(input);

        if (ids == null || !ids.Any())
        {
            Console.WriteLine("No valid IDs provided.");
            return;
        }

        var dataFetcher = new DataFetcher(ConnectionString);
        var jsonProcessor = new JsonProcessor();

        foreach (var id in ids)
        {
            try
            {
                // Step 1: Data Fetch
                var jsonObject = await dataFetcher.FetchDataAsync(id);
                if (jsonObject == null)
                {
                    Console.WriteLine($"Data not found or already processed for ID: {id}");
                    continue;
                }

                // Step 2: JSON Processing
                dynamic candidate = jsonProcessor.ConvertToDynamic(jsonObject);
                var dataList = jsonProcessor.ExtractData(candidate);

                // Step 3: PDF Generation
                var pdfPath = PdfGenerator.GeneratePdf(candidate);
                Console.WriteLine($"PDF generated at: {pdfPath}");

                // Update the record to indicate that PDF has been generated
                var updateSuccessful = await dataFetcher.UpdateRecordAsync(id);
                if (updateSuccessful)
                {
                    Console.WriteLine($"Record updated successfully for ID: {id}");
                }
                else
                {
                    Console.WriteLine($"Failed to update the record for ID: {id}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred for ID {id}: {ex.Message}");
            }
        }
    }

    private static List<int> ParseIds(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new List<int>();

        var idStrings = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var ids = new List<int>();

        foreach (var idString in idStrings)
        {
            if (int.TryParse(idString.Trim(), out var id))
            {
                ids.Add(id);
            }
            else
            {
                Console.WriteLine($"Invalid ID: {idString.Trim()}");
            }
        }

        return ids;
    }
}
