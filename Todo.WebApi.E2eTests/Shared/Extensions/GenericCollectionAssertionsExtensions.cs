using FluentAssertions;
using FluentAssertions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Todo.Application.Common.Models;
using Todo.Domain.Common;

namespace Todo.WebApi.E2eTests.Shared.Extensions;

public static class GenericCollectionAssertionsExtensions
{
    private static readonly string[] _propertyNamesToIgnore = new string[]
    {
        nameof(IEntity.Id),
        nameof(AuditableEntity.CreatedBy),
        nameof(AuditableEntity.CreatedAt),
        nameof(AuditableEntity.LastUpdatedBy),
        nameof(AuditableEntity.LastUpdatedAt),
    };

    public static AndConstraint<GenericCollectionAssertions<T>> BeEquivalentToIgnoringIdAndAudit<T>(
        this GenericCollectionAssertions<T> actual,
        IEnumerable<T> expected)
        where T : IEntity, IAuditableDto
    {
        return actual.BeEquivalentTo(expected, options =>
            options.Excluding(memberInfo => _propertyNamesToIgnore.Contains(memberInfo.Name)));
    }
}