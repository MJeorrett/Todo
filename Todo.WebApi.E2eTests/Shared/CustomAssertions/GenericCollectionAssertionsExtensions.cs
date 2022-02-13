using FluentAssertions;
using FluentAssertions.Collections;
using System.Collections.Generic;
using System.Linq;
using Todo.Application.Common.Models;

namespace Todo.WebApi.E2eTests.Shared.Assertions;

public static class GenericCollectionAssertionsExtensions
{
    public static AndConstraint<GenericCollectionAssertions<T>> EqualIgnoringIdAndAuditProperties<T>(this GenericCollectionAssertions<T> target, IEnumerable<T> expected)
        where T : AuditableEntityDto
    {
        var subject = target.Subject;

        var _expected = expected.Select(_ => _ with { Id = 0, CreatedAt = default, CreatedBy = "", LastUpdatedAt = default, LastUpdatedBy = "" });
        var _actual = subject.Select(_ => _ with { Id = 0, CreatedAt = default, CreatedBy = "", LastUpdatedAt = default, LastUpdatedBy = "" });

        _actual.Should().Equal(_expected);

        return new AndConstraint<GenericCollectionAssertions<T>>(target);
    }
}