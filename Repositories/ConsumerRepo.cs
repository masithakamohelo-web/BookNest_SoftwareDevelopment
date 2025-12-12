using ASPNETCore_DB.Data;
using ASPNETCore_DB.Interfaces;
using ASPNETCore_DB.Models;
using System.Linq;

namespace ASPNETCore_DB.Repositories
{
    public class ConsumerRepo : IConsumer
    {
        private readonly SQLiteDBContext _context;

        public ConsumerRepo(SQLiteDBContext context)
        {
            _context = context;
        }

        public Consumer Create(Consumer consumer)
        {
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));

            consumer.RegistrationDate = DateTime.Now;
            _context.Add(consumer);
            _context.SaveChanges();
            return consumer;
        }

        public bool Delete(Consumer consumer)
        {
            _context.Remove(consumer);
            _context.SaveChanges();
            return !IsExist(consumer.ConsumerId);
        }

        public Consumer Details(string id)
        {
            return _context.Consumers?.FirstOrDefault(x => x.ConsumerId == id);
        }

        public Consumer ByEmail(string id)
        {
            return _context.Consumers?.FirstOrDefault(x => x.Email == id);
        }

        public Consumer Edit(Consumer consumer)
        {
            _context.Update(consumer);
            _context.SaveChanges();
            return consumer;
        }

        public IQueryable<Consumer> GetConsumers(string searchString, string sortOrder)
        {
            var consumers = _context.Consumers.ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                consumers = consumers.Where(c =>
                    c.ConsumerId.Contains(searchString) ||
                    c.Name.Contains(searchString) ||
                    c.Email.Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "id_desc":
                    consumers = consumers.OrderByDescending(c => c.ConsumerId).ToList();
                    break;
                case "name_desc":
                    consumers = consumers.OrderByDescending(c => c.Name).ToList();
                    break;
                case "Date":
                    consumers = consumers.OrderBy(c => c.RegistrationDate).ToList();
                    break;
                case "date_desc":
                    consumers = consumers.OrderByDescending(c => c.RegistrationDate).ToList();
                    break;
                default:
                    consumers = consumers.OrderBy(c => c.Name).ToList();
                    break;
            }

            return consumers.AsQueryable();
        }

        public bool IsExist(string id)
        {
            return Details(id) != null;
        }
    }
}