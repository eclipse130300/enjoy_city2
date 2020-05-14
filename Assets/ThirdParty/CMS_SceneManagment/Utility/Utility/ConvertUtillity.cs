using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class ConvertUtillity
{
    private static byte[] privateKey;
    public static byte[] PrivateKey
    {
        get
        {
            if(privateKey == null)
            {
                privateKey = CharsToByteArray("ITS TEST KEY".ToCharArray());
            }
            return privateKey;
        }
    }
    [StructLayout(LayoutKind.Explicit)]
    private struct ConverterHelperDouble
    {
        [FieldOffset(0)]
        public ulong Along;

        [FieldOffset(0)]
        public double Adouble;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct ConverterHelperFloat
    {
        [FieldOffset(0)]
        public int Aint;

        [FieldOffset(0)]
        public float Afloat;
    }
    public static byte ConvertToByte(this BitArray bits)
    {
        if (bits.Count != 8)
        {
            throw new ArgumentException("illegal number of bits");
        }

        byte b = 0;
        if (bits.Get(0)) b++;
        if (bits.Get(1)) b += 2;
        if (bits.Get(2)) b += 4;
        if (bits.Get(3)) b += 8;
        if (bits.Get(4)) b += 16;
        if (bits.Get(5)) b += 32;
        if (bits.Get(6)) b += 64;
        if (bits.Get(7)) b += 128;
        return b;
    }
    private static void Write(byte[] buffer, int offset, ulong data)
    {
        buffer[offset] = (byte)(data);
        buffer[offset + 1] = (byte)(data >> 8);
        buffer[offset + 2] = (byte)(data >> 16);
        buffer[offset + 3] = (byte)(data >> 24);
        buffer[offset + 4] = (byte)(data >> 32);
        buffer[offset + 5] = (byte)(data >> 40);
        buffer[offset + 6] = (byte)(data >> 48);
        buffer[offset + 7] = (byte)(data >> 56);
    }

    private static void Write(byte[] buffer, int offset, int data)
    {
        buffer[offset] = (byte)(data);
        buffer[offset + 1] = (byte)(data >> 8);
        buffer[offset + 2] = (byte)(data >> 16);

    }

    public static void Write(byte[] buffer, int offset, short data)
    {

        buffer[offset] = (byte)(data);
        buffer[offset + 1] = (byte)(data >> 8);
    }

    public static void GetBytes(byte[] bytes, int startIndex, double value)
    {
        ConverterHelperDouble ch = new ConverterHelperDouble { Adouble = value };
        Write(bytes, startIndex, ch.Along);
    }

    public static void GetBytes(byte[] bytes, int startIndex, float value)
    {
        ConverterHelperFloat ch = new ConverterHelperFloat { Afloat = value };
        Write(bytes, startIndex, ch.Aint);
    }

    public static void GetBytes(byte[] bytes, int startIndex, short value)
    {
        Write(bytes, startIndex, value);
    }

    public static void GetBytes(byte[] bytes, int startIndex, ushort value)
    {
        Write(bytes, startIndex, (short)value);
    }

    public static void GetBytes(byte[] bytes, int startIndex, int value)
    {
        Write(bytes, startIndex, value);
    }

    public static void GetBytes(byte[] bytes, int startIndex, uint value)
    {
        Write(bytes, startIndex, (int)value);
    }

    public static void GetBytes(byte[] bytes, int startIndex, long value)
    {
        Write(bytes, startIndex, (ulong)value);
    }

    public static void GetBytes(byte[] bytes, int startIndex, ulong value)
    {
        Write(bytes, startIndex, value);
    }

    public static string BytesToString(byte[] contents)
    {
        return new string(BytesToCrarArray(contents));
    }
    public static byte[] StringToByteArray(string contents)
    {
        return CharsToByteArray(contents.ToCharArray());
    }
    /*
    public static byte[] stringToBytesUTFCustom(char[] str)
    {
        byte[] b = new byte[str.Length << 1];
        for (int i = 0; i < str.Length; i++)
        {
            char strChar = str[i];
            int bpos = i << 1;
            b[bpos] = (byte)((strChar & 0xFF00) >> 8);
            b[bpos + 1] = (byte)(strChar & 0x00FF);
        }
        return b;
    }*/
    public static byte[] CharsToByteArray(char[] contents)
    {
        if (contents == null)
            return null;

        byte[] byteArray = Encoding.Default.GetBytes(contents);

        byte[] b = new byte[contents.Length];

        for (int i = 0; i < b.Length; i++)
        {
            b[i] = (byte)contents[i];
        }
        return byteArray;
    }
    public static char[] BytesToCrarArray(byte[] contents)
    {
        char[] charArray = Encoding.Default.GetChars(contents);

        if (contents == null)
            return null;

        char[] b = new char[contents.Length];

        for (int i = 0; i < b.Length; i++)
        {

            b[i] = (char)contents[i];
        }
         return charArray;
    }
    /// <summary>
    /// Для однобайтовых символов
    /// </summary>
    /// <param name="chars"></param>
    public static void Decrypt(ref char[] chars)
    {
        Encrypt(ref chars);
    }
    /// <summary>
    /// Для однобайтовых символов
    /// </summary>
    /// <param name="chars"></param>
    public static void Encrypt(ref char[] chars)
    {
        int n = 0;
        for (int i = 0; i < chars.Length; i++)
        {
            if (n >= PrivateKey.Length)
            {
                n = 0;
            }
            chars[i] =(char) ((byte)chars[i] ^ PrivateKey[n]);
            n++;   
        }
    }
    public static void Decrypt(this byte[] chars)
    {
        Encrypt(chars, PrivateKey);
    }
    public static void Encrypt(this byte[] chars)
    {
        Encrypt(chars, PrivateKey);
    }
    public static void Decrypt(this byte[] chars, byte[] key)
    {
        Encrypt(chars, key);
    }
    public static void Encrypt(this byte[] chars, byte[] key)
    {
        int n = 0;
        for (int i = 0; i < chars.Length; i++)
        {
            if (n >= key.Length)
            {
                n = 0;
            }
            chars[i] = (byte)(chars[i] ^ key[n]);
            n++;
           
        }
    }
    public static bool GetBit(this int b, int bitNumber)
    {
        return (b & (1 << bitNumber - 1)) != 0;
    }
    public static bool GetBit(this byte b, int bitNumber)
    {
        return (b & (1 << bitNumber - 1)) != 0;
    }
    public static byte SetBit(this byte b, int bitNumber,int value)
    {
        byte mask = (byte)( 1 << bitNumber);

       
        if (value == 0)
        {
            b = (byte)(b & ~mask);
        }
        else
        {
            b = (byte)(b | mask);
        }
        return b;
    }
}

