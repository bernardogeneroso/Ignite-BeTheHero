using Application.Core;
using Domain;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using Persistence;

namespace Application.Records;

public class Details
{
    public class Query : IRequest<Result<Record>>
    {
        public string Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Result<Record>>
    {
        private readonly IMongoCollection<Record> _collection;

        public Handler(MongoDbContext context)
        {
            _collection = context.GetCollection<Record>("records");
        }

        public async Task<Result<Record>> Handle(Query request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.Id, out _)) return Result<Record>.Failure("Invalid id");

            var filter = Builders<Record>.Filter.Eq(x => x.Id, request.Id);

            var record = await _collection.Find(filter).FirstOrDefaultAsync();

            if (record == null) return null;

            return Result<Record>.Success(record);
        }
    }
}
