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
    private Dictionary<UpgradeButtonInfo, ReactiveProperty<ulong>> _upgradeButtonCounters;
    private UIBehaviour _ui;
    private ReactiveProperty<double> _gold;
    private DateTimeOffset _lastAdded;
    public int CastleLevel;

    [Inject]
    private void Construct(UIBehaviour ui)
    {
        _resourceButtonCounters = new Dictionary<ResourceButtonInfo, ReactiveProperty<ulong>>();
        _upgradeButtonCounters = new Dictionary<UpgradeButtonInfo, ReactiveProperty<ulong>>();
        _ui = ui;
        foreach (ResourceButtonInfo btn in _ui.Buttons)
        {
            _resourceButtonCounters.Add(btn, new ReactiveProperty<ulong>(0));
        }
        foreach (UpgradeButtonInfo btn in _ui.UpgradeButtons)
        {
            _upgradeButtonCounters.Add(btn, new ReactiveProperty<ulong>(0));
        }
    }

    // Use this for initialization
    void Start()
    {
        CastleLevel = 1;
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
            _gold.Where(l => l >= btn.Info.UnlockThreshhold).Subscribe(_ =>
            {
                btn.gameObject.SetActive(true);
                CastleLevel += 1;
            });
        }

        foreach (UpgradeButtonInfo btn in _upgradeButtonCounters.Keys)
        {
            _gold.Where(l => l >= btn.Info.UnlockThreshhold).Subscribe(_ =>
            {
                btn.gameObject.SetActive(true);
            });
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

        for (var i = 0; i < _upgradeButtonCounters.Keys.Count; i++)
        {
            UpgradeButtonInfo button = _upgradeButtonCounters.Keys.ToList()[i];
            button.Click += UpgradeButton_Click;
            button.gameObject.SetActive(false);
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
    private void UpgradeButton_Click(object sender, EventArgs e)
    {
        _upgradeButtonCounters[(UpgradeButtonInfo)sender].Value += 1;
    }
}
