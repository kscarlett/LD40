using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

public class UpgradeButtonInfo : MonoBehaviour
{
    public UpgradeInfo Info;
    public event EventHandler Click;
    public ReactiveProperty<int> Amount;
    public ReactiveProperty<float> Cost;

    [Inject]
    private void Construct(UpgradeInfo info)
    {
        Info = info;
    }

    void Start()
    {
        Cost = new ReactiveProperty<float>(Info.Cost);
        Amount = new ReactiveProperty<int>(0);
        GetComponent<Button>().onClick.AsObservable().Subscribe(_ =>
        {
            if (Click != null) Click.Invoke(this, null);
            Amount.Value += 1;
        });
        Amount.Subscribe(i => transform.Find("Owned number text").GetComponent<TextMeshProUGUI>().text = i.ToString());
        Cost.Subscribe(i => transform.Find("Price text").GetComponent<TextMeshProUGUI>().text = i.ToString(CultureInfo.CurrentCulture));
        transform.Find("Name text").GetComponent<TextMeshProUGUI>().text = Info.Name;
    }
}
