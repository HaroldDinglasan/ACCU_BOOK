using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Timers;
using static System.Net.WebRequestMethods;

namespace Accounting_V4.Services.SystemServices.TimeZoneServices;


public class TimeService
{
    private readonly HttpClient _httpClient;
    private static DateTime _initialManilaTime;
    private static Stopwatch _stopwatch = new();
    public string CountryName { get; private set; } = "Loading...";
    private bool _isInitialized = false;

    public TimeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // Simulan na agad ang pag-sync sa background (fire and forget)
        _ = SyncWithApi();
    }

    private async Task SyncWithApi()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<TimeApiResponse>("https://timeapi.io/api/Time/current/zone?timeZone=Asia/Manila");
            if (response?.DateTime != null)
            {
               
                _initialManilaTime = DateTime.Parse(response.DateTime);
                CountryName = response.TimeZone?.Split('/').Last();
                _stopwatch.Restart(); // Simulan ang bilang mula sa official time
                _isInitialized = true;
            }
        }
        catch
        {
            CountryName = "Philippines (Offline)";
            _initialManilaTime = DateTime.Now; // Fallback
            _isInitialized = true;
        }
    }

    // ITO ANG MAGIC PROPERTY: Instant access, walang delay!
    public DateTime ManilaNow
    {
        get
        {
            if (!_isInitialized) return DateTime.Now;
            // Official API Time + Time elapsed since last sync
            return _initialManilaTime.Add(_stopwatch.Elapsed);
        }
    }

    public static DateTime ServerTime
    {
        get
        {
            if (_initialManilaTime == default)
                return DateTime.Now;

            return _initialManilaTime.Add(_stopwatch.Elapsed);
        }
        //get
        //{
        //    // Official API Time + Time elapsed since last sync
        //    return _initialManilaTime.Add(_stopwatch.Elapsed);
        //}
    }

  
   // public DateTime GetManilaTime() => ManilaNow; // Optional method for explicit access
}

//public class TimeService : IDisposable
//{
//    private readonly HttpClient _httpClient;
//   private System.Timers.Timer _timer;
//    private DateTime? _currentTime;

//    // Ito ang tatawagin ng Page para makakuha ng update
//    public event Action<DateTime>? OnTimeUpdated;

//    public TimeService(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//        StartTimer();
//    }

//    private void StartTimer()
//    {
//        _timer = new System.Timers.Timer(1000); // Bawat 1 segundo
//        _timer.Elapsed += async (sender, e) => await UpdateTime();
//        _timer.AutoReset = true;
//        _timer.Enabled = true;
//    }

//    private async Task UpdateTime()
//    {
//        if (_currentTime == null)
//        {
//            // First time lang tatawag sa API para sa initial sync
//            _currentTime = await FetchTimeFromApi();
//        }
//        else
//        {
//            // Sa susunod na ticks, dagdag na lang sa memory para tipid sa API call
//            _currentTime = _currentTime.Value.AddSeconds(1);
//        }

//        if (_currentTime.HasValue)
//        {
//            // I-notify ang Page na may bagong oras na
//            OnTimeUpdated?.Invoke(_currentTime.Value);
//        }
//    }

//    private async Task<DateTime?> FetchTimeFromApi()
//    {
//        try
//        {
//            var response = await _httpClient.GetFromJsonAsync<TimeApiResponse>("https://timeapi.io/api/Time/current/zone?timeZone=Asia/Manila");
//            return DateTime.Parse(response.DateTime);
//        }
//        catch
//        {
//            return DateTime.Now; // Fallback sa server time pag nag-fail ang API
//        }
//    }

//    public void Dispose() => _timer?.Dispose();
//}

