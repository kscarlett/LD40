using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UIBehaviour : MonoBehaviour
{
    public List<ResourceButtonInfo> Buttons;
    public List<UpgradeButtonInfo> UpgradeButtons;
    public TextMeshProUGUI GoldText;
    public Button GoldButton;
    private GameObject _messageBoxObject;
    private List<string> _messages;
    private bool _isLost;
    private GameObject _notEnoughGoldObject;
    private DateTimeOffset _lastEnabled;
    private Camera _cam;

    [Inject]
    private void Construct(List<ResourceButtonInfo> buttons, List<UpgradeButtonInfo> upgradeButtons, [Inject(Id = "GoldText")] TextMeshProUGUI goldText, [Inject(Id = "GoldClicker")] Button goldButton, [Inject(Id = "MessageBox")] GameObject messageBoxObject, [Inject(Id = "NotEnoughGoldBox")] GameObject notEnoughGoldObject, Camera cam)
    {
        Buttons = buttons;
        GoldText = goldText;
        GoldButton = goldButton;
        UpgradeButtons = upgradeButtons;
        _messageBoxObject = messageBoxObject;
        _notEnoughGoldObject = notEnoughGoldObject;
        _cam = cam;
    }

    void Start()
    {
        _lastEnabled = DateTimeOffset.Now;
        this.UpdateAsObservable().Where(_ => _notEnoughGoldObject.activeInHierarchy).Timestamp()
            .Where(x => x.Timestamp > _lastEnabled.AddSeconds(2)).Subscribe(x =>
            {
                _notEnoughGoldObject.SetActive(false);
            });
        _messageBoxObject.GetComponentInChildren<Button>().onClick.AsObservable().Subscribe(_ =>
        {
            _messageBoxObject.SetActive(false);
            Time.timeScale = 1;
        });
        _messages = new List<string>();
        this.UpdateAsObservable().Where(_ => !_messageBoxObject.activeInHierarchy && _messages.Count > 0).Subscribe(x =>
        {
            ShowMessage(_messages.Last());
            _messages.Remove(_messages.Last());
        });
        this.UpdateAsObservable().Where(_ => _messageBoxObject.activeInHierarchy && Time.timeScale == 0).Subscribe(x =>
        {
            Time.timeScale = 1;
            if (_isLost)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        });
        _isLost = false;
    }

    public void PlayGoldSound()
    {
        _cam.GetComponent<AudioSource>().Play();
    }

    public void AlertUserToNoGold()
    {
        _lastEnabled = DateTimeOffset.Now;
        _notEnoughGoldObject.SetActive(true);
    }

    public void ShowMessage(string message, bool isLost = false)
    {
        _isLost = isLost;
        if (_messageBoxObject.activeInHierarchy)
        {
            _messages.Add(message);
        }
        else
        {
            Time.timeScale = 0;
            _messageBoxObject.SetActive(true);
            _messageBoxObject.transform.Find("Message").GetComponent<TextMeshProUGUI>().text = message;
        }
    }
}
