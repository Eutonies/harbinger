using Harbinger.Domain.Util;
using Harbinger.Domain.Model;
using Harbinger.Domain.Generation;

namespace Harbinger.UI.Pages.Frame;

public partial record FrameSessionVmo
{
    private const string DisabledName = "disabled";
    private readonly SemaphoreSlim _tasksLock = new SemaphoreSlim(1, 1);
    private readonly HashSet<Guid> _currentTasks = new HashSet<Guid>();
    private readonly SemaphoreSlim _buttonPushLock = new SemaphoreSlim(1, 1);

    public bool UpdateButtonIsPushed => _buttonPushLock.CurrentCount == 0;
    public bool IsDisabled => _currentTasks.Any() || UpdateButtonIsPushed;

    public string? DisabledAttr => IsDisabled ? DisabledName : null;

    public bool IsDisabledWith(Func<bool> predicate) => IsDisabledWithAll(predicate);

    public bool IsDisabledWithAll(params IEnumerable<Func<bool>> predicates)
    {
        if (IsDisabled)
            return true;
        foreach (var pred in predicates)
            if (pred())
                return true;
        return false;
    }

    public string? DisabledAttrWithAll(params IEnumerable<Func<bool>> preds) => IsDisabledWithAll(preds) ? DisabledName : null;

    public string? DisabledAttrWith(Func<bool> pred) => DisabledAttrWithAll(pred);



    public async Task DoWhileDisabledAsync(Func<Task> toDo) => _ = await WhileDisabled(async () =>
    {
        await toDo();
        return 0;
    });
    public void DoWhileDisabled(Action toDo) => _ = WhileDisabled(() =>
    {
        toDo();
        return 0;
    });

    

    public Task DoWithUpdateButtonPushedAsync(Func<Task> toDo) => WithUpdateButtonPushedAsync(async () =>
    {
        await toDo();
        return 0;
    });

    public async Task<TOut> WithUpdateButtonPushedAsync<TOut>(Func<Task<TOut>> toDo) 
    {
        await _tasksLock.WaitAsync();
        try
        {
            var returnee = await toDo();
            return returnee;
        }
        finally
        {
            _tasksLock.Release();
        }
    }


    public async Task<TOut> WhileDisabledAsync<TOut>(Func<Task<TOut>> toDo)
    {
        var taskId = Guid.NewGuid();
        await _tasksLock.WaitAsync();
        try
        {
            _currentTasks.Add(taskId);
            var returnee = await toDo();
            return returnee;
        }
        finally
        {
            _currentTasks.Remove(taskId);
            _tasksLock.Release();
        }
    }

    public TOut WhileDisabled<TOut>(Func<TOut> toDo)
    {
        var taskId = Guid.NewGuid();
        _tasksLock.Wait();
        try
        {
            _currentTasks.Add(taskId);
            var returnee = toDo();
            return returnee;
        }
        finally
        {
            _currentTasks.Remove(taskId);
            _tasksLock.Release();
        }
    }






}
