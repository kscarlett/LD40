using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Events;

public class UIBehaviour : MonoBehaviour
{
    public List<ResourceButtonInfo> Buttons;
    public TextMeshProUGUI GoldText;
    public Button GoldButton;

    [Inject]
    private void Construct(List<ResourceButtonInfo> buttons,[Inject(Id = "GoldText")] TextMeshProUGUI goldText, [Inject(Id = "GoldClicker")] Button goldButton)
    {
        Buttons = buttons;
        GoldText = goldText;
        GoldButton = goldButton;
    }
}
