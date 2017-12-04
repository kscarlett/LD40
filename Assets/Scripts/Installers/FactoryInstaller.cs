using UnityEngine;
using Zenject;

public class FactoryInstaller : MonoInstaller<FactoryInstaller>
{
    public GameObject EnemyPrefab;
    public override void InstallBindings()
    {
        Container.BindFactory<AIBase, AIBase.Factory>().FromComponentInNewPrefab(EnemyPrefab);
    }
}