using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace AlpacaTech
{
    /// <summary>
    /// ジェネリック＋シリアライズ＋XML＋暗号化でファイル読み書き
    /// </summary>
    public class SerializerJson
    {
        //	IV設定
        static public int BLOCK_SIZE = 128;   // 128bit 固定
        static public int KEY_SIZE = 128;     // 128/192/256bit から選択
        static public string cryptoIV = "DRKgYZrkikjxrOJf";

        //----------------------------------------------------------------------
        //	ファイル
        //----------------------------------------------------------------------
        /// <summary>
        /// ファイルに保存
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="data">データ</param>
        /// <param name="key">暗号化キー。null指定の場合はJson保存</param>
        public static void SaveToFile<Type>(string filePath, Type data, string key = null)
        {
            if (key == null)
            {
                //	XML
                using (var sw = new StreamWriter(filePath, false, new System.Text.UTF8Encoding(false)))
                {
                    //	シリアル化
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Type));
                    serializer.Serialize(sw, data);
                }
            }
            else
            {
                //  XML + AES
                using (var aes = new AesManaged())
                {
                    aes.BlockSize = BLOCK_SIZE;
                    aes.KeySize = KEY_SIZE;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = toByteArray(key);
                    aes.IV = toByteArray(cryptoIV);
                    var encryptor = aes.CreateEncryptor();

                    using (var sw = new FileStream(filePath, FileMode.Create))
                    using (var cs = new CryptoStream(sw, encryptor, CryptoStreamMode.Write))
                    {
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Type));
                        serializer.Serialize(cs, data);
                    }
                }
            }
        }

        /// <summary>
        /// ファイルから読み込み
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="key">暗号化キー。指定ない場合はXML</param>
        /// <returns></returns>
		public static Type LoadFromFile<Type>(string filePath, string key = null) where Type : new()
        {
            if (key == null)
            {
                //  XML
                using (var sr = new StreamReader(filePath, new System.Text.UTF8Encoding(false)))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Type));
                    var data = (Type)serializer.Deserialize(sr);
                    return data;
                }
            }
            else
            {
                //  XML+AES
                using (var aes = new AesManaged())
                {
                    aes.BlockSize = BLOCK_SIZE;
                    aes.KeySize = KEY_SIZE;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = toByteArray(key);
                    aes.IV = toByteArray(cryptoIV);
                    var decryptor = aes.CreateDecryptor();
                    using (var fs = new FileStream(filePath, FileMode.Open))
                    using (var cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read))
                    {
                        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Type));
                        var data = (Type)serializer.Deserialize(cs);
                        return data;
                    }
                }
            }
        }

        /// <summary>
        /// 文字列を16byteの配列に変換
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        static byte[] toByteArray(string password)
        {
            byte[] bufferKey = new byte[16];
            byte[] bufferPassword = Encoding.UTF8.GetBytes(password);
            for (int i = 0; i < bufferKey.Length; i++)
            {
                if (i < bufferPassword.Length)
                {
                    bufferKey[i] = bufferPassword[i];
                }
                else
                {
                    bufferKey[i] = 0;
                }
            }
            return bufferKey;
        }

    }


}
