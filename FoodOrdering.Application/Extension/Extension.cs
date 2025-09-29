using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Extension
{
    public static class Extension
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> values, int page, int pageSize) where T : class
        {
            return values.Skip((page -1) * pageSize).Take(pageSize);
        }
    }
}
