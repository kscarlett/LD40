using UnityEngine;
using Zenject;

public class FactoryInstaller : MonoInstaller<FactoryInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<AIBase.Factory>().AsTransient();
    }
}