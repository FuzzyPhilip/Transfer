// Taken from System.CommandLine.AnonymousCommandHandler, as it's internal

using System.CommandLine.Invocation;


namespace System.CommandLine;


class AnonymousCommandHandler: ICommandHandler
{
	#region Fields

	readonly Func<InvocationContext, Task>? _asyncHandle;
	readonly Action<InvocationContext>? _syncHandle;

	#endregion Fields

	#region Construction

	public AnonymousCommandHandler (Func<InvocationContext, Task> handle)
		=> _asyncHandle = handle ?? throw new ArgumentNullException (nameof (handle));

	public AnonymousCommandHandler (Action<InvocationContext> handle)
		=> _syncHandle = handle ?? throw new ArgumentNullException (nameof (handle));

	#endregion Construction

	#region ICommandHandler implementation

	public int Invoke (InvocationContext context)
	{
		if (_syncHandle is not null)
		{
			_syncHandle (context);

			return context.ExitCode;
		}

		return syncUsingAsync (context);

		// kept in a separate method to avoid JITting
		int syncUsingAsync (InvocationContext context)
			=> InvokeAsync (context).GetAwaiter().GetResult();
	}

	public async Task<int> InvokeAsync (InvocationContext context)
	{
		if (_syncHandle is not null)
			return Invoke (context);

		object returnValue = _asyncHandle! (context);

		return returnValue switch
		{
			Task<int> exitCodeTask => await exitCodeTask,
			Task task => await getExitCode (task),
			int exitCode => exitCode,
			_ => context.ExitCode
		};

		// local helper
		async Task<int> getExitCode (Task task)
		{
			await task;

			return context.ExitCode;
		}
	}

	#endregion ICommandHandler implementation
}
