using System;

namespace DbSeeder.Model.Models
{
    public struct SeedItemJsonEntry
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }

        public bool IsRegex { get; set; }
        public string Regex { get; set; }

        public bool IsUnique { get; set; }

        public SeedItemJsonEntry(bool isRegex = false, bool isUnique = false) : this()
        {
            IsRegex = isRegex;
            IsUnique = isUnique;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                SeedItemJsonEntry otherObject = (SeedItemJsonEntry)obj;
                return 
                    (Name.Equals(otherObject.Name)) && 
                    (Type.Equals(otherObject.Type)) &&
                    (Value.Equals(otherObject.Value));
            }
        }
        
        // as https://www.loganfranken.com/blog/692/overriding-equals-in-c-part-2/
        public override int GetHashCode()
        {
            unchecked
            {
                const int HashingBase = (int)2166136261;
                const int HashingMultiplier = 16777619;

                int hash = HashingBase;
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, Name) ? Name.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, Type) ? Type.GetHashCode() : 0);
                hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, Value) ? Value.GetHashCode() : 0);
                return hash;
            }
        }

        public static bool operator ==(SeedItemJsonEntry left, SeedItemJsonEntry right)
        {
            if (ReferenceEquals(null, right)) return true;

            if (ReferenceEquals(null, left)) return false;

            return left.Equals(right);
        }

        public static bool operator !=(SeedItemJsonEntry left, SeedItemJsonEntry right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return string.Format($"{Name} - {Type} - {Value}");
        }
    }
}
