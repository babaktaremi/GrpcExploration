using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcServer.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace GrpcServer.Infrastructure
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<RandomMessageDb> RandomMessages { get; set; }
    }
}
