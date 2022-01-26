using System.Collections.Generic;
using Core.Pool.Object;
using UnityEngine;

namespace Core.Pool
{
    public class Storage<TObject, TObjectTools, TSignature>
        where TObject : BaseObject<TSignature, TObjectTools>
        where TObjectTools : ToolsObject
        where TSignature : Template<TObject>
    {
        public event System.Action OnEndAllCreated;
        public IReadOnlyList<TObject> Objects => _objects;
        public TSignature Signature { get; private set; }

        private readonly int _numberObjsChangeFrame;
        private readonly Transform _storageLocation;
        private readonly Queue<TObject> _freeForUse;
        private readonly List<TObject> _objects;

        public Storage(MonoBehaviour sender,
                        Transform storageLocation, 
                        Package package,
                        int numberObjsChangeFrame)
        {
            Signature = package.Signature;
            _numberObjsChangeFrame = numberObjsChangeFrame;
            _storageLocation = storageLocation;
            _freeForUse = new Queue<TObject>();
            _objects = new List<TObject>();

            sender.StartCoroutine(AllCreate(storageLocation, package));
        }
        public TObject Receive()
        {
            TObject answer = _freeForUse.Dequeue();
            return answer;
        }
        private System.Collections.IEnumerator AllCreate(Transform storageLocation, Package package)
        {
            uint numberOrders = package.AmountForStorage;
            while (numberOrders > 0)
            {
                for(int number = 0; number < _numberObjsChangeFrame; number++)
                {
                    TObject obj = Create(storageLocation, package.Signature, package.Distributor);
                    obj.name += numberOrders;

                    _freeForUse.Enqueue(obj);
                    _objects.Add(obj);
                    numberOrders--;
                }
                yield return null;
            }
            OnEndAllCreated?.Invoke();
        }
        private TObject Create(Transform storageLocation, TSignature signature, System.Func<TObject, TObjectTools> distributor)
        {
            TObject answer = BaseObject<TSignature, TObjectTools>.Create(signature.Object, storageLocation, signature, distributor) as TObject;
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
            public TSignature Signature { get; private set; }
            public uint AmountForStorage { get; private set; } 
            public System.Func<TObject, TObjectTools> Distributor { get; private set; }
            public Package(TSignature signature, uint amountForStorage, System.Func<TObject, TObjectTools> distributor)
            {
                Signature = signature;
                AmountForStorage = amountForStorage;
                Distributor = distributor;
            }
        }
    }
}
