using System.Security.Cryptography;
using System.Text;

namespace Business.Crypto {
    public class Encryptor : IEncryptor {
        private string MD5Hash(string text) {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for(int i = 0; i < result.Length; i++) {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        #region IEncryptor
        string IEncryptor.MD5Hash(string text) => MD5Hash(text);
        #endregion
    }
}
