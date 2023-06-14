using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

using Proyecto26;
using FullSerializer;
using TMPro;
using Newtonsoft.Json;

public class FirebaseDB : MonoBehaviour
{
    // register Variables
    [Space]
    [Header("Register")]
    public TMP_InputField firstName;
    public TMP_InputField middleName;
    public TMP_InputField lastName;
    public TMP_InputField age;
    public TMP_Dropdown sex;
    public TMP_InputField email;
    
    public TMP_InputField IDNumber;
    public TMP_InputField Username;
    public TMP_InputField Pass;
    public TMP_InputField ConfirmPass;
    public TMP_Text NewWelcomePrompt;
    public TMP_Text AccountExistsPrompt;


    public TMP_Text signupErrorMessage;

   // Login Variables
    [Space]
    [Header("Login")]
    public TMP_InputField LoginEmail;
    public TMP_InputField LoginPass;
    public TMP_Text WelcomePrompt;
    public TMP_Text LoginErrorMessage;

    private User localUser;
    private string userID, existingUserName;
    private Animator canvasAnimator;

    public static fsSerializer serializer = new fsSerializer();
    public static string tempUserHeader;
    private string AuthKey = "AIzaSyD4qpx35N7ULIfazz9ckli9QZsM9l3H-eY";
    private string databaseURL = "https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/";
    public static string localId;
    private string idToken;
    public TMP_Text debugText;

    private void Awake(){
        debugText.text = "Awake";
        canvasAnimator = GameObject.Find("/Canvas").GetComponent<Animator>();
        if(tempUserHeader==null)
            tempUserHeader = "custillano";
    }

    void Start()
    {
        localUser =  new User(
                            email: "",
                            username: "",
                            firstName: "",
                            middleName: "",
                            lastName: "",
                            age: 0,
                            sex: "",
                            isFaculty: false,
                            classCode: ""
                        );
    }

    //posts credentials to realtime database
    // use PostToDatabase(idNumber)
    private void PostToDatabase( string idTokenTemp = "")
    {
        debugText.text = "PostToDatabase";
        if (idTokenTemp == "")
        {
            idTokenTemp = idToken;
        }

        User user = new User(
            // Set the values for the User object properties
            email: email.text,
            username: Username.text,
            firstName: firstName.text,
            middleName: middleName.text,
            lastName: lastName.text,
            age: int.Parse(age.text),
            sex: sex.options[sex.value].text,
            isFaculty: false,
            classCode: "" // Update this based on your logic
        );

        RestClient.Put(databaseURL + "users/" + IDNumber.text + ".json", user);
    }

    public void CreateAccount(){
        debugText.text="createAccount";
        //This checks if the input boxes are null. The create user will not continue if the input boxes are empty
        if (string.IsNullOrEmpty(Username.text) || string.IsNullOrEmpty(Pass.text) || string.IsNullOrEmpty(ConfirmPass.text)){
            canvasAnimator.SetTrigger("PasswordDontMatch");
            signupErrorMessage.text = "Fields cannot be empty!";
            return;
        }            

        //ch1eck if email is valid, check if email entered ends with plm edu ph
        string enteredEmail = email.text.ToLower();
        string targetDomain = "@plm.edu.ph".ToLower();
        if (!enteredEmail.EndsWith(targetDomain)){
            canvasAnimator.SetTrigger("EmailInvalid");
            signupErrorMessage.text = "Invalid Email Address!";
            return;
        }

        if (Pass.text != ConfirmPass.text) //This checks if the password and confirmation match.
        {
            Pass.text = "";
            ConfirmPass.text = "";
            canvasAnimator.SetTrigger("PasswordDontMatch");
            signupErrorMessage.text = "Passwords don't match!";
            Debug.Log("Passwords do not match!");
            return;
        }
        SignUpUser(email.text , Username.text, Pass.text);
        Debug.Log("email: "+email.text+", userID: "+Username.text+", password: "+Pass.text);
    }

    public void LoginAndEmailVerify(){
        debugText.text = "LoginAndEmailVerify";
        SignInUser(LoginEmail.text, LoginPass.text);
    }
    
