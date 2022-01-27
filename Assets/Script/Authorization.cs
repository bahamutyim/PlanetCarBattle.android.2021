using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

using Firebase.Auth;
#if UNITY_ANDROID || UNITY_IOS
using Facebook.Unity;
#endif

namespace com.ahyim.planet
{


    public class Authorization : MonoBehaviour
    {

        public Text emailText;
        public Text passwordText;

        public Text playerName;

        private AuthorMenu authorMenu;
        private NewPlayerMenu newPlayerMenu;
        private MySceneManager mySceneManager;

        

        // Use this for initialization
        void Start()
        {
            //AppSetting.GenAppValuesJSON();

            authorMenu = GetComponent<AuthorMenu>();
            mySceneManager = GetComponent<MySceneManager>();
            newPlayerMenu = GetComponent<NewPlayerMenu>();
            if (PlayerAuthInfo.AuthMethod == "email")
            {
               //internalEmailSignin(playerSetting.EmailLogin, playerSetting.EmailPassword);
            }

            Debug.Log("Start Load Setting");
            
            

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EmailCreateUser()
        {

            string email = emailText.text;
            string password = GetMD5Hash( passwordText.text);


            Debug.LogFormat("email={0},password={1}", email, password);
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(
                task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync was cancled.");
                        authorMenu.ShowMessage("CreateUserWithEmailAndPasswordAsync was cancled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered and error:" + task.Exception);
                        authorMenu.ShowMessage("CreateUserWithEmailAndPasswordAsync encountered and error:" + task.Exception);
                        return;
                    }

                    // Firebase user has been created.
                    FirebaseUser newUser = task.Result;
                    AppParameters.User = newUser;
                    Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                    PlayerAuthInfo.AuthMethod = "email";
                    PlayerAuthInfo.EmailLogin = email;
                    PlayerAuthInfo.EmailPassword = password;
                    PlayerAuthInfo.UserUID = newUser.UserId;
                    //Debug.LogFormat("User saved to playerSetting, wait 3 secound to Main menu");

                    
                    LoadNewPlayerMenu();



                }


                );

        }

        public void EmailSigninUser()
        {
            string email = emailText.text;
            string password = GetMD5Hash(passwordText.text);

            EmailSigninUser(email, password);
           



        }

        public void EmailSigninUser(string email, string password)
        {

            PlayerAuthInfo.AuthMethod = "email";
            PlayerAuthInfo.EmailLogin = email;
            PlayerAuthInfo.EmailPassword = password;
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(SignInTask);

        }


        private void SignInTask(Task<FirebaseUser> task)
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInTask was canceled.");
                if (authorMenu != null)
                {
                    authorMenu.ShowMessage("SignInTask was canceled.");
                    PlayerAuthInfo.AuthMethod = "";
                }
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInTask encountered an error: " + task.Exception);
                if (authorMenu != null)
                {
                    authorMenu.ShowMessage("SignInTask encountered an error: " + task.Exception);
                    PlayerAuthInfo.AuthMethod = "";
                }
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;

            
            PlayerAuthInfo.UserUID = newUser.UserId;

            AppParameters.User = newUser;
            

            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            //FireBaseStorage.WriteUserFile("test/login.txt", System.Text.ASCIIEncoding.Default.GetBytes("User Logined.")  );

