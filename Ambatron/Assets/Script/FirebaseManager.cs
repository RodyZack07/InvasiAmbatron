using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    public GameObject loginPanel, registerPanel;
    public InputField emailFieldLog, passwordFieldLog, confirmPassField, nameField, regEmail, regPassword;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void openLogin() {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void OpenRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void LoginUser() {
        if (string.IsNullOrEmpty(emailFieldLog.text) && string.IsNullOrEmpty(passwordFieldLog.text)){
            return;
        }
    }

    public void RegistUser() {
        if (string.IsNullOrEmpty(regEmail.text) && string.IsNullOrEmpty(regPassword.text)) {
            return;
        }
    }
}
