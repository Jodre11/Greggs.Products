namespace Greggs.Products.Api.DataAccess
{
    public interface IDataAccessSingle<out T, in TKey>
    {
        T Get(TKey key);
    }
}
