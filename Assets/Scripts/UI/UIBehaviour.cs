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

public class UIBehaviour : MonoBehaviour
{
    public List<ResourceButtonInfo> Buttons;
    public List<UpgradeButtonInfo> UpgradeButtons;
    public TextMeshProUGUI GoldText;
    public Button GoldButton;
    private GameObject _messageBoxObject;
    private List<string> _messages;

    [Inject]
    private void Construct(List<ResourceButtonInfo> buttons, List<UpgradeButtonInfo> upgradeButtons, [Inject(Id = "GoldText")] TextMeshProUGUI goldText, [Inject(Id = "GoldClicker")] Button goldButton, [Inject(Id = "MessageBox")] GameObject messageBoxObject)
    {
        Buttons = buttons;
        GoldText = goldText;
        GoldButton = goldButton;
        UpgradeButtons = upgradeButtons;
        _messageBoxObject = messageBoxObject;
        _messageBoxObject.GetComponentInChildren<Button>().onClick.AsObservable().Subscribe(_ =>
        {
            _messageBoxObject.SetActive(false);
            Time.timeScale = 1;
        });
    }

    void Start()
    {
        _messages = new List<string>();
        this.UpdateAsObservable().Where(_ => !_messageBoxObject.activeInHierarchy && _messages.Count > 0).Subscribe(x =>
        {
            ShowMessage(_messages.Last());
            _messages.Remove(_messages.Last());
        });
    }

    public void ShowMessage(string message)
    {
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
