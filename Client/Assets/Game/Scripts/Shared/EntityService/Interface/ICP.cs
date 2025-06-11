using System;
using System.Collections.Generic;

namespace EntityService {
    public interface ICP {
        event Action<string, ESProperty, ESProperty> PropertyValueChanged;
        IReadOnlyDictionary<string, ESProperty> Properties { get; }
        ESProperty GetProperty(string key);
        void AddCP(string key, CPDelegate cpmethod);
        void RemoveCP(string key);
        void RefreshCP();
        bool IsCP(string propNames);
    }
}
