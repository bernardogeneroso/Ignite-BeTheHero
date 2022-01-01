using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using MongoDB.Driver;
using Persistence;

namespace Application.Records;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
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
            var record = request.Record;

            record.RoomId = Guid.NewGuid();

            await _collection.InsertOneAsync(record);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}
