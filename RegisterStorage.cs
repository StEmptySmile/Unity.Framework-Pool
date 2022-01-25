using System.Collections.Generic;
using Core.Pool.Object;
using UnityEngine;

namespace Core.Pool
{
    public sealed class RegisterStorage<TObject, TObjectTools, TSignature>
        where TObject : BaseObject<TSignature, TObjectTools>
        where TObjectTools : ToolsObject
        where TSignature : Template<TObject>
    {
        public IReadOnlyDictionary<TSignature, Storage<TObject, TObjectTools, TSignature>> Storages => _storages;
        
        private readonly MonoBehaviour _sender;
        private readonly Transform _storageLocation; 
        private readonly Dictionary<TSignature, Storage<TObject, TObjectTools, TSignature>> _storages;
        
        public RegisterStorage(MonoBehaviour sender, Transform storageLocation)
        {
            _sender = sender;
            _storageLocation = storageLocation;
            _storages = new Dictionary<TSignature, Storage<TObject, TObjectTools, TSignature>>();
        }
        public void SendRegister(Storage<TObject, TObjectTools, TSignature>.Package package, int numberObjsChangeFrame = 1)
        {
            if(_storages.ContainsKey(package.Signature) == false)
            {
                Storage<TObject, TObjectTools, TSignature> storage = new Storage<TObject, TObjectTools, TSignature>(
                    _sender,
                    _storageLocation,
                    package,
                    numberObjsChangeFrame
                );
                _storages.Add(package.Signature, storage);
            }
        }
    }
}
