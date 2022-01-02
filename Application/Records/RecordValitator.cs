using Domain;
using FluentValidation;

namespace Application.Records;

public class RecordValidator : AbstractValidator<Record>
{
  public RecordValidator()
  {
    RuleFor(x => x.Email).NotEmpty();
    RuleFor(x => x.CaseTitle).NotEmpty();
    RuleFor(x => x.CaseDescription).NotEmpty();
    RuleFor(x => x.CasePrice).GreaterThan(0).NotEmpty();
  }
}
