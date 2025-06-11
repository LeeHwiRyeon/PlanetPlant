using EntityService;
using System;

public class PlanetProperties : ESObject {
}

public partial class PlanetManager {
    private PlanetProperties m_data = new PlanetProperties();

    public int Score {
        get => m_data.GetInt(PropNames.Score);
        set => m_data.Set(PropNames.Score, value);
    }

    public string LastPlanet {
        get => m_data.GetString(PropNames.LastPlanet, "Planet_1");
        set => m_data.Set(PropNames.LastPlanet, value);
    }

    public int GetPlanetStar(string name)
    {
        return m_data.GetInt(name);
    }

    public int SetPlanetStar(string name)
    {
        return m_data.GetInt(name);
    }

    public void AddPropertyValueChanged(Action<string, ESProperty, ESProperty> action)
    {
        m_data.PropertyValueChanged += action;
    }
    public void RemovePropertyValueChanged(Action<string, ESProperty, ESProperty> action)
    {
        m_data.PropertyValueChanged -= action;
    }
}
