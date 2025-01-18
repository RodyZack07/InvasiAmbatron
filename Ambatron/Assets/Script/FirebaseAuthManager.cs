using Firebase.Auth;
using Firebase;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FirebaseAuthManager : MonoBehaviour
{
    // Firebase Var
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Space]
    [Header("login")]
    public InputField emailField;
    public InputField passwordField;


    [Space]
    [Header("login")]
    public InputField regNameField;
    public InputField regEmailField;
    public InputField regPasswordField;
    public InputField regConfPassword;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {

                InitializeFirebase();
                
            }
        });
    }

    void InitializeFirebase() {

        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs) {

        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null) {

                Debug.Log("Signed out" + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn) {
                Debug.Log("Signed in" + user.UserId);
            }
        }
    
    }

    public  void Login() {

        StartCoroutine(LoginAsync(emailField.text, passwordField.text));

    }

    private IEnumerator LoginAsync(string email, string password) {

        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMassage = "Login Error";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMassage += "Email Invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMassage += "Password is wrong";
                    break;
                case AuthError.MissingEmail:
                    failedMassage += "Email Missing";
                    break;
                case AuthError.MissingPassword:
                    failedMassage += "Password Missing";
                    break;
            }
            Debug.Log(failedMassage);
        }
        else {
            user = loginTask.Result.User;

            Debug.LogFormat("{0} Login Success", user.DisplayName);
        }

    }

    public void Register() {

        StartCoroutine(RegisterAsync(regNameField.text, regEmailField.text, regPasswordField.text, regConfPassword.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (name == "")
        {
            Debug.LogError("User Name is empty");
        }
        else if (email == "")
        {
            Debug.LogError("email field is empty");
        }
        else if (regPasswordField.text != regConfPassword.text)
        {
            Debug.LogError("Password does not match");
        }
        else {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);


            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Result);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed ! Because";

                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        break;
                    default:
                        failedMessage = "Registration Failed";
                        break;

                }
            }
            else {
                user = registerTask.Result.User;

                UserProfile userProfile = new UserProfile { DisplayName = name };

                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Delete the user if user update failed
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    string failedMessage = "Profile update Failed! Becuase ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            break;
                        default:
                            failedMessage = "Profile update Failed";
                            break;
                    }

                    Debug.Log(failedMessage);
                }
              
            }
        }

    }

    }
