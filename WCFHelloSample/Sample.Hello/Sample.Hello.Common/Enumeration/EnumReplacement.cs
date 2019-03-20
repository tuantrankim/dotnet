
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
namespace Sample.Hello.Common.Enumeration
{

    [DataContract]
    public class EnumReplacement : IEquatable<EnumReplacement>
    {
        [DataMember]
        private int _value;
        public EnumReplacement(int value)
        {
            _value = value;
        }

        public static implicit operator int(EnumReplacement value)
        {
            return value.Value;
        }

        public int Value { get { return _value; } set { _value = value; } }
        public bool Equals(EnumReplacement other)
        {
            if (other == null) return false;
            return this.Value == other.Value;
        }


        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            EnumReplacement o = obj as EnumReplacement;
            if (o == null)
                return false;
            else
                return Equals(o);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static bool operator ==(EnumReplacement lhs, EnumReplacement rhs)
        {
            if (((object)lhs) == null || ((object)rhs) == null)
                return Object.Equals(lhs, rhs);

            return lhs.Equals(rhs);
        }

        public static bool operator !=(EnumReplacement lhs, EnumReplacement rhs)
        {
            if (((object)lhs) == null || ((object)rhs) == null)
                return !Object.Equals(lhs, rhs);

            return !(lhs.Equals(rhs));
        }
        public override string ToString()
        {
            try
            {
                var t = this.GetType();
                foreach (FieldInfo fi in t.GetFields())
                {
                    if ((int)fi.GetRawConstantValue() == this.Value) return fi.Name;
                }
            }
            catch { }
            return "";
        }
    }
}
