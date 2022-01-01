using Application.Core;
using Domain;
using MediatR;
using MongoDB.Driver;
using Persistence;

namespace Application.Records
{
    public class List
    {
        public class Query : IRequest<Result<List<Record>>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<List<Record>>>
        {
            private readonly IMongoCollection<Record> _collection;
            public Handler(MongoDbContext context)
            {
                _collection = context.GetCollection<Record>("records");
            }

            public async Task<Result<List<Record>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var records = await _collection.Find(_ => true).ToListAsync();

                return Result<List<Record>>.Success(records);
            }
        }
    }
}