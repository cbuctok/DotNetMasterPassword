using MasterPassword.Crypto;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MasterPassword.Core
{
    /// <summary>
    /// <para>contains the core algorithm for Master Password</para>
    /// <para>
    /// usage:
    ///  var masterkey = Algorithm.CalcMasterKey(userName, masterPassword);
    ///  var templateSeed = Algorithm.CalcTemplateSeed(masterkey, siteName, counter);
    ///  var generatedPassword = Algorithm.CalcPassword(templateSeed, type);
    /// </para>
    /// <para>http://masterpasswordapp.com/algorithm.html</para>
    /// </summary>
    public static class Algorithm
    {
        /// <summary>
        /// Template for the password type. Each letter in the template stands for a character group.
        /// </summary>
        public static Dictionary<PasswordType, string[]> TemplateForType = new Dictionary<PasswordType, string[]>
        {
            { PasswordType.MaximumSecurityPassword, new string[] { "anoxxxxxxxxxxxxxxxxx", "axxxxxxxxxxxxxxxxxno"} },
            { PasswordType.LongPassword, new string[]
            {
                "CvcvnoCvcvCvcv",
                "CvcvCvcvnoCvcv",
                "CvcvCvcvCvcvno",
                "CvccnoCvcvCvcv",
                "CvccCvcvnoCvcv",
                "CvccCvcvCvcvno",
                "CvcvnoCvccCvcv",
                "CvcvCvccnoCvcv",
                "CvcvCvccCvcvno",
                "CvcvnoCvcvCvcc",
                "CvcvCvcvnoCvcc",
                "CvcvCvcvCvccno",
                "CvccnoCvccCvcv",
                "CvccCvccnoCvcv",
                "CvccCvccCvcvno",
                "CvcvnoCvccCvcc",
                "CvcvCvccnoCvcc",
                "CvcvCvccCvccno",
                "CvccnoCvcvCvcc",
                "CvccCvcvnoCvcc",
                "CvccCvcvCvccno"
            } },
            { PasswordType.MediumPassword, new string[] { "CvcnoCvc", "CvcCvcno"} },
            { PasswordType.ShortPassword, new string[] { "Cvcn"} },
            { PasswordType.BasicPassword, new string[] { "aaanaaan", "aannaaan", "aaannaaa"} },
            { PasswordType.PIN, new string[] { "nnnn"} }
        };

        public static Dictionary<char, string> CharacterGroup { get; set; } = new Dictionary<char, string>
        {
            ['V'] = "AEIOU",
            ['C'] = "BCDFGHJKLMNPQRSTVWXYZ",
            ['v'] = "aeiou",
            ['c'] = "bcdfghjklmnpqrstvwxyz",
            ['A'] = "AEIOUBCDFGHJKLMNPQRSTVWXYZ",
            ['a'] = "AEIOUaeiouBCDFGHJKLMNPQRSTVWXYZbcdfghjklmnpqrstvwxyz",
            ['n'] = "0123456789",
            ['o'] = "@&%?,=[]_:-+*$#!'^~;()/.",
            ['x'] = "AEIOUaeiouBCDFGHJKLMNPQRSTVWXYZbcdfghjklmnpqrstvwxyz0123456789!@#$%^&*()"  // Typo in spec, stated X instead of x
        };

        /// <summary>
        /// Calculates the master key.
        /// </summary>
        /// <returns>The master key.</returns>
        /// <param name="userName">User name.</param>
        /// <param name="masterPassword">Master password.</param>
        public static byte[] CalcMasterKey(string userName, string masterPassword)
        {
            var result = new byte[64];

            var masterBytes = UTF8Encoding.UTF8.GetBytes(masterPassword);
            var salt = Combine(UTF8Encoding.UTF8.GetBytes("com.lyndir.masterpassword"), GetBytes(userName.Length), UTF8Encoding.UTF8.GetBytes(userName));
            SCrypt.ComputeKey(
                masterBytes,
                salt,
                32768,
                8,
                2,
                64,
                result
            );

            return result;
        }

        /// <summary>
        /// Calculate the password for the site based on the password type.
        /// </summary>
        /// <returns>The site password.</returns>
        /// <param name="templateSeedForPassword">Template seed for password.</param>
        /// <param name="typeOfPassword">Type of password.</param>
        public static string CalcPassword(byte[] templateSeedForPassword, PasswordType typeOfPassword)
        {
            string[] templates = TemplateForType[typeOfPassword];
            string template = templates[templateSeedForPassword[0] % templates.Length];

            char[] password = new char[template.Length];

            for (int i = 0; i < password.Length; i++)
            {
                string characterGroup = CharacterGroup[template[i]]; // get charactergroup from template
                password[i] = characterGroup[templateSeedForPassword[i + 1] % characterGroup.Length];
            }

            return new string(password); // convert character array to string
        }

        /// <summary>
        /// Calculates the template seed for a site. The template seed is essentially the site-specific secret in binary form.
        /// </summary>
        /// <returns>The template seed.</returns>
        /// <param name="masterKey">Master key.</param>
        /// <param name="siteName">Site name.</param>
        /// <param name="counter">Counter.</param>
        public static byte[] CalcTemplateSeed(byte[] masterKey, string siteName, int counter)
        {
            var hash = new HMACSHA256(masterKey);

            return hash.ComputeHash(
                Combine(
                    Encoding.UTF8.GetBytes("com.lyndir.masterpassword"),
                    GetBytes(siteName.Length),
                    Encoding.UTF8.GetBytes(siteName),
                    GetBytes(counter)));
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            int len = 0;
            for (int i = 0; i < arrays.Length; i++)
                len += arrays[i].Length;

            byte[] rv = new byte[len];
            int offset = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                System.Buffer.BlockCopy(arrays[i], 0, rv, offset, arrays[i].Length);
                offset += arrays[i].Length;
            }
            return rv;
        }

        private static byte[] GetBytes(int value) => GetBytes((uint)value);

        private static byte[] GetBytes(uint value)
        {
            //32-bit unsigned integers in network byte order. (big endian)
            var result = new byte[4];
            result[0] = (byte)((value >> 24) & 0xFFu);
            result[1] = (byte)((value >> 16) & 0xFFu);
            result[2] = (byte)((value >> 8) & 0xFFu);
            result[3] = (byte)(value & 0xFFu);

            return result;
        }
    }
}