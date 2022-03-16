using UnityEngine;

namespace Core.Pool.Object
{
    public interface IObjectTemplate<out TObject> 
        where TObject : MonoBehaviour
    {
        public TObject Object { get; }
    }
}
