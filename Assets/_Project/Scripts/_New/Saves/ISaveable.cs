public interface ISaveable<T>
{
    T GetSaveData();                     // отдать свой срез данных
    void LoadSaveData(T data);           // применить данные при загрузке (без вызова событий)
}