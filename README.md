# PlanetPlant

PlanetPlant is a Unity project. Basic game scripts can be found under `Client/Assets/Game`. Each stage is a planet where plants are fired from a cannon. Players clear a stage by planting within a limit and avoiding plant deaths.

## Automated Tests

The project uses the Unity Test Framework for automated tests. Test scripts are located under `Client/Assets/Tests`. To run edit mode tests from the command line:

```bash
unity -batchmode -projectPath Client -runTests -testPlatform editmode -testResults TestResults.xml -quit
```

Replace `unity` with the path to your Unity executable. Test results will be stored in `TestResults.xml`.
