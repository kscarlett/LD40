using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CastleBehaviour : MonoBehaviour
{
    private Dictionary<ResourceButtonInfo, int> _resourceButtonCounters;

    [Inject]
    private void Construct(UIBehaviour ui)
    {
        foreach (ResourceButtonInfo btn in ui.Buttons)
        {
            _resourceButtonCounters.Add(btn, 0);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
