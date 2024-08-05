using Dapper;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;


namespace ParsedCvData.DataFetcher
{
    public class DataFetcher
    {
        private readonly string _connectionString;
        public DataFetcher(string connectionString)
        {
            _connectionString = connectionString;
        }
        // Fetch data asynchronously
        public async Task<JObject> FetchDataAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Check if IsStandardCVGenerated is 0 and IsValidCV is 1 (i.e., eligible for fetching)
                    var isEligible = await connection.ExecuteScalarAsync<bool>(
                        @"SELECT CASE WHEN IsStandardCVGenerated = 0 AND IsValidCV = 1 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END 
                        FROM tblParsedCVData 
                        WHERE Id = @Id",
                        new { Id = id });

                    // If not eligible, return null
                    if (!isEligible) return null;

                    // Fetch the ParsedData as JSON string
                    var jsonData = await connection.QuerySingleOrDefaultAsync<string>(
                        "SELECT ParsedData FROM tblParsedCVData WHERE Id = @Id",
                        new { Id = id });

                    // Parse JSON string to JObject and return
                    return jsonData != null ? JObject.Parse(jsonData) : null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Data Fetch Error: {ex.Message}");
                return null;
            }
        }
        // Update record asynchronously
        public async Task<bool> UpdateRecordAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    // Check if IsValidCV is 1 before updating
                    var isValid = await connection.ExecuteScalarAsync<bool>(
                        "SELECT IsValidCV FROM tblParsedCVData WHERE Id = @Id",
                        new { Id = id });

                    if (!isValid)
                    {
                        Console.WriteLine("Record is not valid for update.");
                        return false;
                    }

                    var rowsAffected = await connection.ExecuteAsync(
                        "UPDATE tblParsedCVData SET IsStandardCVGenerated = 1 WHERE Id = @Id",
                        new { Id = id });

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update Error: {ex.Message}");
                return false;
            }
        }
    }
}
