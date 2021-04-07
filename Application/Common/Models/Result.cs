using System.Collections.Generic;
using System.Linq;

namespace Application.Common.Models
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }

        internal Result(IEnumerable<string> errors)
        {
            Succeeded = errors == null || !errors.Any();
            Errors = errors;
        }
        
        public static Result Success()
        {
            return new Result(new string[]{});
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(errors);
        }
    }
}