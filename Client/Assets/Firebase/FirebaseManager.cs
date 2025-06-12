using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FirebaseManager : MonoBehaviour {
    private static FirebaseManager _instance;
    public static FirebaseManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<FirebaseManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (_instance != this) {
            Destroy(gameObject);
            return;
        }

        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted) {
                GameLogger.Log.Error("FirebaseManager", "Failed to check Firebase dependencies: " + task.Exception);
                return;
            }

            FirebaseApp.Create();

            // Firebase Analytics 초기화
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            SetAnonymousUserId();
            GameLogger.Log.Info("FirebaseManager", "Firebase initialized successfully.");
        });
    }


    public void SetAnonymousUserId()
    {
        //var deviceIdentifier = AdManager.Inst.DeviceUniqueIdentifier();
        //var anonymousUserId = HashString(deviceIdentifier);

        //FirebaseAnalytics.SetUserId(anonymousUserId);
    }

    private string HashString(string input)
    {
        using (var hasher = new SHA256Managed()) {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashedBytes = hasher.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var b in hashedBytes) {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}