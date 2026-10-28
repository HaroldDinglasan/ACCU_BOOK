using System.Data;
using System.Text.RegularExpressions;
using Accounting_V4.ConnectionServices;
using Accounting_V4.Model.QNEModels.RunningNumberModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Accounting_V4.Services.QNEServices.RunningNumberServices;

/// <summary>
///  FOR GENERATING RUNNING NUMBERS BASED ON SPECIFIED FORMATS AND DATE VARIABLES
/// </summary>
public class RunningNumberService
{


    private readonly QneConnectionString qneConnection;

    public RunningNumberService(QneConnectionString qneConnection)
    {
        this.qneConnection = qneConnection;
    }

    public async Task<string> GetNextRunningNumber(RunningNumbers runningNumbers)
    {
        string regexPattern = BuildSqlLikePattern(runningNumbers.Format, runningNumbers.DocDate);
        string currentDatePart = ReplaceDateVariables(runningNumbers.Format, runningNumbers.DocDate);

        var connectionString = await qneConnection.GetConnectionStringAsync();
        var connection = new SqlConnection(connectionString);

        var sql = $@"
        SELECT TOP 1 {runningNumbers.ColumnName}
        FROM {runningNumbers.TableName}
        WHERE {runningNumbers.ColumnName} LIKE @Pattern
        ORDER BY {runningNumbers.ColumnName} DESC";

        var lastCode = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Pattern = regexPattern });

        int nextNumber = 1;
        if (!string.IsNullOrEmpty(lastCode))
        {
            var numberPart = ExtractRunningNumber(lastCode, runningNumbers.Format, runningNumbers.DocDate);
            if (int.TryParse(numberPart, out int num))
                nextNumber = num + 1;
        }

        return ReplaceRunningNumber(currentDatePart, runningNumbers.Format, nextNumber, runningNumbers.DocDate);
    }

    public string BuildSqlLikePattern(string format, DateTime docDate)
    {
        string pattern = format;

        pattern = pattern.Replace("{yy}", docDate.ToString("yy"));
        pattern = pattern.Replace("{yyyy}", docDate.ToString("yyyy"));
        pattern = pattern.Replace("{mm}", docDate.ToString("MM"));
        pattern = pattern.Replace("{m}", docDate.Month.ToString());
        pattern = pattern.Replace("{dd}", docDate.ToString("dd"));
        pattern = pattern.Replace("{d}", docDate.Day.ToString());

        var match = Regex.Match(format, @"\{n(\d+)\}");
        if (match.Success)
        {
            int length = int.Parse(match.Groups[1].Value);
            pattern = Regex.Replace(pattern, @"\{n\d+\}", new string('_', length));
        }

        return pattern;
    }

    public string ReplaceDateVariables(string format, DateTime docDate)
    {
        string result = format;
        result = result.Replace("{yy}", docDate.ToString("yy"));
        result = result.Replace("{yyyy}", docDate.ToString("yyyy"));
        result = result.Replace("{mm}", docDate.ToString("MM"));
        result = result.Replace("{m}", docDate.Month.ToString());
        result = result.Replace("{dd}", docDate.ToString("dd"));
        result = result.Replace("{d}", docDate.Day.ToString());
        return result;
    }

    public string ExtractRunningNumber(string code, string format, DateTime docDate)
    {
        var match = Regex.Match(format, @"\{n(\d+)\}");
        if (!match.Success) return "0";

        int length = int.Parse(match.Groups[1].Value);

        string sample = ReplaceDateVariables(format, docDate);

        int placeholderIndex = format.IndexOf(match.Value);
        if (placeholderIndex < 0)
            return "0";

        string beforePlaceholder = ReplaceDateVariables(format.Substring(0, placeholderIndex), docDate);
        int startIndex = beforePlaceholder.Length;

        if (startIndex < 0 || startIndex + length > code.Length)
            return "0";

        return code.Substring(startIndex, length);
    }

    public string ReplaceRunningNumber(string baseCode, string format, int number, DateTime docDate)
    {
        var match = Regex.Match(format, @"\{n(\d+)\}");
        if (!match.Success) return baseCode;

        int length = int.Parse(match.Groups[1].Value);
        string numStr = number.ToString().PadLeft(length, '0');

        string codeWithDate = ReplaceDateVariables(baseCode, docDate);
        return Regex.Replace(codeWithDate, @"\{n\d+\}", numStr);
    }
}
