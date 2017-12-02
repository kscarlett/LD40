using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;

public class CastleBehaviour : MonoBehaviour
{
    private Dictionary<ResourceButtonInfo, ReactiveProperty<ulong>> _resourceButtonCounters;
    private UIBehaviour _ui;
    private ReactiveProperty<double> _gold;
    private DateTimeOffset _lastAdded;

    [Inject]
    private void Construct(UIBehaviour ui)
    {
        _resourceButtonCounters = new Dictionary<ResourceButtonInfo, ReactiveProperty<ulong>>();
        _ui = ui;
        foreach (ResourceButtonInfo btn in _ui.Buttons)
        {
            _resourceButtonCounters.Add(btn, new ReactiveProperty<ulong>(0));
        }
    }

    // Use this for initialization
    void Start()
    {
        _gold = new ReactiveProperty<double>(0);
        _lastAdded = DateTimeOffset.Now;

        this.UpdateAsObservable()
            .Timestamp()
            .Where(x => x.Timestamp >= _lastAdded.AddSeconds(1))
            .Subscribe(x =>
            {
                AddGold();
                _lastAdded = x.Timestamp;
            });

        _gold.Subscribe(d => _ui.GoldText.text = "Gold: " + d.ToString(CultureInfo.CurrentCulture));

        _ui.GoldButton.onClick.AsObservable().Subscribe(_ =>
        {
            AddGold();
            _gold.Value += 1;
        });

        foreach (ResourceButtonInfo btn in _resourceButtonCounters.Keys)
        {
            _gold.Where(l => l >= btn.Info.UnlockThreshhold).Subscribe(_ => btn.gameObject.SetActive(true));
        }

        for (var i = 0; i < _resourceButtonCounters.Keys.Count; i++)
        {
            ResourceButtonInfo button = _resourceButtonCounters.Keys.ToList()[i];
            button.Click += Button_Click;
            if (i > 0)
            {
                button.gameObject.SetActive(false);
            }
        }

        
    }

    private void AddGold()
    {
        foreach (ResourceButtonInfo btn in _resourceButtonCounters.Keys)
        {
            _gold.Value += _resourceButtonCounters[btn].Value * btn.Info.GoldPerSecond;
        }
    }

    private void Button_Click(object sender, EventArgs e)
    {
        _resourceButtonCounters[(ResourceButtonInfo) sender].Value += 1;
    }
}
