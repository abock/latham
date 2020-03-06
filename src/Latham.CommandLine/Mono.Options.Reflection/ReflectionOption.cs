//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Mono.Options.Reflection
{
    sealed class ReflectionOption : Option
    {
        public ReflectionCommand Command { get; }
        public PropertyInfo Property { get; }

        public ReflectionOption(
            ReflectionCommand command,
            OptionAttribute attribute,
            PropertyInfo property)
            : base(
                attribute.Prototype,
                attribute.Description,
                maxValueCount: 1,
                hidden: attribute.Hidden)
        {
            Command = command;
            Property = property;
        }

        protected override void OnParseComplete(OptionContext c)
        {
            string? stringValue = c.OptionValues[0];
            object? convertedValue;

            var propertyType = Property.PropertyType;
            var propertyIsList = false;

            if (propertyType.IsConstructedGenericType)
            {
                var genericTypeDefinition = propertyType.GetGenericTypeDefinition();
                var genericArgument = propertyType.GenericTypeArguments[0];

                if (genericTypeDefinition == typeof(Nullable<>))
                {
                    propertyType = genericArgument;
                }
                else if (typeof(IList<>)
                    .MakeGenericType(genericArgument)
                    .IsAssignableFrom(propertyType))
                {
                    propertyType = genericArgument;
                    propertyIsList = true;
                }
            }

            if (propertyType == typeof(bool))
            {
                convertedValue = stringValue is string;
            }
            else
            {
                convertedValue = TypeDescriptor
                    .GetConverter(propertyType)
                    .ConvertFromString(stringValue);
            }

            if (propertyIsList)
            {
                var addMethod = Property.PropertyType.GetMethod(
                    "Add",
                    new [] { propertyType });

                if (addMethod is null)
                    throw new InvalidOperationException("should not be reached");

                addMethod.Invoke(
                    Property.GetValue(Command),
                    new [] { convertedValue });
            }
            else
            {
                Property.SetValue(Command, convertedValue);
            }
        }
    }
}
