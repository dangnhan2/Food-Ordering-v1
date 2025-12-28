using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class ValidationDictionaryException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationDictionaryException(IDictionary<string, string[]> errors)
        {
            Errors = errors;
        }

    }
}
