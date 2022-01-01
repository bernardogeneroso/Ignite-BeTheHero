using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using Persistence;

namespace Application.Records;

public class Edit
{
    public class Command : IRequest<Result<Unit>>
    {
        public string Id { get; set; }
        public Record Record { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Record).SetValidator(new RecordValidator());
        }
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

            var record = request.Record;

            record.Id = request.Id;

            var filter = Builders<Record>.Filter.Eq(x => x.Id, request.Id);

            await _collection.ReplaceOneAsync(filter, record);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
