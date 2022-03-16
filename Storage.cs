using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Core.Pool
{
    using Object;

    public sealed class Storage<TObject, TObjectTools, TObjectTemplate>
        where TObject : MonoPoolObject<TObjectTools, TObjectTemplate>
        where TObjectTools : IObjectTools 
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        public event System.Action OnEndAllCreated;

        public Transform Location => _location;
        public TObjectTemplate Template => _package.Template;
        public ISpawner Spawner => _package.Spawner;

        private readonly Transform _location;
        private readonly Queue<TObject> _freeForUse;
        private readonly List<TObject> _objects;
        private readonly MonoBehaviour _sender;
        private readonly Package _package;
        
        public Storage(MonoBehaviour sender,
                        Transform storageLocation, 
                        Package package)
        {
            _location = storageLocation;
            _freeForUse = new Queue<TObject>();
            _objects = new List<TObject>();
            _sender = sender;
            _package = package;
        }
        public void Execute()
        {
            _sender.StartCoroutine(AllCreate(_location, _package));
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
                for(int number = 0; number < package.NumberObjsChangeFrame; number++)
                {
                    TObject obj = Create(storageLocation, package.Template, package.Distributor);
                    obj.name += numberOrders;

                    _freeForUse.Enqueue(obj);
                    _objects.Add(obj);
                    numberOrders--;
                }
                yield return null;
            }
            OnEndAllCreated?.Invoke();
        }
        private TObject Create(Transform storageLocation, TObjectTemplate template, System.Func<TObject, TObjectTools> distributor)
        {
            TObject answer = MonoPoolObject<TObjectTools, TObjectTemplate>.Create<TObject>(template, storageLocation, distributor);
            answer.OnReturn += () => Returning(answer);

            return answer;
        }
        private void Returning(TObject obj)
        {
            obj.transform.parent = _location;
            _freeForUse.Enqueue(obj);
        }

        [System.Serializable] public struct Package
        {
            public ISpawner Spawner { get; private set; }
            public TObjectTemplate Template { get; private set; }
            public System.Func<TObject, TObjectTools> Distributor { get; private set; }
            public int NumberObjsChangeFrame { get; private set; }
            
            public Package(ISpawner spawner, TObjectTemplate template, System.Func<TObject, TObjectTools> distributor, int numberObjsChangeFrame)
            {
                Spawner = spawner;
                Template = template;
                Distributor = distributor;
                NumberObjsChangeFrame = numberObjsChangeFrame;
            }
        }
    }
}
