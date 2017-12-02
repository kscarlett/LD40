using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Events;

public class UIBehaviour : MonoBehaviour
{
    private List<ResourceButtonInfo> _buttons;

    // Use this for initialization
    void Start()
    {
        foreach (ResourceButtonInfo button in _buttons)
        {
            button.Click += Button_Click;
        }
    }

    private void Button_Click(object sender, System.EventArgs e)
    {
        print("you clicked the: " + ((ResourceButtonInfo)sender).Info.Name + " button!");
    }

    [Inject]
    private void Construct(List<ResourceButtonInfo> buttons)
    {
        _buttons = buttons;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
