using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Extension
{
    public static class Extensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> values, int page, int pageSize) where T : class
        {
            return values.Skip((page -1) * pageSize).Take(pageSize);
        }

        public static string HashToken(this string token)
        {
            var sha256 = SHA256.Create();
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hashToken = Convert.ToBase64String(sha256.ComputeHash(tokenBytes));
            return hashToken;
        }

        public static string GenerateString(int size)
        {
            Random res = new Random();
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            StringBuilder sb = new StringBuilder(size);

            for (int i = 0; i < size; i++)
            {
                sb.Append(str[res.Next(str.Length)]);
            }

            return sb.ToString();
        }
    }
}
