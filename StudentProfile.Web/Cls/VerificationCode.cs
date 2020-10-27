using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dashboard_StudentProfile.Cls
{
    public class VerificationCode
    {
        private Random Random = new Random();
        private const string Chars = "0123456789";
        private const int DefaultVerificationCodeLength = 5;

        public string Generate(int length)
        {
            return new string(Enumerable.Repeat(Chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public string Generate() => Generate(DefaultVerificationCodeLength);
           
    }
}