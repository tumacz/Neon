using UnityEngine;
using Zenject;

public class GameplaySceneInstaller : MonoInstaller
{
    [SerializeField]
    private MapGenerator _mapGenerator;

    public override void InstallBindings()
    {
        Container.Bind<MapGenerator>().FromInstance(_mapGenerator).AsSingle();
    }
}