using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GrpcServer.Infrastructure.Model
{
    public class RandomMessageDb
    {
        [Key]
        public Guid Id { get; set; }

        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }
}
