using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildScript {
    private static string KeystorePath => System.IO.Path.Combine(Application.dataPath, "../../keystore/creator.keystore");
    private const string KeystorePassword = "wnlrnajd12!@";
    private const string KeyAlias = "rathole";
    private const string KeyPassword = "wnlrnajd12!@";

    [MenuItem("Build/Build for iOS")]
    public static void BuildForiOS()
    {
        EditorApplication.delayCall += () => {
            AddressableAssetSettings.BuildPlayerContent();
            EditorApplication.delayCall += BuildiOSContent;
        };
    }

    private static void BuildiOSContent()
    {
        // 빌드 타겟 설정
        var buildTarget = BuildTarget.iOS;
        var buildTargetGroup = BuildTargetGroup.iOS;

        // 빌드 옵션 설정
        var buildOptions = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
        buildOptions |= BuildOptions.CompressWithLz4HC;

        // Keystore 정보 설정
        SetKeystoreSettings(buildTargetGroup);

        // 빌드 경로 설정
        var buildPath = "Builds/iOS/";

        // 심볼 추가
        AddScriptingDefineSymbol(buildTargetGroup, "ENABLE_LOG");

        // 빌드 설정
        var buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.targetGroup = buildTargetGroup;
        buildPlayerOptions.options = buildOptions;
        buildPlayerOptions.locationPathName = $"{buildPath}{PlayerSettings.productName}";

        // 빌드 실행
        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var buildSummary = buildReport.summary;
        var buildResult = buildSummary.result;
        if (buildResult == BuildResult.Succeeded) {
            UnityEngine.Debug.Log($"iOS build succeeded: {buildSummary.outputPath}");
        } else if (buildResult == BuildResult.Failed) {
            UnityEngine.Debug.LogError("iOS build failed!");
        }
    }

    [MenuItem("Build/Build for Android")]
    public static void BuildForAndroid()
    {
        EditorApplication.delayCall += () => {
            //EditorUserBuildSettings.buildAppBundle = true;
            // 심볼 추가
            //AddScriptingDefineSymbol(BuildTargetGroup.Android, "ENABLE_LOG");
            AddressableAssetSettings.BuildPlayerContent();
            EditorApplication.delayCall += BuildAndroidContent;
        };
    }

    private static void BuildAndroidContent()
    {
        // 빌드 타겟 설정
        var buildTarget = BuildTarget.Android;
        var buildTargetGroup = BuildTargetGroup.Android;

        // 빌드 옵션 설정
        var buildOptions = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
        buildOptions |= BuildOptions.CompressWithLz4HC;

        // Keystore 정보 설정
        SetKeystoreSettings(buildTargetGroup);

        // 빌드 경로 설정
        var buildPath = "Builds/Android/";

        // 빌드 설정
        var buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenePaths();
        buildPlayerOptions.target = buildTarget;
        buildPlayerOptions.targetGroup = buildTargetGroup;
        buildPlayerOptions.options = buildOptions;
        var extension = EditorUserBuildSettings.buildAppBundle ? "aab" : "apk";
        buildPlayerOptions.locationPathName = $"{buildPath}{PlayerSettings.productName}.{extension}";

        // 빌드 실행
        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        var buildSummary = buildReport.summary;
        var buildResult = buildSummary.result;
        if (buildResult == BuildResult.Succeeded) {
            UnityEngine.Debug.Log($"Android build succeeded: {buildSummary.outputPath}");
        } else if (buildResult == BuildResult.Failed) {
            UnityEngine.Debug.LogError("Android build failed!");
        }
    }

    private static void AddScriptingDefineSymbol(BuildTargetGroup buildTargetGroup, string symbol)
    {
        var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        if (!symbols.Contains(symbol)) {
            symbols += ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
        }
    }

    private static string[] GetScenePaths()
    {
        var scenes = EditorBuildSettings.scenes;
        var scenePaths = new string[scenes.Length];
        for (var i = 0; i < scenes.Length; i++) {
            scenePaths[i] = scenes[i].path;
        }

        return scenePaths;
    }

    private static void SetKeystoreSettings(BuildTargetGroup buildTargetGroup)
    {
        PlayerSettings.Android.keystoreName = KeystorePath;
        PlayerSettings.Android.keystorePass = KeystorePassword;
        PlayerSettings.Android.keyaliasName = KeyAlias;
        PlayerSettings.Android.keyaliasPass = KeyPassword;
    }
}
