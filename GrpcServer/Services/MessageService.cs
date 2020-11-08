using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcServer.Infrastructure;
using GrpcServer.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using RandomMessage.Protos;

namespace GrpcServer.Services
{
    public class MessageService:RandomMessageService.RandomMessageServiceBase
    {
        private readonly ApplicationDbContext _db;

        public MessageService(ApplicationDbContext db)
        {
            _db = db;
        }

        public override async Task<ReceiveResult> SendRandomMessage(RanodmMessageContent request, ServerCallContext context)
        {
            _db.RandomMessages.Add(new RandomMessageDb
                {SentAt = DateTimeOffset.FromUnixTimeSeconds(request.SentAt).Date, Message = request.Message});

            var saveResult = await _db.SaveChangesAsync();

            return new ReceiveResult{IsReceived = saveResult>0};
        }

        public override async Task<RanodmMessageContent> GetRandomMessage(Empty request, ServerCallContext context)
        {
            var messageCount = await _db.RandomMessages.CountAsync();

            var skip = new Random().Next(0, messageCount - 1);

            var message = await _db.RandomMessages.Skip(skip).Take(1).FirstOrDefaultAsync();
            var date=new DateTimeOffset(message.SentAt);
            return new RanodmMessageContent
            {
                Message = $"{message.Message}",
                SentAt = date.ToUnixTimeMilliseconds()
            };
        }
    }
}
