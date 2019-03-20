using System;
using System.Reflection;
using System.Runtime.Serialization;
namespace Sample.Hello.Common.Enumeration
{

    [DataContract]
    public class PostMarkDateTypes : EnumReplacement
    {
        public const int PostMarkDate = 0;
        public const int FormRequest = 1;

        public PostMarkDateTypes(int value) : base(value) { }
        public static implicit operator PostMarkDateTypes(int value)
        {
            return new PostMarkDateTypes(value);
        }
    }
}
