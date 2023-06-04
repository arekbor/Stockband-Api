using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Stockband.Domain.Common;
using Stockband.Domain.Exceptions;

namespace Stockband.Infrastructure.Common;

public static class SoftDeleteQueryExtension
{
    public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityType)
    {
        MethodInfo? methodInfo = typeof(SoftDeleteQueryExtension).GetMethod(nameof(GetSoftDeleteFilter),
         BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityType.ClrType);

        if (methodInfo == null)
        {
            throw new ObjectNotFound(typeof(MethodInfo));
        }

        object? filter = methodInfo.Invoke(null, new object[] { })!;
        if (filter == null)
        {
            throw new ObjectNotFound(nameof(filter));
        }

        entityType.SetQueryFilter((LambdaExpression)filter);
        entityType.AddIndex(entityType.FindProperty(nameof(AuditEntity.Deleted)) ?? throw new ObjectNotFound(typeof(AuditEntity)));

    }
    
    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : AuditEntity
    {
        Expression<Func<TEntity, bool>> filter = x => !x.Deleted;
        return filter;
    }
}