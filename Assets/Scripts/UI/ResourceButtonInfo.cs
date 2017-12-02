using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

public class ResourceButtonInfo : MonoBehaviour
{
    public ResourceCollectorInfo Info;
    public event EventHandler Click;
    public ReactiveProperty<int> Amount;

    [Inject]
    private void Construct(ResourceCollectorInfo info)
    {
        Info = info;
    }

    void Start()
    {
        Amount = new ReactiveProperty<int>(0);
        GetComponent<Button>().onClick.AsObservable().Subscribe(_ =>
        {
            if (Click != null) Click.Invoke(this, null);
            Amount.Value += 1;
        });
        Amount.Subscribe(i => transform.Find("Owned number text").GetComponent<TextMeshProUGUI>().text = i.ToString());
        transform.Find("Price text").GetComponent<TextMeshProUGUI>().text = Info.Cost.ToString(CultureInfo.CurrentCulture);
        transform.Find("Name text").GetComponent<TextMeshProUGUI>().text = Info.Name;
    }
}
