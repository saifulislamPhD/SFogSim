﻿using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace i4o
{
    public class IndexSpecification<T>
    {
        public Collection<string> IndexedProperties { get; private set; }

        public IndexSpecification()
        {
            IndexedProperties = new Collection<string>();
        }

        public IndexSpecification<T> Add<TProperty>(Expression<Func<T, TProperty>> propertyExpressions)
        {
            var value = propertyExpressions.GetMemberName();

            // Should only add property once
            if (!IndexedProperties.Contains(value))
                this.IndexedProperties.Add(value);

            return this;
        }

        public IndexSpecification<T> Remove<TProperty>(Expression<Func<T, TProperty>> propertyExpressions)
        {
            var value = propertyExpressions.GetMemberName();

            this.IndexedProperties.Remove(value);

            return this;
        }
    }
}
