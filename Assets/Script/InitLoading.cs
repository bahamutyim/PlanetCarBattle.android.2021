using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ahyim.planet
{
    public class InitLoading : MonoBehaviour
    {

        public UnityEngine.UI.Text messageText;
        public GameObject messagePanel;

        private string testMessage;
        private bool doneTesting = false;
        private bool useNat = false;
        private bool probingPublicIP = false;
        private string testStatus = "Testing network connection capabilities.";
        private string shouldEnableNatMessage = "";
        private int serverPort = 7777;
        private float timer = 0.0f;
        private ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;

        private Authorization myAuth;
        // Use this for initialization
        void Start()
        {

            

            //AppSetting.GenAppValuesJSON();
            Init();



        }
        public void Init()
        {
            Debug.Log("Start init");
            Debug.Log("get authroize");
            myAuth = GetComponent<Authorization>();

            Debug.Log("check network");
            messageText.text = "Checking Network Connection...";
            TestConnection();
            messageText.text += "\n" + testMessage;
            messageText.text = "Loading Config";

            AppSetting.LoadSetting(loadSettingCompleted);


            //Loadconfig();

            
        }
        //private void Loadconfig()
        //{
        //    Debug.Log("load config");

        //    AppSetting.LoadConfig().ContinueWith(
        //        (System.Threading.Tasks.Task iniTask) =>  {
        //                if ( iniTask.IsFaulted || iniTask.IsFaulted)
        //                {
        //                    ShowMessage(string.Format("Load Config error: {0}", iniTask.Exception.Message));
        //                    Debug.LogFormat("Load Config error: {0}", iniTask.Exception.Message);
        //                }
        //                else
        //                {
        //                    AppSetting.ActivateConfig();

        //                    if (AppSetting.StorageURL != null && AppSetting.StorageURL.Trim().Length > 0)
        //                    {
        //                        Debug.Log("load setting");
        //                        AppSetting.LoadSetting(loadSettingCompleted);
        //                    }
        //                    else
        //                    {
        //                        ShowMessage("Config setup is not completed, Please retry.");
        //                    }
        //                }


        //            }

        //        );

            

        //}
        
        private void loadSettingCompleted(bool loadSuccess, string exceptionMessage)
        {
            if (loadSuccess)
            {
                ChangeScreenResolution();
                login();
            }
            else
            {
                Debug.Log(exceptionMessage);
                messageText.text = string.Format("Load Setting Error: {0}", exceptionMessage);
            }
        }

        private void login()
        {
            Debug.Log("Try login");
            messageText.text = "Loading user information";
            Debug.LogFormat("AuthMethod : {0}", PlayerAuthInfo.AuthMethod);
            switch (PlayerAuthInfo.AuthMethod)
            {
                case "email":
                    Debug.Log("try email login");
                    messageText.text += "\nEmail User";
                    myAuth.EmailSigninUser(PlayerAuthInfo.EmailLogin, PlayerAuthInfo.EmailPassword);
                    break;

                case "facebook":
                    Debug.Log("try facebook login");
                    messageText.text += "\nFacebook User";
                    myAuth.FacebookSigninUser();
                    break;
                case "github":

                    break;

                case "google":

                    break;
                case "twitter":

                    break;
                case "anonymous":
                    messageText.text += "\nanonymous User";
                    myAuth.AnonymousLogin();
                    break;
                default:
                    Debug.Log("goto login menud");

                    MySceneManager.LoadSceneStatic("LoginMenu");
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        void TestConnection()
        {
            // Start/Poll the connection test, report the results in a label and
            // react to the results accordingly
            connectionTestResult = Network.TestConnection();
            switch (connectionTestResult)
            {
                case ConnectionTesterStatus.Error:
                    testMessage = "Problem determining NAT capabilities";
                    doneTesting = true;
                    break;

                case ConnectionTesterStatus.Undetermined:
                    testMessage = "Undetermined NAT capabilities";
                    doneTesting = false;
                    break;

                case ConnectionTesterStatus.PublicIPIsConnectable:
                    testMessage = "Directly connectable public IP address.";
                    useNat = false;
                    doneTesting = true;
                    break;

                // This case is a bit special as we now need to check if we can
                // circumvent the blocking by using NAT punchthrough
                case ConnectionTesterStatus.PublicIPPortBlocked:
                    testMessage = "Non-connectable public IP address (port " +
                        serverPort + " blocked), running a server is impossible.";
                    useNat = false;
                    // If no NAT punchthrough test has been performed on this public
                    // IP, force a test
                    if (!probingPublicIP)
                    {
                        connectionTestResult = Network.TestConnectionNAT();
                        probingPublicIP = true;
                        testStatus = "Testing if blocked public IP can be circumvented";
                        timer = Time.time + 10;
                    }
                    // NAT punchthrough test was performed but we still get blocked
                    else if (Time.time > timer)
                    {
                        probingPublicIP = false;        // reset
                        useNat = true;
                        doneTesting = true;
                    }
                    break;

                case ConnectionTesterStatus.PublicIPNoServerStarted:
                    testMessage = "Public IP address but server not initialized, " +
                        "it must be started to check server accessibility. Restart " +
                        "connection test when ready.";
                    break;

                case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
                    testMessage = "Limited NAT punchthrough capabilities. Cannot " +
                        "connect to all types of NAT servers. Running a server " +
                        "is ill advised as not everyone can connect.";
                    useNat = true;
                    doneTesting = true;
                    break;

                case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
                    testMessage = "Limited NAT punchthrough capabilities. Cannot " +
                        "connect to all types of NAT servers. Running a server " +
                        "is ill advised as not everyone can connect.";
                    useNat = true;
                    doneTesting = true;
                    break;

                case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
                case ConnectionTesterStatus.NATpunchthroughFullCone:
                    testMessage = "NAT punchthrough capable. Can connect to all " +
                        "servers and receive connections from all clients. Enabling " +
                        "NAT punchthrough functionality.";
                    useNat = true;
                    doneTesting = true;
                    break;

                default:
                    testMessage = "Error in test routine, got " + connectionTestResult;
                    break;
            }
            Debug.Log(testMessage);
            if (doneTesting)
            {
                if (useNat)
                    shouldEnableNatMessage = "When starting a server the NAT " +
                        "punchthrough feature should be enabled (useNat parameter)";
                else
                    shouldEnableNatMessage = "NAT punchthrough not needed";
                testStatus = "Done testing";
            }
        }
        public void ShowMessage(string message)
        {
            UnityEngine.UI.Text messageText = messagePanel.transform.Find("Message").GetComponent<UnityEngine.UI.Text>();
            if (messageText)
            {
                messageText.text = message;
                messagePanel.SetActive(true);
            }
        }
        public void ShowMessage(string message, params object[] args)
        {
            ShowMessage(string.Format(message, args));

        }
        public void HideMessage()
        {
            messagePanel.SetActive(false);
        }

        private void ChangeScreenResolution()
        {
            Resolution currResultion = Screen.currentResolution;

            int width = 960;

            if (Int32.TryParse(AppSetting.ScreenWidth, out width))
            {
                width = 960;
            }
            
            int height = width * currResultion.height / currResultion.width;
            Debug.LogFormat("Screen size set to {0}x{1}", width, height);
            Screen.SetResolution(width, height, true, 30);


        }
    }

}