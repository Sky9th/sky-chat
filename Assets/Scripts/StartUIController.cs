using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartUIController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        UIDocument menu = GetComponent<UIDocument>();
        VisualElement root = menu.rootVisualElement;
        VisualElement button = root.Query<Button>("Start").First();
        button.RegisterCallback<ClickEvent>(BtnStart);
    }

    IEnumerator Login()
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        UnityWebRequest www = UnityWebRequest.Post("http://api.skyadmin.sky9th.cn/user/login", formData);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            Res res = JsonUtility.FromJson<Res>(www.downloadHandler.text);
            Debug.Log(res.code);
        }
    }

    private void BtnStart(ClickEvent evt)
    {
        StartCoroutine(Login());
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[System.Serializable]
public class Res
{

    public string code;
    public string msg;
    public string data;

    public static Res CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Res>(jsonString);
    }
}