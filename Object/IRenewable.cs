namespace Core.Pool.Object
{
    public interface IRenewable
    {
        public event System.Action OnReturn;
        public void Return();
    }
}
