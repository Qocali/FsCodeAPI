using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Repository
{
    public interface IGenericRepo<T>
    {
        Task<List<T>> Read();
        Task<T> GetById(int id);
        Task Create(T endpoint);
        Task Update(Reminder reminder);
        Task Delete(int id);
    }
}
