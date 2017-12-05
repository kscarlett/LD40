using System;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIInstaller : MonoInstaller<UIInstaller>
{
    public ResourceCollectorInfo[] ResourceCollectors;
    public UpgradeInfo[] Upgrades;
    public GameObject UpgradesButtonContainer;
    public GameObject ResourcesButtonContainer;
    public GameObject ButtonPrefab;
    public GameObject UpgradeButtonPrefab;
    public GameObject GoldTextObj;
    public GameObject GoldButtonObj;
    public GameObject MessageBoxObject;
    public GameObject NotEnoughGoldObject;

    public override void InstallBindings()
    {
        Container.Bind<Camera>().FromComponentInHierarchy();
        Container.BindInstance(NotEnoughGoldObject).WithId("NotEnoughGoldBox");
        Container.BindInstance(MessageBoxObject).WithId("MessageBox");
        Container.BindInstance(GoldButtonObj.GetComponent<Button>()).WithId("GoldClicker");
        Container.BindInstance(GoldTextObj.GetComponent<TextMeshProUGUI>()).WithId("GoldText");
        Container.BindInstance(ResourceCollectors);
        Container.BindInstance(Upgrades);
        Container.BindInstance(ButtonPrefab).WithId("ButtonPrefab");
        Container.BindInstance(UpgradeButtonPrefab).WithId("UpgradeButtonPrefab");
        Container.BindInstance(ResourcesButtonContainer).WithId("ResourcesListContainer");
        Container.BindInstance(UpgradesButtonContainer).WithId("UpgradesListContainer");
        Container.Bind<List<ResourceButtonInfo>>().FromMethod(ConstructResourceButtons);
        Container.Bind<List<UpgradeButtonInfo>>().FromMethod(ConstructUpgradeButtons);
        Container.Bind<UIBehaviour>().FromComponentInHierarchy();
    }

    List<ResourceButtonInfo> ConstructResourceButtons(InjectContext context)
    {
        var buttons = new List<ResourceButtonInfo>();
        ResourceCollectorInfo[] infoColl = context.Container.Resolve<ResourceCollectorInfo[]>();
        GameObject buttonPrefab = context.Container.ResolveId<GameObject>("ButtonPrefab");
        GameObject resourcesListContainer = context.Container.ResolveId<GameObject>("ResourcesListContainer");
        foreach (ResourceCollectorInfo info in infoColl)
        {
            GameObject result = Instantiate(buttonPrefab, resourcesListContainer.transform);
            buttons.Add(context.Container.InjectGameObjectForComponent<ResourceButtonInfo>(result, new System.Object[] {info}));
        }
        return buttons;
    }

    List<UpgradeButtonInfo> ConstructUpgradeButtons(InjectContext context)
    {
        var buttons = new List<UpgradeButtonInfo>();
        UpgradeInfo[] infoColl = context.Container.Resolve<UpgradeInfo[]>();
        GameObject buttonPrefab = context.Container.ResolveId<GameObject>("UpgradeButtonPrefab");
        GameObject resourcesListContainer = context.Container.ResolveId<GameObject>("UpgradesListContainer");
        foreach (UpgradeInfo info in infoColl)
        {
            GameObject result = Instantiate(buttonPrefab, resourcesListContainer.transform);
            buttons.Add(context.Container.InjectGameObjectForComponent<UpgradeButtonInfo>(result, new System.Object[] { info }));
        }
        return buttons;
    }
}