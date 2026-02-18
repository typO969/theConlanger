using System.Collections.Generic;

namespace UI.WPF.Project;

public sealed class ProjectHistory
{
    private readonly int _capacity;
    private readonly Stack<LanguageProjectState> _undo = new();
    private readonly Stack<LanguageProjectState> _redo = new();

    public ProjectHistory(int capacity = 50) => _capacity = capacity < 1 ? 1 : capacity;

    public void Push(LanguageProjectState state, string _label, bool suppressDuplicate = false)
    {
        if (!suppressDuplicate && _undo.TryPeek(out var current) && current.SemanticallyEquals(state))
            return;

        _undo.Push(state.Clone());
        while (_undo.Count > _capacity)
        {
            var items = _undo.ToArray();
            _undo.Clear();
            for (var i = items.Length - 2; i >= 0; i--)
                _undo.Push(items[i]);
        }

        _redo.Clear();
    }

    public LanguageProjectState? Undo(LanguageProjectState current)
    {
        if (_undo.Count <= 1)
            return null;

        _redo.Push(current.Clone());
        _undo.Pop();
        return _undo.Peek().Clone();
    }

    public LanguageProjectState? Redo(LanguageProjectState current)
    {
        if (_redo.Count == 0)
            return null;

        _undo.Push(current.Clone());
        return _redo.Pop().Clone();
    }
}
