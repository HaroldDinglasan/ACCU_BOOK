using System.Security.Cryptography;

namespace Accounting_V4.Services.QNEServices.AllBasicServices;

public class EISIdService
{
    private const string Characters ="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string Generate()
    {
        string date = DateTime.Now.ToString("yyyyMMdd");
        string random = GenerateRandomString(8);

        return $"{date}{random}";
    }

    private static string GenerateRandomString(int length)
    {
        char[] result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = Characters[
                RandomNumberGenerator.GetInt32(Characters.Length)
            ];
        }

        return new string(result);
    }
}
