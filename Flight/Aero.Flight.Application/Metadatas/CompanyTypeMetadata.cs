using Aero.Flights.Contracts.Metadatas;
using Aero.Flights.Domain.Metadatas;
using FluentValidation;
using System.Linq.Expressions;

namespace Aero.Flights.Application.Metadatas;

public sealed class CompanyTypeRequestValidator : AbstractValidator<CompanyTypeRequest>
{
    public CompanyTypeRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
    }
}

public sealed class CompanyTypeMetadataMap : IMetadataMap<CompanyType, CompanyTypeRequest, CompanyTypeResponse>
{
    public string GetCode(CompanyType entity) => entity.Id;

    public CompanyType Create(CompanyTypeRequest r) => CompanyType.Create(r.Code, r.Description);

    public void Apply(CompanyType e, CompanyTypeRequest r) => e.Update(r.Description);

    public CompanyTypeResponse ToResponse(CompanyType e) => new(e.Id, e.Description);

    public Expression<Func<CompanyType, string>> CodeSelector { get; } = e => e.Id;

    public Expression<Func<CompanyType, CompanyTypeResponse>> Projection { get; } =
        e => new CompanyTypeResponse(e.Id, e.Description);
}
