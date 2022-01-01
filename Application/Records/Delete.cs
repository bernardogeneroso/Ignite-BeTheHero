using Application.Core;
using Domain;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using Persistence;

namespace Application.Records;

public class Delete
{
    public class Command : IRequest<Result<Unit>>
    {
        public string Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly IMongoCollection<Record> _collection;

        public Handler(MongoDbContext context)
        {
            _collection = context.GetCollection<Record>("records");
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!ObjectId.TryParse(request.Id, out _)) return Result<Unit>.Failure("Invalid id");

            var filter = Builders<Record>.Filter.Eq(x => x.Id, request.Id);

            var result = await _collection.DeleteOneAsync(filter);

            if (result.DeletedCount == 0) return null;

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
