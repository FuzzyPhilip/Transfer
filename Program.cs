using System.CommandLine;


namespace Beinggs.Transfer;


// see README.md for details
partial class Program
{
	/// <summary>
	/// Defines the entry point for <see cref="Program"/>.
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>
	static async Task<int> Main (string[] args)
	{
		// as commands can't be declared with aliases, we have to declare them here and add aliases after <grr!>
		Command sendCommand;
		Command sendFileCommand;
		Command sendTestCommand;

		Command receiveCommand;
		Command receiveFileCommand;
		Command receiveTestCommand;

		// build the command line...
		RootCommand rootCommand = new ("Simple transfer app")
		{
			(sendCommand = new (
				name: "send",
				description: "Send a file or test data")
			{
				repeat,

				(sendFileCommand = new (
					name: "file",
					description: "Send a file")
				{
					file,
					includeFileName,
					toCommand
				}),
				(sendTestCommand = new (
					name: "test",
					description: "Send test data")
				{
					testSize,
					toCommand
				})
			}),

			(receiveCommand = new (
				name: "receive",
				description: "Receive a file or test data")
			{
				(receiveFileCommand = new (
					name: "file",
					description: "Receive a file")
				{
					fileName,
					fromCommand
				}),
				(receiveTestCommand = new (
					name: "test",
					description: "Receive test data")
				{
					maxSize,
					fromCommand
				})
			})
		};

		// ... add aliases...
		sendCommand.AddAlias ("s");
		sendFileCommand.AddAlias ("f");
		sendTestCommand.AddAlias ("t");

		receiveCommand.AddAlias ("r");
		receiveFileCommand.AddAlias ("f");
		receiveTestCommand.AddAlias ("t");

		// ... add globals...
		rootCommand.AddGlobalOption (timeout);
		rootCommand.AddGlobalOption (measured);

		// ... set handlers...
		toCommand.SetHandler (ToCommand, timeout, measured, repeat, file, includeFileName, testSize, recipient);
		fromCommand.SetHandler (FromCommand, timeout, measured, fileName, maxSize, sender);

		// ... and let the magic happen here!
		return await rootCommand.InvokeAsync (args);
	}
}
