using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/* =================================== */
/* SAVEBLE OBJECT MARKER               */
/* =================================== */

public interface ISaveble { }

/* =================================== */
/* SAVE OBJECTS WHEREVER YOU WANT      */
/* =================================== */

public static class ISavebleExtension
{
    public static void Save<T>(this T data) where T : ISaveble
    {
        DataSaver.Save(data);
    }

    public static T Load<T>(this T data) where T : ISaveble
    {
        return DataSaver.Load<T>();
    }
}

/* =================================== */
/* SAVE OBJECTS TO PLAYER_PREFS        */
/* =================================== */

public static class DataSaver
{
    public static void Save<T>(T data)
    {
        var key = typeof(T).ToString();
        using (var stream = new MemoryStream())
        {
            new BinaryFormatter().Serialize(stream, data);
            var str = Convert.ToBase64String(stream.ToArray());
            PlayerPrefs.SetString(key, str);
        }
    }

    public static T Load<T>()
    {
        var key = typeof(T).ToString();
        if (PlayerPrefs.HasKey(key) && PlayerPrefs.GetString(key) != string.Empty)
        {
            var str = PlayerPrefs.GetString(key);
            byte[] bytes = Convert.FromBase64String(str);

            using (var stream = new MemoryStream(bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
        }
        return default(T);
    }

    public static void Clear<T>()
    {
        var key = typeof(T).ToString();
        if (PlayerPrefs.HasKey(key))
            PlayerPrefs.DeleteKey(key);
    }
}