using UnityEngine;
using Zenject;

public class CastleInstaller : MonoInstaller<CastleInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<CastleBehaviour>().FromComponentInHierarchy();
    }
}