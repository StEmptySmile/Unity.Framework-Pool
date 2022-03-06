using UnityEngine;

namespace Core.Pool.Object
{
    public abstract class BaseObject : MonoBehaviour
    {
        public IObjectTools Tools { get; protected set; }
        public IObjectTemplate<BaseObject> Template { get; protected set; }
        public event System.Action OnReturn;

        protected virtual void Construct() { }
        public static TObject Create<TObject, TTools>(IObjectTemplate<TObject> template, Transform storageLocation, System.Func<TObject, TTools> distributor)
            where TObject : BaseObject
        {
            TObject answer = Instantiate(template.Object, storageLocation);
            answer.Template = template as IObjectTemplate<BaseObject>;
            answer.Tools = distributor.Invoke(answer) as IObjectTools;
            answer.Construct();
            return answer;
        }
        public void Return()
        {
            OnReturn?.Invoke();
        }
    }
}