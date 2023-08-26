using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NitariCupBackend.Models;

namespace NitariCupBackend.Server.Data
{
    public class NitariCupBackendServerContext : DbContext
    {
        public NitariCupBackendServerContext (DbContextOptions<NitariCupBackendServerContext> options)
            : base(options)
        {
        }

        public DbSet<NitariCupBackend.Models.TaskScheme> TaskScheme { get; set; } = default!;
    }
}