    private void SignUpUser(string email, string username, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + AuthKey, userData).Then(
            response =>
            {
                string emailVerification = "{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post(
                    "https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + AuthKey,
                    emailVerification);
                localId = response.localId;
                PostToDatabase(response.idToken);

                canvasAnimator.SetTrigger("ContentDown");
                canvasAnimator.SetTrigger("CreateSuccess");
                canvasAnimator.SetBool("isQueryComplete", true);
                NewWelcomePrompt.text = "Please check the inbox, spam, or junk of your PLM Email, "+email+", and click on the verification link to get started with your account for Vitual Lab!";

            }).Catch(error =>
            {
                GetUsername(email, username =>
                                {
                                    if (username != null)
                                    {   
                                         // Email already exists, handle the error
                                        Debug.Log("Email already exists");
                                        AccountExistsPrompt.text = "An account under the address of "+email+" already exists with the username "+username+". Would you like to login instead?";
                                        canvasAnimator.SetTrigger("ContentDown");
                                        canvasAnimator.SetTrigger("AccountExists");
                                    }
                                    else
                                    {
                                        // No user found with the given email
                                        Debug.Log("No user found with email: " + email);
                                    }
                                });
                debugText.text = "emailver"+error;
            });
    }

    private void GetUsername(string email, Action<string> callback)
{
    debugText.text = "trying to GetUsername of: " + email;

    RestClient.Get("https://virtual-lab-65d18-default-rtdb.asia-southeast1.firebasedatabase.app/users.json").Then(response =>
    {
        debugText.text = "Received response: " + response.Text;
        Debug.Log("Received response: " + response.Text);

        if (!string.IsNullOrEmpty(response.Text))
        {
            var users = JsonConvert.DeserializeObject<Dictionary<string, User>>(response.Text);
            if (users != null && users.Count > 0)
            {
                foreach (var userEntry in users)
                {
                    var user = userEntry.Value;
                    if (user.Email == email)
                    {
                        debugText.text = "User is: " + user.Username;
                        Debug.Log("User is: " + user.Username);
                        callback(user.Username);
                        return;
                    }
                }
            }
        }

        debugText.text = "User not found";
        callback(null);
    }).Catch(getUserError =>
    {
        Debug.LogError("Error getting account info: " + getUserError);
    });
}




    public void BackToSignUp()
    {
        IDNumber.text = "";
        Username.text = "";
        Pass.text = "";
        ConfirmPass.text = "";
        LoginPageManager.state = true;
        canvasAnimator.SetTrigger("BackToSignUp");
    }

    public void BackToLogin()
    {
        LoginEmail.text = existingUserName;
        Pass.text = "";
        ConfirmPass.text = "";
        LoginPageManager.state = false;
        canvasAnimator.SetTrigger("BackToLogin");
    }

    public void SignInUser(string email, string password)
    {
        debugText.text = "I will sign in now";
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        Debug.Log("the user data is: " + userData);

        RestClient.Post<SignResponse>("https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + AuthKey, userData)
            .Then(response =>
            {
                Debug.Log("RestClient has now posted request to verify password");

                string emailVerification = "{\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post(
                    "https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=" + AuthKey, emailVerification)
                    .Then(emailResponse =>
                        {
                            fsData emailVerificationData = fsJsonParser.Parse(emailResponse.Text);
                            EmailConfirmationInfo emailConfirmationInfo = new EmailConfirmationInfo();
                            serializer.TryDeserialize(emailVerificationData, ref emailConfirmationInfo)
                                .AssertSuccessWithoutWarnings();
                            if (emailConfirmationInfo.users[0].emailVerified)
                            {
                                idToken = response.idToken;
                                localId = response.localId;
                                Debug.Log("logged in.Why?");
                                debugText.text = "logged in, trying to animate";
                                canvasAnimator.SetTrigger("ContentDown");
                                canvasAnimator.SetTrigger("LoginVerified");
                                //The user is loggeed in successfully.
                                GetUsername(email, username =>
                                {
                                    debugText.text = "callback finished";
                                    if (username != null)
                                    {   
                                        debugText.text = "username found: "+username;
                                        WelcomePrompt.text = "Welcome Back, " + username + "!";
                                        tempUserHeader = username;
                                        // Use the username
                                        Debug.Log("Username: " + username);
                                        canvasAnimator.SetBool("isQueryComplete", true);
                                        LoginEmail.text = "";
                                        LoginPass.text = "";
                                        debugText.text = "should have logged in now";
                                    }
                                    else
                                    {
                                        // No user found with the given email
                                        debugText.text = "incorrect credentials should be playing";
                                        Debug.Log("No user found with email: " + email);
                                        canvasAnimator.SetTrigger("IncorrectCredentials");
                                        LoginEmail.text = "";
                                        LoginPass.text = "";
                                        LoginErrorMessage.text = "Incorrect Password!";
                                        debugText.text = "account doesn't exist";
                                    }
                                });
                            }
                            else
                            {
                                //Email is not yet verified
                                debugText.text = "email not verified";
                                Debug.Log("You need to verify your email.");
                                canvasAnimator.SetTrigger("IncorrectCredentials");
                                LoginPass.text = "";
                                LoginErrorMessage.text = "Email not yet Verified!";
                            }
                        })
                        .Catch(emailError =>
                        {
                            //Server Error or Connectivity Error
                            Debug.LogError("Error getting account info: " + emailError);
                        });
                })
                .Catch(signInError =>
            {
                // error with post request. wrong credentials.
                debugText.text = ""+signInError; 
               Debug.LogError("Error signing in: " + signInError);

                if (signInError is System.Net.WebException || signInError is System.Net.Sockets.SocketException || signInError is System.IO.IOException)
                {
                    // Connectivity issue occurred
                    debugText.text = "Connectivity issue. Please check your network connection.";
                }
                else
                {
                    if (signInError is Proyecto26.RequestException requestException)
                    {
                        if (requestException.StatusCode == (long)System.Net.HttpStatusCode.BadRequest)
                        {
                            var errorResponse = JsonUtility.FromJson<ErrorResponse>(requestException.Response);
                            string errorMessage = errorResponse.error.message;
                            if (errorMessage.Contains("EMAIL_NOT_FOUND"))
                            {
                                // Email does not exist in the authentication system
                                Debug.Log("Email does not exist. Please check your email.");
                                canvasAnimator.SetTrigger("IncorrectCredentials");
                                LoginEmail.text = "";
                                LoginPass.text = "";
                                LoginErrorMessage.text = "Email not yet registered!";
                            }
                            else if (errorMessage.Contains("INVALID_PASSWORD"))
                            {
                                // Incorrect password
                                Debug.Log("Incorrect password. Please check your password.");
                                canvasAnimator.SetTrigger("IncorrectCredentials");
                                LoginPass.text = "";
                                LoginErrorMessage.text = "Password incorrect!";
                            }
                            else
                            {
                                // Invalid email or password (generic error)
                                // input must be an email!
                                Debug.Log("Invalid email or password. Please check your credentials.");
                                canvasAnimator.SetTrigger("IncorrectCredentials");
                                LoginEmail.text = "";
                                LoginPass.text = "";
                                LoginErrorMessage.text = "Account doesn't exist!";
                            }
                        }
                    }
                }
            });
    }

    public void trySignUp(){
        debugText.text = "pls work huhu";
    }
}
