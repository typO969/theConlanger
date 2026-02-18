using System;
using System.Collections.Generic;
using System.Linq;
using ConlangBuilder.Core;

namespace UI.WPF.ViewModels;

public sealed class PluginArgumentViewModel : ViewModelBase
{
    public string Name { get; }
    public string Label { get; }
    public string Type { get; }
    public bool Required { get; }
    public string? Description { get; }
    public string? DefaultValue { get; }
    public IReadOnlyList<string> Options { get; }

    public bool HasOptions => Options.Count > 0;
    public bool IsBoolean => NormalizeType(Type) is "bool" or "boolean";
    public bool IsNumber => NormalizeType(Type) is "int" or "integer" or "double" or "float" or "number";

    private string _value;
    public string Value { get => _value; set { _value = value; Raise(nameof(Value)); Raise(nameof(BoolValue)); } }

    public bool BoolValue
    {
        get => bool.TryParse(Value, out var parsed) && parsed;
        set => Value = value.ToString().ToLowerInvariant();
    }

    public PluginArgumentViewModel(PluginArgumentDefinition definition)
    {
        Name = definition.Name;
        Label = string.IsNullOrWhiteSpace(definition.Label) ? definition.Name : definition.Label;
        Type = definition.Type;
        Required = definition.Required;
        Description = definition.Description;
        DefaultValue = definition.DefaultValue;
        Options = definition.Options?.ToList() ?? new List<string>();
        _value = definition.DefaultValue ?? string.Empty;
    }

    private static string NormalizeType(string? type) => type?.Trim().ToLowerInvariant() ?? "string";
}
