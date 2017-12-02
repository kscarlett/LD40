using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Events;

public class UIBehaviour : MonoBehaviour
{
    public List<ResourceButtonInfo> Buttons;

    // Use this for initialization
    void Start()
    {
        for (var i = 0; i < Buttons.Count; i++)
        {
            ResourceButtonInfo button = Buttons[i];
            button.Click += Button_Click;
            if (i > 0)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    private void Button_Click(object sender, System.EventArgs e)
    {
        print("you clicked the: " + ((ResourceButtonInfo)sender).Info.Name + " button!");
    }

    [Inject]
    private void Construct(List<ResourceButtonInfo> buttons)
    {
        Buttons = buttons;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
