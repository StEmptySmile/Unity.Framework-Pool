using System.Collections.Generic;
using UnityEngine;

namespace Core.Pool
{
    using Object;

    public sealed class Storage<TObject, TObjectTools, TObjectTemplate>
        where TObject : BaseObject
        where TObjectTools : IObjectTools 
        where TObjectTemplate : IObjectTemplate<TObject>
    {
        public event System.Action OnEndAllCreated;
        public IReadOnlyList<TObject> Objects => _objects;
        public TObjectTemplate Template { get; private set; }
        public ISpawner Spawner { get; private set; }

        private readonly Transform _storageLocation;
        private readonly Queue<TObject> _freeForUse;
        private readonly List<TObject> _objects;

        public Storage(MonoBehaviour sender,
                        Transform storageLocation, 
                        Package package)
        {
            Template = package.Template;
            Spawner = package.Spawner;
            _storageLocation = storageLocation;
            _freeForUse = new Queue<TObject>();
            _objects = new List<TObject>();

            if(package.Spawner.AmountForStorage > 0)
                sender.StartCoroutine(AllCreate(storageLocation, package));
        }
        public TObject Receive()
        {
            TObject answer = _freeForUse.Dequeue();
            return answer;
        }
        private System.Collections.IEnumerator AllCreate(Transform storageLocation, Package package)
        {
            int numberOrders = package.Spawner.AmountForStorage;
            if(package.NumberObjsChangeFrame <= 0)
            {
                for(int number = 0; number < numberOrders; number++)
                {
                    TObject obj = Create(storageLocation, package.Template, package.Distributor);
                    obj.name += number;

                    _freeForUse.Enqueue(obj);
                    _objects.Add(obj);
                }
            }
            else
            {
                while (numberOrders > 0)
                {
                    int number = Mathf.Clamp(package.NumberObjsChangeFrame - numberOrders, 0, package.NumberObjsChangeFrame);
                    for(; number < package.NumberObjsChangeFrame; number++)
                    {
                        TObject obj = Create(storageLocation, package.Template, package.Distributor);
                        obj.name += numberOrders;

                        _freeForUse.Enqueue(obj);
                        _objects.Add(obj);
                        numberOrders--;
                    }
                    yield return null;
                }
            }
            OnEndAllCreated?.Invoke();
        }
        private TObject Create(Transform storageLocation, TObjectTemplate template, System.Func<TObject, TObjectTools> distributor)
        {
            TObject answer = BaseObject.Create<TObject, TObjectTools>(template, storageLocation, distributor) as TObject;
            answer.OnReturn += () => Returning(answer);

            return answer;
        }
        private void Returning(TObject obj)
        {
            obj.transform.parent = _storageLocation;
            _freeForUse.Enqueue(obj);
        }

        [System.Serializable] public struct Package
        {
            public ISpawner Spawner { get; private set; }
            public TObjectTemplate Template { get; private set; }
            public System.Func<TObject, TObjectTools> Distributor { get; private set; }
            public int NumberObjsChangeFrame { get; private set; }
            
            public Package(ISpawner spawner, TObjectTemplate template, System.Func<TObject, TObjectTools> distributor, int numberObjsChangeFrame = 1)
            {
                Spawner = spawner;
                Template = template;
                Distributor = distributor;
                NumberObjsChangeFrame = numberObjsChangeFrame;
            }
        }
    }
}
