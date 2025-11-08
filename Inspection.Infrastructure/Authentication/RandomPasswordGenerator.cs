using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Inspection.Application.Common.Interfaces.Authentication;

namespace Inspection.Infrastructure.Authentication
{
    public class RandomPasswordGenerator:IRandomPasswordGenerator
    {

        public string GenerateSecurePassword(int length)
        {
            if (length < 4)
                throw new ArgumentException("Password length must be at least 4 for complexity.");

            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string symbols = "!@#$%^&*";

            string allChars = letters + digits + symbols;
            var password = new char[length];
            var rng = RandomNumberGenerator.Create();
            byte[] buffer = new byte[sizeof(uint)];

            // Ensure at least one letter, one digit, and one symbol
            password[0] = GetRandomChar(letters, rng, buffer);
            password[1] = GetRandomChar(digits, rng, buffer);
            password[2] = GetRandomChar(symbols, rng, buffer);

            // Fill the rest randomly
            for (int i = 3; i < length; i++)
            {
                password[i] = GetRandomChar(allChars, rng, buffer);
            }

            // Shuffle the password to avoid predictable placement
            return Shuffle(password, rng);
        }

        private char GetRandomChar(string charset, RandomNumberGenerator rng, byte[] buffer)
        {
            rng.GetBytes(buffer);
            uint num = BitConverter.ToUInt32(buffer, 0);
            return charset[(int)(num % (uint)charset.Length)];
        }

        private string Shuffle(char[] array, RandomNumberGenerator rng)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                byte[] buffer = new byte[sizeof(uint)];
                rng.GetBytes(buffer);
                uint num = BitConverter.ToUInt32(buffer, 0);
                int j = (int)(num % (uint)(i + 1));

                // Swap
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }

            return new string(array);
        }


        //    public string GenerateSecurePassword(int length )
        //{
        //    const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
        //    var res = new char[length];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        byte[] uintBuffer = new byte[sizeof(uint)];

        //        for (int i = 0; i < length; i++)
        //        {
        //            rng.GetBytes(uintBuffer);
        //            uint num = BitConverter.ToUInt32(uintBuffer, 0);
        //            res[i] = valid[(int)(num % (uint)valid.Length)];
        //        }
        //    }

        //    return new string(res);
        //}


    }
}
