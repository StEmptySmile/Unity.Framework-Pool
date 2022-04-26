using UnityEngine;

namespace ESUnity.Pool
{
    public interface ISpawner<TTemplate>
    {
        public Transform StorageLocation { get; }
        public TTemplate ObjectInstance { get; }
        public int AmountForStorage { get; } 
    }
}