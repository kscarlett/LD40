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

    // Use this for initialization
    void Start()
    {

    }

    [Inject]
    private void Construct(List<ResourceButtonInfo> buttons,[Inject(Id = "GoldText")] TextMeshProUGUI goldText)
    {
        Buttons = buttons;
        GoldText = goldText;
    }
}
