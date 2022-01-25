using UnityEngine;

namespace Core.Pool.Object
{
    public abstract class BaseObject<TSignature, TTools> : MonoBehaviour, IRenewable
        where TSignature : ScriptableObject
        where TTools : ToolsObject
    {
        public TSignature Signature { get; private set; }
        public event System.Action OnReturn;

        public static TObject Create<TObject>(TObject prefab, Transform storageLocation, TSignature signature, System.Func<TObject, TTools> distributor)
            where TObject : BaseObject<TSignature, TTools>
        {
            TObject answer = Instantiate(prefab, storageLocation);
            answer.Construct(signature, distributor.Invoke(answer));
            return answer;
        }
        protected virtual void Construct(TSignature signature, TTools tools)
        {
            Signature = signature;
        }
        public virtual void Return()
        {
            OnReturn?.Invoke();
        }
    }   
}
