using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using PopupWindow = UnityEditor.PopupWindow;

public class Request
{
    readonly string host = "http://api.skyadmin.sky9th.cn/";

    public delegate void Callback(Res res);

    public IEnumerator requestPost(string url, Dictionary<string, string> data, Callback callback)
    {

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        foreach (KeyValuePair<string, string> kvp in data)
        {
            //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
            formData.Add(new MultipartFormDataSection(kvp.Key, kvp.Value));
        }

        UnityWebRequest www = UnityWebRequest.Post(host + url, formData);
        www = SetHeader(www);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log(Screen.width);
            Rect rect = new Rect(0, 0, 0, 0);
            PopupWindow.Show(rect, new PopUp());
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            Res res = JsonUtility.FromJson<Res>(www.downloadHandler.text);
            callback(res);
        }
    }

    public IEnumerator requestGet(string url, Dictionary<string, string> data, Callback callback)
    {
        string param = "";
        foreach (KeyValuePair<string, string> kvp in data)
        {
            //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
            param +=  (param.Length > 0 ? "&" : "") + kvp.Key + '=' + kvp.Value;
        }

        UnityWebRequest www = UnityWebRequest.Get(host + url + (param.Length > 0 ? '?' : "") + param);
        www = SetHeader(www);
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
            callback(res);
        }
    }

    private UnityWebRequest SetHeader (UnityWebRequest www)
    {
        string nonceStr = RandomStr(6);
        string timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();

        string signature = MakeSignature(nonceStr, timestamp);
        www.SetRequestHeader("Signature", signature);
        www.SetRequestHeader("Timestamp", timestamp);
        www.SetRequestHeader("NonceStr", nonceStr);
        www.SetRequestHeader("Session-Key", "");
        return www;
    }

    private string RandomStr (int length)
    {
        string x = "0123456789qwertyuioplkjhgfdsazxcvbnm";
        string tmp = "";
        for (int i = 0; i < length; i++)
        {
            System.Random rnd = new System.Random();
            int rand = rnd.Next(0, 99999999);
            int charAt = Math.Abs(rand * 100000000 % x.Length);
            tmp += x[charAt];
        }
        return tmp;
    }

    private string MakeSignature (string nonceStr, string timestamp)
    {
        SignatureData sd = new SignatureData(nonceStr, timestamp);
        string encryptJson = JsonUtility.ToJson(sd);

        RSACryptoService rsa = new RSACryptoService(null, "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCAUlVSmPZnhNFJgBzWIzfTt4bJ\n" +
        "y1EZC7JLumn/1raTNTHwbC3vUzT6JRUbXJ8rTtfFI3ul/848HJPQlCbp37EcawrE\n" +
        "lbr0G3IibEf7R21s8Yz65B6Z1ERrd/ZZzQIvVoo95YJMuk8oKJrVylcYin7RiXRM\n" +
        "UOxcgVUarN4Pn1DByQIDAQAB");
        string encryptStr = rsa.Encrypt(encryptJson);
        Debug.Log(encryptStr);
        return encryptStr;
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

public class SignatureData
{
    public string timestamp;
    public string nonceStr;
    public string key = "MTY0MDY4MDgzNw==";

    public SignatureData (string nonceStr, string timestamp)
    {
        this.timestamp = timestamp;
        this.nonceStr = nonceStr;
    }

}