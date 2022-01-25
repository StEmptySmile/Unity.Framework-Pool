using UnityEngine;

namespace Core.Pool.Object
{
    public abstract class Template<TObject> : ScriptableObject
        where TObject : MonoBehaviour
    {
        public TObject Object => _object;
        [SerializeField] private TObject _object = null;
    }
}