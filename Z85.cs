using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ZeroMQ.lib;

namespace ZeroMQ
{
  public static class Z85
  {
    public static void CurveKeypair(out string publicKey, out string secretKey)
    {
      const int destLen = 40;
      var publicKeyBuilder = new StringBuilder(destLen);
      var secretKeyBuilder = new StringBuilder(destLen);

      int res = zmq.curve_keypair(publicKeyBuilder, secretKeyBuilder);
      Debug.Assert(res == 0);

      publicKey = publicKeyBuilder.ToString();
      secretKey = secretKeyBuilder.ToString();
    }


    [Obsolete]
    public static void CurveKeypair(out byte[] publicKey, out byte[] secretKey)
    {
      string publicKeyStr, secretKeyStr;
      CurveKeypair(out publicKeyStr, out secretKeyStr);
      publicKey = Encoding.ASCII.GetBytes(publicKeyStr);
      secretKey = Encoding.ASCII.GetBytes(secretKeyStr);
    }

    [Obsolete]
    public static byte[] Encode(byte[] decoded)
    {
      return Encoding.ASCII.GetBytes(ToZ85(decoded));
    }

    public static string ToZ85(byte[] decoded)
    {
      int dataLen = decoded.Length;
      if (dataLen % 4 > 0)
      {
        throw new ArgumentException("decoded.Length must be divisible by 4", nameof(decoded));
      }
      int destLen = (Int32)(decoded.Length * 1.25);

      var data = GCHandle.Alloc(decoded, GCHandleType.Pinned);

      try
      {
        var dest = new StringBuilder(destLen);
        IntPtr res = zmq.z85_encode(dest, data.AddrOfPinnedObject(), dataLen);
        Debug.Assert(res != IntPtr.Zero);

        return dest.ToString();
      }
      finally
      {
        data.Free();
      }
    }

    [Obsolete]
    public static byte[] ToZ85Encoded(this byte[] decoded) { return Encode(decoded); }

    [Obsolete]
    public static string ToZ85Encoded(this string decoded) { return Encode(decoded, ZContext.Encoding); }

    [Obsolete]
    public static string ToZ85Encoded(this string decoded, Encoding encoding) { return Encode(decoded, encoding); }

    [Obsolete]
    public static byte[] ToZ85EncodedBytes(this string decoded) { return EncodeBytes(decoded, ZContext.Encoding); }

    [Obsolete]
    public static byte[] ToZ85EncodedBytes(this string decoded, Encoding encoding)
    {
      return EncodeBytes(decoded, encoding);
    }

    [Obsolete]
    public static string Encode(string strg) { return Encode(strg, ZContext.Encoding); }

    [Obsolete]
    public static string Encode(string strg, Encoding encoding)
    {
      byte[] encoded = EncodeBytes(strg, encoding);
      return encoding.GetString(encoded);
    }

    [Obsolete]
    public static byte[] EncodeBytes(string strg) { return EncodeBytes(strg, ZContext.Encoding); }

    [Obsolete]
    public static byte[] EncodeBytes(string strg, Encoding encoding)
    {
      byte[] bytes = encoding.GetBytes(strg);
      return Encode(bytes);
    }

    [Obsolete]
    public static byte[] Decode(byte[] encoded) { return FromZ85(Encoding.ASCII.GetString(encoded)); }

    public static byte[] FromZ85(string encoded)
    {
      int dataLen = encoded.Length;
      if (dataLen % 5 > 0)
      {
        throw new ArgumentException("encoded.Length must be divisible by 5", nameof(encoded));
      }
      int destLen = (Int32)(encoded.Length * .8);

      var data = GCHandle.Alloc(encoded, GCHandleType.Pinned);

      try
      {
        using (var dest = DispoIntPtr.Alloc(destLen))
        {
          if (IntPtr.Zero == zmq.z85_decode(dest, data.AddrOfPinnedObject()))
          {
            throw new ArgumentException("Cannot decode Z85 data", nameof(encoded));
          }

          var decoded = new byte[destLen];

          Marshal.Copy(dest, decoded, 0, decoded.Length);

          return decoded;
        }
      }
      finally { data.Free(); }
    }

    [Obsolete]
    public static byte[] ToZ85Decoded(this byte[] encoded) { return Decode(encoded); }

    [Obsolete]
    public static string ToZ85Decoded(this string encoded) { return Decode(encoded, ZContext.Encoding); }

    [Obsolete]
    public static string ToZ85Decoded(this string encoded, Encoding encoding) { return Decode(encoded, encoding); }

    [Obsolete]
    public static byte[] ToZ85DecodedBytes(this string encoded) { return DecodeBytes(encoded, ZContext.Encoding); }

    [Obsolete]
    public static byte[] ToZ85DecodedBytes(this string encoded, Encoding encoding)
    {
      return DecodeBytes(encoded, encoding);
    }

    [Obsolete]
    public static string Decode(string strg) { return Decode(strg, ZContext.Encoding); }

    [Obsolete]
    public static string Decode(string strg, Encoding encoding)
    {
      byte[] encoded = DecodeBytes(strg, encoding);
      return encoding.GetString(encoded);
    }

    [Obsolete]
    public static byte[] DecodeBytes(string strg, Encoding encoding)
    {
      byte[] bytes = encoding.GetBytes(strg);
      return Decode(bytes);
    }
  }
}

