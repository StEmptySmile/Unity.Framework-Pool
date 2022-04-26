using UnityEngine;

namespace ESUnity.Pool.Object
{
    public abstract class MonoPoolObject<TTools, TTemplate> : MonoBehaviour 
        where TTools : IObjectTools
        where TTemplate : IObjectTemplate<MonoPoolObject<TTools, TTemplate>>
    {
        public event System.Action OnReturn;
        public TTools Tools { get; private set; }
        public TTemplate Template { get; private set; }

        public static TObject Create<TObject>(TTemplate template, Transform storage, System.Func<TObject, TTools> distributor)
            where TObject : MonoPoolObject<TTools, TTemplate>
        {
            TObject answer = Instantiate(template.Object, storage) as TObject;
            answer.Template = template;
            answer.Tools = distributor.Invoke(answer);
            answer.Construct();
            return answer;
        }
        protected abstract void Construct();
        public void Return()
        {
            OnReturn?.Invoke();
        }
    }
}