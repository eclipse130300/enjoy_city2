using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Md5Util  {

    public static string GetHash(string usedString, string secretKey)
    { //Create a Hash to send to server
        MD5 md5 = MD5.Create();
       
        byte[] bytes = Encoding.UTF8.GetBytes(usedString + secretKey);
        byte[] hash = md5.ComputeHash(bytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();
    }
    public static string XOR(string value, string key)
    {
        string result = "";
        if (key.Length > 0)
        {
            int n = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (n >= key.Length)
                {
                    n = 0;
                }
                result += value[i] ^ key[n];

            }
        } else {
            result = value;
        }
        return result;
    }

    public static byte[] XOR(byte[] value, byte[] key)
    {
        if (key != null && key.Length > 0)
        {
            int n = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (n >= key.Length)
                {
                    n = 0;
                }
                value [i] = (byte)(value[i] ^ key[n]);

            }
        }
 
        return value;
    }
}
