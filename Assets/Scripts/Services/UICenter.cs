using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UICenter
{
    private static UICenter uICenter;
    public static GameObject uiDocument;
    public static VisualElement rootElement;

    public static float ps2px;

    public static UICenter getInstance ()
    {
        if (uiDocument == null)
        {
            uiDocument = GameObject.Find("UIDocument");
            rootElement = uiDocument.GetComponent<UIDocument>().rootVisualElement;
            setPs2px();
        }
        return uICenter;
    }

    public static void add(VisualElement ele)
    {
        ele.style.position = Position.Absolute;
        ele.style.left = 0;
        ele.style.top = 0;
        rootElement.Add(ele);
    }

    public static void setPs2px ()
    {
        Vector3 zero = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10));
        Vector3 plus = Camera.main.ScreenToWorldPoint(new Vector3(1, 0, 10));
        ps2px = 1 / (plus.x - zero.x);

    }

}
