using System.Collections.Generic;

public static class DataStorage
{
    private static readonly Dictionary<string, object> dataStore = new Dictionary<string, object>();

    public static void Set(string key, object value)
    {
        if (dataStore.ContainsKey(key))
        {
            dataStore[key] = value;
        }
        else
        {
            dataStore.Add(key, value);
        }
    }

    public static object Get(string key)
    {
        return dataStore.ContainsKey(key) ? dataStore[key] : null;
    }

    public static T Get<T>(string key, T defaultValue = default)
    {
        if (dataStore.ContainsKey(key) && dataStore[key] is T typedValue)
        {
            return typedValue;
        }
        
        return defaultValue;
    }

    public static bool Contains(string key)
    {
        return dataStore.ContainsKey(key);
    }

    public static void Remove(string key)
    {
        if (dataStore.ContainsKey(key))
        {
            dataStore.Remove(key);
        }
    }

    public static void Clear()
    {
        dataStore.Clear();
    }
}
