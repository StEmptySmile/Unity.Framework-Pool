using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace ESUnity.Pool
{
    using Object;

    public sealed class Storage<TObject, TObjectTools, TObjectTemplate>
        where TObject : MonoPoolObject<TObjectTools, TObjectTemplate>
        where TObjectTools : IObjectTools 
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        public event System.Action OnAllObjectsAreCreated;
        public bool IsObjectsAreCreated { get; private set; } = false;
        public ISpawner<TObjectTemplate> Spawner => _package.Spawner;

        private readonly Queue<TObject> _freeForUse;
        private readonly List<TObject> _objects;
        private readonly MonoBehaviour _sender;
        private readonly Package _package;
        
        public Storage(MonoBehaviour sender, Package package)
        {
            _freeForUse = new Queue<TObject>();
            _objects = new List<TObject>();
            _sender = sender;
            _package = package;
        }
        public void Execute()
        {
            _sender.StartCoroutine(AllCreate(Spawner.StorageLocation, _package));
        }
        public TObject Receive()
        {
            TObject answer = _freeForUse.Dequeue();
            return answer;
        }
        private IEnumerator AllCreate(Transform storageLocation, Package package)
        {
            int numberOrders = package.Spawner.AmountForStorage;
            
            while (numberOrders > 0)
            {
                for(int number = 0; number < package.NumberObjectsPerFrame; number++)
                {
                    TObject obj = Create(storageLocation, package.Spawner.ObjectInstance, package.Distributor);
                    obj.name += numberOrders;

                    _freeForUse.Enqueue(obj);
                    _objects.Add(obj);
            
                    numberOrders--;
                }
                yield return null;
            }

            OnAllObjectsAreCreated?.Invoke();
            IsObjectsAreCreated = true;
        }
        private TObject Create(Transform storageLocation, TObjectTemplate template, System.Func<TObject, TObjectTools> distributor)
        {
            TObject answer = MonoPoolObject<TObjectTools, TObjectTemplate>.Create<TObject>(template, storageLocation, distributor);
            answer.OnReturn += () => Returning(answer);

            return answer;
        }
        private void Returning(TObject obj)
        {
            obj.transform.parent = Spawner.StorageLocation;
            _freeForUse.Enqueue(obj);
        }

        [System.Serializable] public struct Package
        {
            public ISpawner<TObjectTemplate> Spawner { get; private set; }
            public System.Func<TObject, TObjectTools> Distributor { get; private set; }
            public int NumberObjectsPerFrame { get; private set; }
            
            public Package(ISpawner<TObjectTemplate> spawner, System.Func<TObject, TObjectTools> distributor, int numberObjectsPerFrame)
            {
                Spawner = spawner;
                Distributor = distributor;
                NumberObjectsPerFrame = numberObjectsPerFrame;
            }
        }
    }
}