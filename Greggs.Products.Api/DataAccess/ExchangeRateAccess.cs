using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.DataAccess
{
    public class ExchangeRateAccess : IDataAccessSingle<ExchangeRate, string>
    {
        public ExchangeRate Get(string locale)
        {
            // Obviously a silly implementation
            return new ExchangeRate(locale, 1.11m);
        }
    }
}
