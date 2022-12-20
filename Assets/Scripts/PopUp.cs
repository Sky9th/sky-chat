using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PopUp : VisualElement
{
    UIDocument root;
    VisualElement popUp;
    private Button confirmBtn;

    public event Action onConfirm;
    public event Action confirmed;

    public PopUp(UIDocument root, string title, string msg, VisualTreeAsset popUpUxml)
    {
        this.root = root;
        popUp = popUpUxml.CloneTree();

        popUp.style.position = new StyleEnum<Position>(Position.Absolute);
        popUp.style.left = 0;
        popUp.style.top = 0;
        popUp.style.width = Length.Percent(100);
        popUp.style.height = Length.Percent(100);

        confirmBtn = popUp.Query<Button>().First();
        confirmBtn.clicked += confirm;

        Label titleLabel = popUp.Query<Label>("Title").First();
        titleLabel.text = title;
        Label msgLabel = popUp.Query<Label>("Msg").First();
        msgLabel.text = msg;

        root.rootVisualElement.Add(popUp);
    }

    private void confirm()
    {
        root.rootVisualElement.Remove(popUp);
        if (confirmed != null)
        {
            onConfirm = confirmed;
        }
        if (onConfirm != null)
        {
            confirmed();
        }
    }
}