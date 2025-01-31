using Firebase.Auth;
using Firebase;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Firebase.Firestore;
using System.Collections.Generic;

public class FirebaseAuthManager : MonoBehaviour
{
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    [Header("Registration UI")]
    [SerializeField] InputField regNameField;
    [SerializeField] InputField regEmailField;
    [SerializeField] InputField regPasswordField;
    [SerializeField] InputField regConfPasswordField;

    [Header("Login UI")]
    [SerializeField] InputField loginEmailField;
    [SerializeField] InputField loginPasswordField;

    private bool isFirebaseInitialized = false;
    private FirebaseFirestore db;

    private IEnumerator Start()
    {
        var checkDependenciesTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkDependenciesTask.IsCompleted);

        dependencyStatus = checkDependenciesTask.Result;
        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
            db = FirebaseFirestore.DefaultInstance;
            isFirebaseInitialized = true;
            Debug.Log("Firebase initialized!");
        }
        else
        {
            Debug.LogError($"Could not resolve dependencies: {dependencyStatus}");
        }
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != User)
        {
            User = auth.CurrentUser;
            if (User != null) Debug.Log($"User signed in: {User.DisplayName}");
        }
    }

    public void Register() => StartCoroutine(RegisterCoroutine());
    public void Login() => StartCoroutine(LoginCoroutine());

    private IEnumerator RegisterCoroutine()
    {
        // Validasi input
        if (regPasswordField.text != regConfPasswordField.text)
        {
            Debug.LogError("Password tidak sama!");
            yield break;
        }

        if (regPasswordField.text.Length < 6)
        {
            Debug.LogError("Password minimal 6 karakter!");
            yield break;
        }

        // Registrasi user
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(
            regEmailField.text.Trim(),
            regPasswordField.text
        );

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            HandleAuthError(registerTask.Exception, "Registrasi");
            yield break;
        }

        // Update profile dengan display name
        User = registerTask.Result.User;
        yield return UpdateUserProfile(regNameField.text.Trim());

        // Simpan data ke Firestore
        yield return SaveInitialUserData();
        Debug.Log("Registrasi berhasil!");
    }

    private IEnumerator LoginCoroutine()
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(
            loginEmailField.text.Trim(),
            loginPasswordField.text
        );

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            HandleAuthError(loginTask.Exception, "Login");
            yield break;
        }

        User = loginTask.Result.User;
        Debug.Log($"Login berhasil! Selamat datang {User.DisplayName}");

        // Load player data
        yield return LoadPlayerData();
    }

    private IEnumerator UpdateUserProfile(string displayName)
    {
        UserProfile profile = new UserProfile { DisplayName = displayName };
        var updateTask = User.UpdateUserProfileAsync(profile);
        yield return new WaitUntil(() => updateTask.IsCompleted);
    }

    private IEnumerator SaveInitialUserData()
    {
        DocumentReference userDoc = db.Collection("Akun").Document(User.DisplayName);

        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "email", User.Email },
            { "created_at", FieldValue.ServerTimestamp },
            { "level", 1 },
            { "coins", 100 },
            { "unlocked_skins", new List<string> { "default" } }
        };

        // Simpan data utama
        var setTask = userDoc.SetAsync(userData);
        yield return new WaitUntil(() => setTask.IsCompleted);

        // Inisialisasi skin
        CollectionReference skinsRef = userDoc.Collection("Skins");
        yield return InitializeDefaultSkins(skinsRef);
    }

    private IEnumerator InitializeDefaultSkins(CollectionReference skinsRef)
    {
        Dictionary<string, bool> defaultSkins = new Dictionary<string, bool>
        {
            { "Soul Blue", true },
            { "Wing of Justice", false },
            { "Space Craft", false },
            { "Core X56", false }
        };

        foreach (var skin in defaultSkins)
        {
            Dictionary<string, object> skinData = new Dictionary<string, object>
            {
                { "unlocked", skin.Value },
                { "equipped", false }
            };

            var setTask = skinsRef.Document(skin.Key).SetAsync(skinData);
            yield return new WaitUntil(() => setTask.IsCompleted);
        }
    }

    private IEnumerator LoadPlayerData()
    {
        DocumentReference userDoc = db.Collection("Akun").Document(User.DisplayName);
        var getTask = userDoc.GetSnapshotAsync();
        yield return new WaitUntil(() => getTask.IsCompleted);

        if (getTask.Result.Exists)
        {
            Debug.Log("Data pemain berhasil dimuat:");
            var data = getTask.Result.ToDictionary();
            foreach (var key in data.Keys)
            {
                Debug.Log($"{key}: {data[key]}");
            }
        }
        else
        {
            Debug.LogError("Data pemain tidak ditemukan!");
        }
    }

    public IEnumerator SavePlayerData(Dictionary<string, object> dataToSave)
    {
        DocumentReference userDoc = db.Collection("Akun").Document(User.DisplayName);
        var updateTask = userDoc.UpdateAsync(dataToSave);
        yield return new WaitUntil(() => updateTask.IsCompleted);

        if (updateTask.Exception == null)
            Debug.Log("Data berhasil disimpan!");
    }

    private void HandleAuthError(System.AggregateException exception, string operation)
    {
        FirebaseException firebaseEx = exception.GetBaseException() as FirebaseException;
        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

        string errorMessage = $"{operation} gagal: ";
        switch (errorCode)
        {
            case AuthError.EmailAlreadyInUse:
                errorMessage += "Email sudah terdaftar";
                break;
            case AuthError.WeakPassword:
                errorMessage += "Password terlalu lemah";
                break;
            case AuthError.WrongPassword:
                errorMessage += "Password salah";
                break;
            case AuthError.UserNotFound:
                errorMessage += "User tidak ditemukan";
                break;
            default:
                errorMessage += $"Error {errorCode}";
                break;
        }

        Debug.LogError(errorMessage);
    }

    private void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth = null;
        }
    }
}