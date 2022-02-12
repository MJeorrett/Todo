using FluentAssertions;
using FluentAssertions.Collections;
using System.Collections.Generic;
using System.Linq;
using Todo.Application.Common.Models;

namespace Todo.WebApi.E2eTests.Shared.Assertions;

public static class AuditableEntityDtoCollectionExtensions
{
    public static AuditableEntityDtoCollectionAssertions<T> Should<T>(this IEnumerable<T> instance)
        where T : AuditableEntityDto
    {
        return new AuditableEntityDtoCollectionAssertions<T>(instance);
    }
}

public class AuditableEntityDtoCollectionAssertions<T> : GenericCollectionAssertions<T>
    where T : AuditableEntityDto
{
    public AuditableEntityDtoCollectionAssertions(IEnumerable<T> actualValue) : base(actualValue)
    {
    }

    public AndConstraint<AuditableEntityDtoCollectionAssertions<T>> EqualIgnoringIdAndAuditProperties(IEnumerable<T> expected)
    {
        var _expected = expected.Select(_ => _ with { Id = 0, CreatedAt = default, CreatedBy = "", LastUpdatedAt = default, LastUpdatedBy = "" });
        var _actual = Subject.Select(_ => _ with { Id = 0, CreatedAt = default, CreatedBy = "", LastUpdatedAt = default, LastUpdatedBy = "" });

        _actual.Should().Equal(_expected);

        return new AndConstraint<AuditableEntityDtoCollectionAssertions<T>>(this);
    }
}