            ///Try to Load current player from database, if data not exit init current player information and save it to database
            Player.LoadCurrPlayer(
                    haveRecord =>
                    {
                        if (haveRecord)
                        {
                            LoadMainMenu();
                        }
                        else
                        {
                            LoadNewPlayerMenu();
                        }
                    }

                );

           
            
        }


        /// <summary>
        /// if mySceneManager exist, call load screen 
        /// </summary>
        private void LoadMainMenu()
        {
            if (mySceneManager != null)
            {
                mySceneManager.StartLoadNextScreen("MainMenu2");
            }
            else
            {
                MySceneManager.LoadSceneStatic("MainMenu2");
            }
        }

        private void LoadNewPlayerMenu()
        {
            if (mySceneManager != null)
            {
                mySceneManager.StartLoadNextScreen("NewPlayerMenu");
            }
            else
            {
                MySceneManager.LoadSceneStatic("NewPlayerMenu");
            }
        }

        private void SignIn(Credential credential)
        {
            Debug.Log("Start Firebase Signin");
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.SignInWithCredentialAsync(credential).ContinueWith(SignInTask);
        }



        public void GoogleSiginUser()
        {
#if UNITY_ANDROID || UNITY_IOS
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            //string googleIdToken = GooglePlayGames.PlayGamesPlatform.Instance.GetIdToken();

            //GooglePlayGames.BasicApi.PlayGamesClientConfiguration config = new GooglePlayGames.BasicApi.PlayGamesClientConfiguration.Builder()
            //    .RequestIdToken()
            //    .Build();
            //GooglePlayGames.PlayGamesPlatform.InitializeInstance(config);
            //GooglePlayGames.PlayGamesPlatform.Activate();
            //Social.localUser.Authenticate(AuthenticateCallBack);
            //string googleIdToken = ((GooglePlayGames.PlayGamesLocalUser)Social.localUser).mPlatform.GetIdToken();

            //authorMenu.ShowMessage("googleIdToken: " + googleIdToken);
            //Firebase.Auth.Credential credential =
            //Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, null);

            //SignIn(credential);
#endif
#if UNITY_STANDALONE
            throw new NotImplementedException();

#endif

        }
        public void FacebookSigninUser()
        {
#if UNITY_ANDROID || UNITY_IOS
           PlayerAuthInfo.AuthMethod = "facebook";
            if (!Facebook.Unity.FB.IsInitialized)
            {
                Facebook.Unity.FB.Init(FBInitCompleted);
            }
            else
            {
                Facebook.Unity.FB.ActivateApp();
                FBInitCompleted();
            }
#endif
#if UNITY_STANDALONE
            throw new NotImplementedException();

#endif

        }
#if UNITY_ANDROID || UNITY_IOS
        private void FBInitCompleted()
        {
            Facebook.Unity.FB.LogInWithReadPermissions(callback: OnLogIn);

            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            string tokenStr = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
            Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(tokenStr);
            
            SignIn(credential);
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        private void OnLogIn(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                AccessToken fbToken = AccessToken.CurrentAccessToken;
                Credential credential = FacebookAuthProvider.GetCredential(fbToken.TokenString);
                SignIn(credential);
            }
        }
#endif
        public void AnonymousLogin()
        {

            PlayerAuthInfo.AuthMethod = "anonymous";
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.SignInAnonymouslyAsync().ContinueWith(SignInTask);

        }

        public void SignOut()
        {

            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            auth.SignOut();

        }

        private string GetMD5Hash(string input)
        {

            MD5 md5Hash = MD5.Create();
           //Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            //Create a new StringBuilder to collect the bytes and create a string.
            StringBuilder builder = new StringBuilder();

            //Loop through each byte of the hashed data and format each one as a hexadecimal strings.
            for (int cnt = 0; cnt < data.Length; cnt++)
            {
                builder.Append(data[cnt].ToString("x2"));
            }

            //Return the hexadecimal string
            return builder.ToString().ToUpper();
        }

        public void CreatePlayer()
        {


            string name = playerName.text;
            if (name != null && name.Trim().Length > 0)
            {

                Player.InitPlayer(name);
                Player currPlayer = Player.CurrentPlayer;
                Debug.LogFormat("Save Player:{0}", name);
                currPlayer.SavePlayer(
                    playerTask =>
                    {
                        if (playerTask.IsCanceled || playerTask.IsFaulted)
                        {
                            Debug.LogFormat("Player save issue:{0}", playerTask.Exception.Message);
                        }
                        else
                        {
                            Debug.LogFormat("Player {0} save successful!", currPlayer.ToString());
                            LoadMainMenu();
                        }



                    }
                    );
            }
            else
            {
                newPlayerMenu.ShowMessage("Please input Player Name.");
            }

        }
    } 
}
