using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;

public class CastleBehaviour : MonoBehaviour, IDamageable
{
    private Dictionary<ResourceButtonInfo, ReactiveProperty<ulong>> _resourceButtonCounters;
    private Dictionary<UpgradeButtonInfo, ReactiveProperty<ulong>> _upgradeButtonCounters;
    private UIBehaviour _ui;
    private ReactiveProperty<double> _gold;
    private DateTimeOffset _lastAdded;
    public ReactiveProperty<int> CastleLevel;
    private EnemyManager _manager;

    [Inject]
    private void Construct(UIBehaviour ui, EnemyManager manager)
    {
        _manager = manager;
        _ui = ui;
    }

    public bool PayGold(double amount)
    {
        if (amount <= _gold.Value)
        {
            _gold.Value -= amount;
            return true;
        }
        else
        {
            _ui.AlertUserToNoGold();
            return false;
        }
    }

    void Start()
    {
        UnderAttack = new ReactiveProperty<bool>();
        _resourceButtonCounters = new Dictionary<ResourceButtonInfo, ReactiveProperty<ulong>>();
        _upgradeButtonCounters = new Dictionary<UpgradeButtonInfo, ReactiveProperty<ulong>>();

        foreach (ResourceButtonInfo btn in _ui.Buttons)
        {
            _resourceButtonCounters.Add(btn, new ReactiveProperty<ulong>(0));
        }
        foreach (UpgradeButtonInfo btn in _ui.UpgradeButtons)
        {
            _upgradeButtonCounters.Add(btn, new ReactiveProperty<ulong>(0));
        }
        CastleLevel = new ReactiveProperty<int>(1);
        _gold = new ReactiveProperty<double>(0);
        _lastAdded = DateTimeOffset.Now;

        this.UpdateAsObservable()
            .Timestamp()
            .Where(x => x.Timestamp >= _lastAdded.AddSeconds(1) && Time.timeScale != 0)
            .Subscribe(x =>
            {
                AddGold();
                _lastAdded = x.Timestamp;
            });

        _gold.Subscribe(d =>
        {
            if (d < 0)
            {
                _ui.ShowMessage("Your castle has been ransacked!", true);
                _ui.GoldText.text = "Gold: 0";
                Destroy(gameObject);
            }
            else
            {
                _ui.GoldText.text = "Gold: " + d.ToString(CultureInfo.CurrentCulture);
                foreach (ResourceButtonInfo btnInfo in _resourceButtonCounters.Keys)
                {
                    if (btnInfo.Info.Cost > _gold.Value)
                    {
                        btnInfo.GetComponent<Button>().interactable = false;
                    }
                    else if (!btnInfo.GetComponent<Button>().interactable)
                    {
                        btnInfo.GetComponent<Button>().interactable = true;
                    }
                }
                foreach (UpgradeButtonInfo btnInfo in _upgradeButtonCounters.Keys)
                {
                    if (btnInfo.Info.Cost > _gold.Value)
                    {
                        btnInfo.GetComponent<Button>().interactable = false;
                    }
                    else if (!btnInfo.GetComponent<Button>().interactable)
                    {
                        btnInfo.GetComponent<Button>().interactable = true;
                    }
                }
            }
        });

        _ui.GoldButton.onClick.AsObservable().Subscribe(_ =>
        {
            AddGold();
            _gold.Value += 1;
        });

        foreach (ResourceButtonInfo btn in _resourceButtonCounters.Keys)
        {
            _gold.Where(l => l >= btn.Info.UnlockThreshhold && !btn.Info.HasRun).Subscribe(_ =>
            {
                btn.gameObject.SetActive(true);
                _ui.ShowMessage(btn.Info.UnlockMessage);
                CastleLevel.Value += 1;
                btn.Info.HasRun = true;
            });
        }

        foreach (UpgradeButtonInfo btn in _upgradeButtonCounters.Keys)
        {
            _gold.Where(l => l >= btn.Info.UnlockThreshhold && !btn.Info.HasRun).Subscribe(_ =>
            {
                btn.gameObject.SetActive(true);
                btn.Info.HasRun = true;
                _ui.ShowMessage(btn.Info.UnlockMessage);
            });
        }

        foreach (ResourceButtonInfo btn in _resourceButtonCounters.Keys)
        {
            btn.Amount
                .Where(x => x >= (int)btn.Info.EnemyUnlockThreshold && !_manager.GetEnemy(btn.Info.EnemyName).Spawnable)
                .Subscribe(x => _manager.GetEnemy(btn.Info.EnemyName).Spawnable = true);
            _ui.ShowMessage(btn.Info.EnemyUnlockMessage);
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
        var theBtn = (ResourceButtonInfo) sender;
        if (_gold.Value >= theBtn.Info.Cost)
        {
            _gold.Value -= theBtn.Info.Cost;
            theBtn.Cost.Value += 1;
            _resourceButtonCounters[theBtn].Value += 1;
        }

    }

    private void UpgradeButton_Click(object sender, EventArgs e)
    {
        var theBtn = (UpgradeButtonInfo) sender;
        if (_gold.Value >= theBtn.Info.Cost)
        {
            _gold.Value -= theBtn.Info.Cost;
            theBtn.Cost.Value += 1;
            _upgradeButtonCounters[theBtn].Value += 1;
        }
    }

    public ReactiveProperty<bool> UnderAttack { get; set; }

    public void TakeDamage(int damage)
    {
        _gold.Value -= damage;
    }
}
