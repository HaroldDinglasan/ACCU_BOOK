using System.Reflection;

namespace Accounting_V4.Services.Security;


public interface IDevExpressPasswordService
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string storedHash);
}
public class DevExpressPasswordService : IDevExpressPasswordService
{
    private readonly Type cryptographerType;
    private readonly MethodInfo hashMethod;
    private readonly MethodInfo verifyMethod;

    public DevExpressPasswordService()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "DevExpress", "DevExpress.Persistent.Base.v20.1.dll");
        if (!File.Exists(path))
            path = Path.Combine(AppContext.BaseDirectory, "DevExpress.Persistent.Base.v20.1.dll");
        if (!File.Exists(path))
            throw new FileNotFoundException("DevExpress password DLL was not found.");

        Assembly assembly = Assembly.LoadFrom(path);
        cryptographerType = assembly.GetType("DevExpress.Persistent.Base.PasswordCryptographer", true);

        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        // Static ito, kaya target = null
        SetStatic("EnableRfc2898", false);
        SetStatic("SupportLegacySha512", true);

        hashMethod = cryptographerType.GetMethod("HashPassword", flags, null, new[] { typeof(string) }, null)
            ?? throw new MissingMethodException("DevExpress HashPassword method was not found.");

        verifyMethod = cryptographerType.GetMethod("VerifyHashedPassword", flags, null,
                           new[] { typeof(string), typeof(string) }, null)
            ?? throw new MissingMethodException("DevExpress VerifyHashedPassword method was not found.");
    }

    private void SetStatic(string name, object value)
    {
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        PropertyInfo prop = cryptographerType.GetProperty(name, flags);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(null, value);
            return;
        }

        FieldInfo field = cryptographerType.GetField(name, flags);
        field?.SetValue(null, value);
    }

    public string Hash(string plainPassword)
    {
        if (string.IsNullOrEmpty(plainPassword))
            throw new ArgumentException("Password is required.", nameof(plainPassword));

        return (string)hashMethod.Invoke(null, new object[] { plainPassword });
    }

    public bool Verify(string plainPassword, string storedHash)
    {
        if (string.IsNullOrEmpty(plainPassword) || string.IsNullOrEmpty(storedHash))
            return false;

        try
        {
            // Order ng parameters: (hashedPassword, password)
            return (bool)verifyMethod.Invoke(null, new object[] { storedHash, plainPassword });
        }
        catch (TargetInvocationException)
        {
            return false; // corrupt/invalid hash format
        }
    }
}
