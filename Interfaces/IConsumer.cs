using ASPNETCore_DB.Models;
using System.Linq;

namespace ASPNETCore_DB.Interfaces
{
    public interface IConsumer
    {
        IQueryable<Consumer> GetConsumers(string searchString, string sortOrder);
        Consumer Details(string id);
        Consumer ByEmail(string id);
        Consumer Create(Consumer consumer);
        Consumer Edit(Consumer consumer);
        bool Delete(Consumer consumer);
        bool IsExist(string id);
    }
}