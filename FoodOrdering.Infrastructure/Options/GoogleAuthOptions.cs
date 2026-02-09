using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Options
{
    public sealed class GoogleAuthOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
