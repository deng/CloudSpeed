using System;
using System.Collections.Generic;
using System.Text;

namespace CloudSpeed.Services
{
    public static class RandomService
    {
        static readonly Random _Global = new Random();
        [ThreadStatic]
        static Random _Local;

        public static void SetRandom(Random rnd)
        {
            _Local = rnd;
        }

        public static int Next()
        {
            return Do(r => r.Next());
        }

        public static int Next(int maxValue)
        {
            return Do(r => r.Next(maxValue));
        }

        public static int Next(int minValue, int maxValue)
        {
            return Do(r => r.Next(minValue, maxValue));
        }

        public static void NextBytes(byte[] buffer)
        {
            Do(r =>
            {
                r.NextBytes(buffer);
                return 0;
            });
        }

        public static double NextDouble()
        {
            return Do(r => r.NextDouble());
        }

        ///<summary>
        ///生成随机字符串 
        ///</summary>
        ///<param name="length">目标字符串的长度</param>
        ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
        ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        ///<returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            return Do(r =>
            {
                byte[] b = new byte[4];
                new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
                string s = null, str = custom;
                if (useNum == true) { str += "0123456789"; }
                if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
                if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
                if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
                for (int i = 0; i < length; i++)
                {
                    s += str.Substring(r.Next(0, str.Length - 1), 1);
                }
                return s;
            });
        }

        static T Do<T>(Func<Random, T> creator)
        {
            Random rnd = _Local;
            if (rnd == null)
            {
                int seed;
                lock (_Global)
                {
                    seed = _Global.Next();
                }
                _Local = rnd = new Random(seed);
            }
            return creator(rnd);
        }
    }
}
