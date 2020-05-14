using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace System.IO
{

    public class FileUtility
    {


        public static void RemoveFile(string path)
        {
            File.Delete(path);
            //await WriteAllChars(path, ConvertUtillity.BytesToCrarArray(contents));
        }


        #region Write
        public static async Task WriteAllText(string path, string contents)
        {
            await WriteAllText(path, contents, Encoding.ASCII);
        }
        public static async Task WriteAllText(string path, string contents, Encoding encoding)
        {
            await WriteAllChars(path, contents.ToCharArray());
        }

        public static async Task WriteAllBytes(string path, byte[] contents)
        {
            try
            {
                if (!Directory.Exists(path.Substring(0, path.LastIndexOf('/'))))
                    Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));
#if UNITY_EDITOR

                using (FileStream SourceStream = File.Open(path, FileMode.Create))
                {
                    SourceStream.Seek(0, SeekOrigin.End);
                    await SourceStream.WriteAsync(contents, 0, contents.Length);
                }
#elif UNITY_ANDROID
            if (LoadSceneAssync.hasReadWritePermission)
            {
                using (FileStream SourceStream = File.Open(path, FileMode.Create))
                {
                    SourceStream.Seek(0, SeekOrigin.End);
                    await SourceStream.WriteAsync(contents, 0, contents.Length);
                }
            }
            else
            {
                string outStr = new string(contents);
                PlayerPrefs.SetString(path, outStr);
            }       
#endif
            }
            catch (Exception ex)
            {
                Debug.Log("WriteAllChars " + ex);
            }

        }
    

        public static async Task WriteAllChars(string path, char[] contents)
        {
            try
            {
                if (!Directory.Exists(path.Substring(0, path.LastIndexOf('/'))))
                    Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('/')));
#if UNITY_EDITOR

                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, true))
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    await sw.WriteLineAsync(contents);
                }
#elif UNITY_ANDROID
            if (LoadSceneAssync.hasReadWritePermission)
            {
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, true))
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    await sw.WriteAsync(contents);
                }
            }
            else
            {
                string outStr = new string(contents);
                PlayerPrefs.SetString(path, outStr);
            }       
#endif
            }
            catch (Exception ex)
            {
                Debug.Log("WriteAllChars "+ex);
            }

        }
        #endregion

        #region Read
        public static async Task<string> ReadAlltext(string path)
        {
            return await ReadAlltext(path, Encoding.ASCII);
        }
        public static async Task<string> ReadAlltext(string path, Encoding encoding)
        {
            return encoding.GetString(await ReadAllBytes(path));
        }
        public static async Task<byte[]> ReadAllBytes(string path)
        {
            try
            {
                Byte[] file;
#if UNITY_EDITOR


                if (!File.Exists(path))
                {
                    return null;
                }

                using (FileStream SourceStream = File.Open(path, FileMode.Open))
                {
                    file = new byte[SourceStream.Length];
                    await SourceStream.ReadAsync(file, 0, (int)SourceStream.Length);
                }


                return file;
#elif UNITY_ANDROID
            if (LoadSceneAssync.hasReadWritePermission)
            {
               string result = "";
                if (!File.Exists(path))
                {
                    return null;
                }

                using (FileStream SourceStream = File.Open(path, FileMode.Open))
                {
                    file = new byte[SourceStream.Length];
                    await SourceStream.ReadAsync(file, 0, (int)SourceStream.Length);
                }


                return file;
            }
            else
            {
                SharedValue<string> outStr = new SharedValue<string>();
                await Task.Run(() =>
                {
                    
                    SharedValue<bool> loaded = new SharedValue<bool>();
                    TaskManager.Instance.ExecuteInMainThread(() => {
                        outStr.Value = PlayerPrefs.GetString(path);
                        loaded.Value = true;
                    });
                    while (!loaded.Value)
                    {
                        Task.Delay(500);
                    }
                });
                
                
                
                return outStr.Value.ToCharArray();
            }
#endif
            }
            catch (Exception ex)
            {
                Debug.Log("ReadAllbyte " + ex);
            }
            return null;
        }
        public static byte[] ReadAllBytesSync(string path)
        {
            try
            {
                Byte[] file;
#if UNITY_EDITOR


                if (!File.Exists(path))
                {
                    return null;
                }

                using (FileStream SourceStream = File.Open(path, FileMode.Open))
                {
                    file = new byte[SourceStream.Length];
                    SourceStream.Read(file, 0, (int)SourceStream.Length);
                }


                return file;
#elif UNITY_ANDROID
            if (LoadSceneAssync.hasReadWritePermission)
            {
               string result = "";
                if (!File.Exists(path))
                {
                    return null;
                }

                using (FileStream SourceStream = File.Open(path, FileMode.Open))
                {
                    file = new byte[SourceStream.Length];
                    SourceStream.ReadAsync(file, 0, (int)SourceStream.Length);
                }


                return file;
            }
            else
            {

                outStr.Value = PlayerPrefs.GetString(path);
                loaded.Value = true;
                return outStr.Value.ToCharArray();
            }
#endif
            }
            catch (Exception ex)
            {
                Debug.Log("ReadAllbyte " + ex);
            }
            return null;
        }
        public static async Task<char[]> ReadAllChars(string path)
        {

#if UNITY_EDITOR
            string result = "";
            if (!File.Exists(path))
            {
                return null;
            }
            using (FileStream SourceStream = File.Open(path, FileMode.OpenOrCreate))
            {
                using (StreamReader reader = new StreamReader(SourceStream))
                {
                    try
                    {
                        result = await reader.ReadLineAsync();
                    }
                    catch
                    {

                    }
                }
            }


            return result.ToCharArray();
#elif UNITY_ANDROID
            if (LoadSceneAssync.hasReadWritePermission)
            {
                string result ="";

                using (FileStream SourceStream = File.Open(path,FileMode.OpenOrCreate))
                {
                    using (StreamReader reader = new StreamReader(SourceStream))
                    {
                        result = await reader.ReadToEndAsync();
                    }
                   
                }
               
                return result.ToCharArray();
            }
            else
            {
                SharedValue<string> outStr = new SharedValue<string>();
                await Task.Run(() =>
                {
                    
                    SharedValue<bool> loaded = new SharedValue<bool>();
                    TaskManager.Instance.ExecuteInMainThread(() => {
                        outStr.Value = PlayerPrefs.GetString(path);
                        loaded.Value = true;
                    });
                    while (!loaded.Value)
                    {
                        Task.Delay(500);
                    }
                });
                
                
                
                return outStr.Value.ToCharArray();
            }
#endif

        }
#endregion
    }
}
