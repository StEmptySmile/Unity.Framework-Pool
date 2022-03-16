using System.Collections.Generic;

namespace Core.Pool
{
    using Object;

    public interface ISpawner
    {
        public bool IsObjectsAreCreated { get; }
        public event System.Action OnEndAllCreated;
        public int AmountForStorage { get; } 
    }
    public interface ISpawnedObject<TObject, TObjectTools, TObjectTemplate> : ISpawner
        where TObject : MonoPoolObject<TObjectTools, TObjectTemplate>
        where TObjectTools : IObjectTools
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        public TObjectTemplate Template { get; }
        public Storage<TObject, TObjectTools, TObjectTemplate> Storage { get; }  
    }
    public interface ISpawnedObjects<TObject, TObjectTools, TObjectTemplate> : ISpawner
        where TObject : MonoPoolObject<TObjectTools, TObjectTemplate>
        where TObjectTools : IObjectTools
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        public IReadOnlyList<TObjectTemplate> Templates { get; }
        public IReadOnlyDictionary<TObjectTemplate, Storage<TObject, TObjectTools, TObjectTemplate>> Storages { get; }  
    }
}