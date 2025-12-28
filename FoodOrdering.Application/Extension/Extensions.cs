using FoodOrdering.Domain.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Extension
{
    public static class Extensions
    {        
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
