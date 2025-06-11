using System;
using System.Linq;
using System.Runtime.Serialization;
using Util;

namespace EntityService {
    using Vector2 = UnityEngine.Vector2;
    using Vector3 = UnityEngine.Vector3;

    public enum ValueType {
        Invalid,

        Bool,
        BoolArray,

        String,
        StringArray,
        StringArray2,

        StringIntArray2,

        Int,
        IntArray,
        IntRange,
        IntRangeArray,

        Float,
        FloatArray,
        FloatRange,
        FloatRangeArray,

        Vector2,
        Vector2Array,
        Vector3,
        Vector3Array,

        Enum,
        EnumArray,
        Time,
    }

    [Serializable]
    public struct ESProperty : ISerializable {
        public static readonly ESProperty Empty = new ESProperty();
        public object ValueObject { get; private set; }
        public ValueType Type {
            get {
                switch (ValueObject) {
                    case bool _: return ValueType.Bool;
                    case bool[] _: return ValueType.BoolArray;
                    case string _: return ValueType.String;
                    case string[] _: return ValueType.StringArray;
                    case string[][] _: return ValueType.StringArray2;
                    case int _: return ValueType.Int;
                    case Range<int> _: return ValueType.IntRange;
                    case Range<int>[] _: return ValueType.IntRangeArray;
                    case int[] _: return ValueType.IntArray;
                    case float _: return ValueType.Float;
                    case float[] _: return ValueType.FloatArray;
                    case Range<float> _: return ValueType.FloatRange;
                    case Range<float>[] _: return ValueType.FloatRangeArray;
                    case Vector2 _: return ValueType.Vector2;
                    case Vector2[] _: return ValueType.Vector2Array;
                    case Vector3 _: return ValueType.Vector3;
                    case Vector3[] _: return ValueType.Vector3Array;
                    case Enum _: return ValueType.Enum;
                    case Enum[] _: return ValueType.EnumArray;
                    case TimeSpan _: return ValueType.Time;
                    default: return ValueType.Invalid;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) {
                return false;
            }

            return obj is ESProperty prop && Equals(prop);
        }

        public override int GetHashCode()
        {
            return ValueObject.GetHashCode();
        }

        public static bool IsEmpty(ESProperty property)
        {
            return property.Type == ValueType.Invalid;
        }
        public bool Equals(ESProperty other)
        {
            if (Type != other.Type) {
                return false;
            }

            switch (Type) {
                case ValueType.Invalid:
                    return false;
                case ValueType.Bool:
                    return GetBool() == other.GetBool();
                case ValueType.Int:
                    return GetInt() == other.GetInt();
                case ValueType.Float:
                    return Math.Abs(GetFloat() - other.GetFloat()) < float.Epsilon;
                case ValueType.Enum:
                case ValueType.EnumArray:
                case ValueType.String:
                case ValueType.StringArray:
                case ValueType.StringArray2:
                case ValueType.IntArray:
                case ValueType.IntRange:
                case ValueType.IntRangeArray:
                case ValueType.FloatArray:
                case ValueType.FloatRange:
                case ValueType.FloatRangeArray:
                case ValueType.Vector3:
                case ValueType.Vector3Array:
                case ValueType.Vector2:
                case ValueType.Vector2Array:
                    return GetString() == other.GetString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool operator ==(ESProperty left, ESProperty right) { return left.Equals(right); }
        public static bool operator !=(ESProperty left, ESProperty right) { return !(left == right); }

        public static implicit operator ESProperty(bool value) { return new ESProperty(value); }
        public static implicit operator ESProperty(bool[] value) { return new ESProperty(value); }
        public static implicit operator ESProperty(int value) { return new ESProperty(value); }
        public static implicit operator ESProperty(int[] value) { return new ESProperty(value); }
        public static implicit operator ESProperty(float value) { return new ESProperty(value); }
        public static implicit operator ESProperty(float[] value) { return new ESProperty(value); }
        public static implicit operator ESProperty(string value) { return new ESProperty(value); }
        public static implicit operator ESProperty(string[] value) { return new ESProperty(value); }
        public static implicit operator ESProperty(Vector2 value) { return new ESProperty(value); }
        public static implicit operator ESProperty(Vector2[] value) { return new ESProperty(value); }
        public static implicit operator ESProperty(Vector3 value) { return new ESProperty(value); }
        public static implicit operator ESProperty(Vector3[] value) { return new ESProperty(value); }
        public static implicit operator ESProperty(Enum value) { return new ESProperty(value); }
        public static implicit operator ESProperty(Enum[] value) { return new ESProperty(value); }
        public static implicit operator bool(in ESProperty self) { return self.GetBool(); }
        public static implicit operator bool[](in ESProperty self) { return self.GetBoolArray(); }
        public static implicit operator int(in ESProperty self) { return self.GetInt(); }
        public static implicit operator int[](in ESProperty self) { return self.GetIntArray(); }
        public static implicit operator float(in ESProperty self) { return self.GetFloat(); }
        public static implicit operator float[](in ESProperty self) { return self.GetFloatArray(); }
        public static implicit operator string(in ESProperty self) { return self.GetString(); }
        public static implicit operator string[](in ESProperty self) { return self.GetStringArray(); }
        public static implicit operator Vector2(in ESProperty self) { return self.GetVector2(); }
        public static implicit operator Vector2[](in ESProperty self) { return self.GetVector2Array(); }
        public static implicit operator Vector3(in ESProperty self) { return self.GetVector3(); }
        public static implicit operator Vector3[](in ESProperty self) { return self.GetVector3Array(); }
        public static implicit operator Range<int>(in ESProperty self) { return self.GetIntRange(); }
        public static implicit operator Range<int>[](in ESProperty self) { return self.GetIntRangeArray(); }
        public static implicit operator Range<float>(in ESProperty self) { return self.GetFloatRange(); }
        public static implicit operator Range<float>[](in ESProperty self) { return self.GetFloatRangeArray(); }
        public static implicit operator Enum(in ESProperty self) { return self.GetEnum(); }
        public static implicit operator Enum[](in ESProperty self) { return self.GetEnumArray(); }

        public ESProperty(object rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(bool rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(bool[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(int rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(int[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Range<int> rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Range<int>[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(float rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(float[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Range<float> rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Range<float>[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(string rawValue)
        {
            if (string.IsNullOrEmpty(rawValue)) {
                throw new NullReferenceException();
            }

            if (float.TryParse(rawValue, out var outNumber)) {
                ValueObject = outNumber;
            } else {
                ValueObject = rawValue;
            }
        }
        public ESProperty(string[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Vector2 rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Vector2[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Vector3 rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Vector3[] rawValue)
        {
            ValueObject = rawValue;
        }
        public ESProperty(Enum rawValue)
        {
            ValueObject = rawValue;
        }

        public T Get<T>()
        {
            return (T)ValueObject;
        }

        public bool GetBool()
        {
            switch (ValueObject) {
                case bool bValue: return bValue;
            }

            return false;
        }

        public bool[] GetBoolArray()
        {
            switch (ValueObject) {
                case bool[] bValue: return bValue;
            }

            return Array.Empty<bool>();
        }

        public int GetInt()
        {
            switch (ValueObject) {
                case int iValue:
                    return iValue;
                case float fValue:
                    return (int)fValue;
                case Enum eValue:
                    return Convert.ToInt32(eValue);
            }
            return 0;
        }

        public int[] GetIntArray()
        {
            switch (ValueObject) {
                case int[] iValue:
                    return iValue;
            }

            return Array.Empty<int>();
        }

        public float GetFloat()
        {
            switch (ValueObject) {
                case int iValue:
                    return iValue;
                case float fValue:
                    return fValue;
            }
            return 0;
        }

        public float[] GetFloatArray()
        {
            switch (ValueObject) {
                case float[] fValue:
                    return fValue;
            }
            return Array.Empty<float>();
        }

        public string GetString()
        {
            switch (ValueObject) {
                case bool boolValue:
                    return boolValue.ToString();

                case int intValue:
                    return intValue.ToString();
                case int[] intArray:
                    return intArray.ToList().ToStringList();
                case Range<int> intRange:
                    return intRange.ToString();
                case Range<int>[] rangeIntArray:
                    return rangeIntArray.ToStringList();

                case float floatValue:
                    // ToString("F") 설명 https://docs.microsoft.com/ko-kr/dotnet/standard/base-types/standard-numeric-format-strings
                    return floatValue.ToString("F");
                case float[] floatArray:
                    return floatArray.ToList().ToStringList("F");
                case Range<float> floatRange:
                    return floatRange.ToString();
                case Range<float>[] rangeDoubleArray:
                    return rangeDoubleArray.ToStringList();

                case Vector2 vector2Value: {
                    var sb = StringFormatter.s_pool.Get();
                    try {
                        return vector2Value.ToStringFormat(sb);
                    } finally {
                        StringFormatter.s_pool.Return(sb);
                    }
                }
                case Vector2[] vector2Array:
                    return vector2Array.ToStringList();
                case Vector3 vector3Value: {
                    var sb = StringFormatter.s_pool.Get();
                    try {
                        return vector3Value.ToStringFormat(sb);
                    } finally {
                        StringFormatter.s_pool.Return(sb);
                    }
                }
                case Vector3[] vector3Array:
                    return vector3Array.ToStringList();

                case string stringValue:
                    return stringValue;
                case string[] stringValueArray:
                    return stringValueArray.ToStringList();
                case string[][] stringValueArray2:
                    return stringValueArray2.ToStringArray2();
                case Enum enumValue:
                    return enumValue.ToString();
            }

            return string.Empty;
        }

        public string[] GetStringArray()
        {
            switch (ValueObject) {
                case string[] sArrayValue:
                    return sArrayValue;
            }

            return Array.Empty<string>();
        }
        public Vector2 GetVector2()
        {
            switch (ValueObject) {
                case Vector2 v2Value:
                    return v2Value;
                case Vector3 v3Value:
                    return v3Value;
            }
            return default;
        }

        public Vector2[] GetVector2Array()
        {
            switch (ValueObject) {
                case Vector2[] v2ArrayValue:
                    return v2ArrayValue;
            }
            return Array.Empty<Vector2>();
        }

        public Vector3 GetVector3()
        {
            switch (ValueObject) {
                case Vector3 v3Value:
                    return v3Value;
            }
            return default;
        }

        public Vector3[] GetVector3Array()
        {
            switch (ValueObject) {
                case Vector3[] v3ArrayValue:
                    return v3ArrayValue;
            }
            return Array.Empty<Vector3>();
        }

        public Enum GetEnum()
        {
            switch (ValueObject) {
                case Enum eValue:
                    return eValue;
            }
            return default;
        }

        public Enum[] GetEnumArray()
        {
            switch (ValueObject) {
                case Enum[] eArrayValue:
                    return eArrayValue;
            }
            return default;
        }

        public EnumType GetEnum<EnumType>() where EnumType : struct, IConvertible
        {
            switch (ValueObject) {
                case EnumType eValue:
                    return eValue;
            }
            return default;
        }

        public EnumType[] GetEnumArray<EnumType>() where EnumType : struct, IConvertible
        {
            switch (ValueObject) {
                case EnumType[] eValue:
                    return eValue;
            }
            return Array.Empty<EnumType>();
        }

        public Range<int> GetIntRange()
        {
            switch (ValueObject) {
                case Range<int> riValue:
                    return riValue;
            }
            return Range<int>.Empty;
        }

        public Range<int>[] GetIntRangeArray()
        {
            switch (ValueObject) {
                case Range<int>[] riValue:
                    return riValue;
            }
            return Array.Empty<Range<int>>();
        }

        public Range<float> GetFloatRange()
        {
            switch (ValueObject) {
                case Range<float> rfValue:
                    return rfValue;
            }
            return Range<float>.Empty;
        }

        public Range<float>[] GetFloatRangeArray()
        {
            switch (ValueObject) {
                case Range<float>[] rfValue:
                    return rfValue;
            }
            return Array.Empty<Range<float>>();
        }

        public TimeSpan GetTimeSpan()
        {
            switch (ValueObject) {
                case TimeSpan tsValue:
                    return tsValue;
            }
            return default;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ValueObject), GetString());
        }

        private ESProperty(SerializationInfo info, StreamingContext context)
        {
            var rawValue = (string)info.GetValue(nameof(ValueObject), typeof(string));
            if (double.TryParse(rawValue, out var outDouble)) {
                ValueObject = outDouble;
            } else {
                ValueObject = rawValue;
            }
        }
    }
}