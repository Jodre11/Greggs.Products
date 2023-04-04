namespace Greggs.Products.Api.Models
{
    public class LocalProduct : Product, ILocalProduct
    {
        public decimal PriceInLocal { get ; set ; }
    }
}
