using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AppointmentsApi.UnitTests.Shared
{
    public class FakeDbSet<TEntity> : DbSet<TEntity>, IQueryable<TEntity>, IAsyncEnumerable<TEntity>
        where TEntity : class
    { // Adapted from https://gist.github.com/axelheer/bdbbd2f92600a45f22d6
        private readonly IEnumerable<PropertyInfo> keys;
        private readonly List<TEntity> items;
        private readonly IQueryable<TEntity> query;

        public FakeDbSet()
        {
            keys = typeof(TEntity).GetProperties()
                                  .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute))
                                           || "Id".Equals(p.Name, StringComparison.Ordinal))
                                  .ToList();

            items = new List<TEntity>();
            query = items.AsQueryable();
        }

        public TEntity[] InnerItems => items.ToArray();

        public override EntityEntry<TEntity> Add(TEntity entity)
        {
            //return base.Add(entity);
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (keys.Any(k => k.GetValue(entity) == null))
                throw new ArgumentOutOfRangeException("entity");

            items.Add(entity);
            return null;
        }

        public override void AddRange(IEnumerable<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("entities");

            items.AddRange(entities);
        }

        public override void AddRange(params TEntity[] entities)
        {
            AddRange(entities.AsEnumerable());
        }

        public override TEntity Find(params object[] keyValues)
        {
            if (keyValues == null)
                throw new ArgumentNullException("keyValues");
            if (keyValues.Any(k => k == null))
                throw new ArgumentOutOfRangeException("keyValues");
            if (keyValues.Length != keys.Count())
                throw new ArgumentOutOfRangeException("keyValues");

            return items.SingleOrDefault(i =>
                keys.Zip(keyValues, (k, v) => v.Equals(k.GetValue(i)))
                    .All(r => r)
            );
        }

        public override EntityEntry<TEntity> Remove(TEntity entity)
        {
            items.Remove(entity);
            return null;
        }

        public Type ElementType
        {
            get { return query.ElementType; }
        }

        public Expression Expression
        {
            get { return query.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return query.Provider; }
        }

        public override IEntityType EntityType => throw new NotImplementedException();

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return query.GetEnumerator();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return query.GetEnumerator();
        }

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator()
        {
            return ((IAsyncEnumerable<TEntity>)query).GetAsyncEnumerator();
        }
    }

    public static class FakeDbFunctions
    {
        public static int? CharIndex(string toSearch, string target)
        {
            return CharIndex(toSearch, target, null);
        }

        public static int? CharIndex(string toSearch, string target, int? startLocation)
        {
            if (toSearch == null)
                return null;
            if (target == null)
                return null;

            return target.IndexOf(toSearch, (startLocation ?? 1) - 1, StringComparison.CurrentCultureIgnoreCase) + 1;
        }

        public static int? PatIndex(string textPattern, string target)
        {
            if (textPattern == null)
                return null;
            if (target == null)
                return null;

            var pattern = Regex.Escape(textPattern)
                               .Replace("_", ".")
                               .Replace("\\[.]", "_")
                               .Replace("%", ".*")
                               .Replace("\\[.*]", "%")
                               .Replace("\\[\\[]", "\\[");

            var match = Regex.Match(target, "(?i)^" + pattern + "$");
            return match.Success ? match.Index + 1 : 0;
        }
    }
}