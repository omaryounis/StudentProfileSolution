using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContext Context;
        public UnitOfWork(DbContext _context)
        {
            Context = _context;
        }

        public int Complete()
        {
            return Context.SaveChanges();
        }
    }
}
