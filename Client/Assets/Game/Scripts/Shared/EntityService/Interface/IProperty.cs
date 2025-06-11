namespace EntityService {
    using System;
    using Vector3 = UnityEngine.Vector3;

    public interface IPropertyTable : IPropertyWrite, IPropertyRead { }

    public interface IPropertyRead {
        bool ContainsKey(string key);
        ESProperty GetProperty(string key);
        bool GetBool(string key, bool defaultValue = false);
        bool[] GetBoolArray(string key, bool[] defaultValue = null);
        int GetInt(string key, int defaultValue = 0);
        int[] GetIntArray(string key, int[] defaultValue = null);
        float GetFloat(string key, float defaultValue = 0f);
        float[] GetFloatArray(string key, float[] defaultValue = null);
        string GetString(string key, string defaultValue = null);
        string[] GetStringArray(string key, string[] defaultValue = null);
        Vector3 GetVector3(string key, in Vector3 defaultValue);
        Vector3[] GetVector3Array(string key, in Vector3[] defaultValue = null);
        TEnumType GetEnum<TEnumType>(string key, TEnumType defaultValue) where TEnumType : struct, IConvertible;
        TEnumType[] GetEnumArray<TEnumType>(string key, in TEnumType[] defaultValue) where TEnumType : struct, IConvertible;
        T Get<T>(string key);

    }

    public interface IPropertyWrite {
        void Set<T>(string key, T value);

        void ReplaceProperty(string key, ESProperty vaule);
    }
}