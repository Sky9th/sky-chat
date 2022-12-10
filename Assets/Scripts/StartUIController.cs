using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartUIController : MonoBehaviour
{
    Request req;

    UIDocument menu;
    VisualElement root;
    VisualElement startBtn;
    VisualElement captchaImg;
    TextField inputEmail;
    TextField inputPassword;
    TextField inputCode;
    // Start is called before the first frame update
    void Start()
    {
        menu = GetComponent<UIDocument>();
        root = menu.rootVisualElement;
        req = new Request();

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

        if (inputCode.value == "")
        {
            new PopUp(menu, "Error", "Please input the code");
            return;
        }
        if (inputEmail.value == "")
        {
            new PopUp(menu, "Error", "Please input the email");
            return;
        }
        if (inputPassword.value == "")
        {
            new PopUp(menu, "Error", "Please input the password");
            return;
        }
        Debug.Log(inputCode.value);
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        Match match = regex.Match(inputEmail.value);
        if (!match.Success)
        {
            new PopUp(menu, "Error", "Please input the correct Email");
            return;
        }

        loginForm.Add("mail", inputEmail.value);
        loginForm.Add("password", inputPassword.value);
        loginForm.Add("fingerprint", GetMacAddress());
        loginForm.Add("code", inputCode.value);

        StartCoroutine(req.requestPost("user/login", loginForm, Login, LoginFail));
    }

    private void Login (Res res, UnityWebRequest www)
    {
        Debug.Log(res.data);
        LoginData ld = JsonUtility.FromJson<LoginData>(www.downloadHandler.text);
        PlayerPrefs.SetString(Store.SESSION_KEY, ld.data.sessionKey);
        PlayerPrefs.SetString(Store.EMAIL, ld.data.mail);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void LoginFail (Res res, UnityWebRequest www)
    {
        RefreshCaptcha(null);
    }

    private void RefreshCaptcha (ClickEvent evt)
    {
        Dictionary<string, string> captchaForm = new Dictionary<string, string> { };
        captchaForm.Add("fingerprint", GetMacAddress());
        StartCoroutine(req.requestPost("user/captcha", captchaForm, DrawCaptcha));
    }

    private void DrawCaptcha (Res res, UnityWebRequest www)
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


[System.Serializable]
public class Data
{
    public string mail;
    public string sessionKey;
}
[System.Serializable]
class LoginData
{
    //{"code":0,"msg":"µÇÂ½³É¹¦","data":{"sessionKey":"web167065836163943939dafa32","mail":"weitianxu@qq.com"}}
    public string code;
    public string msg;
    public Data data;
}