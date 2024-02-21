using UnityEngine;
using Zenject;

public class TestInstaler : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<MapProvider>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Spawner>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Dumpster>().FromComponentInHierarchy().AsSingle();
        Container.Bind<WeaponController>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}