using System;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UIInstaller : MonoInstaller<UIInstaller>
{
    public ResourceCollectorInfo[] ResourceCollectors;
    public GameObject ResourcesButtonContainer;
    public GameObject ButtonPrefab;
    public GameObject GoldTextObj; //WHERE IS THIS FIELD REEEE
    public GameObject GoldButtonObj;

    public override void InstallBindings()
    {
        Container.BindInstance(GoldButtonObj.GetComponent<Button>()).WithId("GoldClicker");
        Container.BindInstance(GoldTextObj.GetComponent<TextMeshProUGUI>()).WithId("GoldText");
        Container.BindInstance(ResourceCollectors);
        Container.BindInstance(ButtonPrefab).WithId("ButtonPrefab");
        Container.BindInstance(ResourcesButtonContainer).WithId("ResourcesListContainer");
        Container.Bind<List<ResourceButtonInfo>>().FromMethod(ConstructButtons);
        Container.Bind<UIBehaviour>().FromComponentInHierarchy();
    }

    List<ResourceButtonInfo> ConstructButtons(InjectContext context)
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
}