using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartUIController : MonoBehaviour
{
    readonly Request req = new();

    VisualElement startBtn;
    VisualElement captchaImg;
    TextField inputEmail;
    TextField inputPassword;
    TextField inputCode;
    // Start is called before the first frame update
    void Start()
    {
        UIDocument menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;

        inputEmail = root.Query<TextField>("Email").First();
        inputPassword = root.Query<TextField>("Password").First();
        inputCode = root.Query<TextField>("Code").First();

        startBtn = root.Query<Button>("Start").First();
        startBtn.RegisterCallback<ClickEvent>(SignUp);

        captchaImg = root.Query<Button>("Captcha").First();
        captchaImg.RegisterCallback<ClickEvent>(RefreshCaptcha);

        RefreshCaptcha(null);
    }

    private void SignUp(ClickEvent evt)
    {
        Dictionary<string,string> loginForm = new Dictionary<string, string> { };

        loginForm.Add("mail", inputEmail.value);
        loginForm.Add("password", inputPassword.value);
        loginForm.Add("fingerprint", GetMacAddress());
        loginForm.Add("code", inputCode.value);

        StartCoroutine(req.requestPost("user/login", loginForm, Login));
    }

    private void Login (Res res)
    {
        Debug.Log(res);
        /*if (res.code == "0")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } else
        {
            VisualElement popUp = new VisualElement();
        }*/
    }

    private void RefreshCaptcha (ClickEvent evt)
    {
        StartCoroutine(req.requestGet("user/captcha", new Dictionary<string, string> { }, DrawCaptcha));
    }

    private void DrawCaptcha (Res res)
    {
        Debug.Log(res.data);
        string[] b64 = res.data.Split(',');
        byte[] imageBytes = Convert.FromBase64String(b64[1]);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        StyleBackground bg = new StyleBackground(tex);
        captchaImg.style.backgroundImage = bg;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string GetMacAddress ()
    {
        string physicalAddress = "";
        NetworkInterface[] networkInerfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adpater in networkInerfaces)
        {
            physicalAddress = adpater.GetPhysicalAddress().ToString();
            if (physicalAddress != "")
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(physicalAddress);
                return System.Convert.ToBase64String(plainTextBytes);
            }
        }
        return physicalAddress;
    }
}