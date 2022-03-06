namespace Core.Pool.Object
{
    public interface IObjectTemplate<TObject>
        where TObject : BaseObject
    {
        public TObject Object { get; }
    }
}
