using System.Collections;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine;
using GameLogger;

class TestCannonInput : ICannonInput
{
    public bool NextPress;

    public bool FirePressed()
    {
        if (NextPress)
        {
            NextPress = false;
            return true;
        }
        return false;
    }
}

public class GamePlayTests
{
    [UnityTest]
    public IEnumerator PlantingTreesWithInputClearsStage()
    {
        // capture logs for later analysis
        var logger = new TestGameLogger();
        Log.Init(logger);
        Log.Info("PlayModeTest", "Test start", LogColor.Orange);

        SceneManager.LoadScene("GameScene");
        yield return null; // wait for scene load

        var gameManager = GameManager.Inst;
        Assert.IsNotNull(gameManager, "GameManager not found");

        var planetManager = gameManager.PlanetManager;
        Assert.IsNotNull(planetManager, "PlanetManager not found");

        // ensure planet created
        planetManager.CreatePlanet(planetManager.LastPlanet, false);
        yield return new WaitForSeconds(0.5f);

        var planet = planetManager.Planet;
        Assert.IsNotNull(planet, "Planet not created");

        var testInput = new TestCannonInput();
        planet.Cannon.InputHandler = testInput;

        // fire plants via input until game ends or no plants remain
        int safety = 20;
        while (!planet.IsEndGame && planet.Cannon.RemainingPlants > 0 && safety-- > 0)
        {
            testInput.NextPress = true;
            yield return null; // allow Update to process input
            Log.Info("PlayModeTest", $"Fired plant. Remaining: {planet.Cannon.RemainingPlants}", LogColor.Green);
            yield return new WaitForSeconds(1f);
        }

        if (planet.IsEndGame)
        {
            Log.Info("PlayModeTest", $"Stage cleared with star: {planet.Info.Star}", LogColor.Cyan);
        }
        else
        {
            Log.Error("PlayModeTest", "Stage failed to clear within safety limit", LogColor.Red);
        }

        Log.Info("PlayModeTest", $"Total logs: {logger.Logs.Count}, Errors: {logger.ErrorLogs.Count}", LogColor.Silver);

        // Persist the log locally so no upload is needed
        var logPath = System.IO.Path.Combine(Application.persistentDataPath, "GamePlayTestLog.txt");
        logger.SaveToFile(logPath);

        Assert.IsTrue(planet.IsEndGame, "Stage was not cleared");
        Assert.Greater(planet.Info.Star, 0, "No stars awarded");
    }
}
