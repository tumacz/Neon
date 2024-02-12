using UnityEngine;
using Zenject;

public class TestInstaler : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CameraPostion>().FromComponentInHierarchy().AsSingle();
    }
}