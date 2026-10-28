namespace Accounting_V4.Services.MenuServices;

public interface ISqlDataAccess
{
    Task<List<T>> LoadData<T, U>(string sql, U parameters);
    Task<int> SaveData<T>(string sql, T parameters);
    Task<T> LoadSingleData<T>(string sql, object parameters = null);
    Task SpSaveData<T>(string sql, T parameters);
    Task<int> SaveDataReturnId<T>(string sql, T parameters);
    //Task<T> SaveData<T>(string query, object parameters);
}
