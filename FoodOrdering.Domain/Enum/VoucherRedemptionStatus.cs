using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Enum
{
    public enum VoucherRedemptionStatus
    {       
        Pending,      
        Used,     
        Cancelled,
    }
}
