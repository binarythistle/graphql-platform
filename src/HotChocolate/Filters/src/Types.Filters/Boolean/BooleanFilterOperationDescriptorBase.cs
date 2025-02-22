using System;
using HotChocolate.Language;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.Types.Filters;

[Obsolete("Use HotChocolate.Data.")]
public class BooleanFilterOperationDescriptorBase
    : FilterOperationDescriptorBase
    , IBooleanFilterOperationDescriptorBase
{
    protected BooleanFilterOperationDescriptorBase(
        IDescriptorContext context,
        string name,
        TypeReference type,
        FilterOperation operation)
        : base(context)
    {
        Definition.Name = name;
        Definition.Type = type ?? throw new ArgumentNullException(nameof(type));
        Definition.Operation = operation ?? throw new ArgumentNullException(nameof(operation));
    }

    /// <inheritdoc/>
    public new IBooleanFilterOperationDescriptorBase Name(string value)
    {
        base.Name(value);
        return this;
    }

    /// <inheritdoc/>
    public new IBooleanFilterOperationDescriptorBase Description(
        string value)
    {
        base.Description(value);
        return this;
    }

    /// <inheritdoc/>
    public new IBooleanFilterOperationDescriptorBase Directive<T>(
        T directiveInstance)
        where T : class
    {
        base.Directive(directiveInstance);
        return this;
    }

    /// <inheritdoc/>
    public new IBooleanFilterOperationDescriptorBase Directive<T>()
        where T : class, new()
    {
        base.Directive<T>();
        return this;
    }

    /// <inheritdoc/>
    public new IBooleanFilterOperationDescriptorBase Directive(
        string name,
        params ArgumentNode[] arguments)
    {
        base.Directive(name, arguments);
        return this;
    }
}
