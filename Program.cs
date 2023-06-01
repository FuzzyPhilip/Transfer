using System.CommandLine;


namespace Beinggs.Transfer;


/// <summary>
/// Implements a simple test data and file transfer command-line utility.
/// </summary>
/// <remarks>
/// See the README.md file for details of operation.
/// </remarks>
public partial class Program
{
	/// <summary>
	/// Defines the entry point for <see cref="Program"/>.
	/// </summary>
	/// <param name="args"></param>
	/// <returns></returns>
	static async Task<int> Main (string[] args)
	{
		// as commands can't be declared with aliases, we have to declare them here and add aliases later <grr!>
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
				optRepeat,

				(sendFileCommand = new (
					name: "file",
					description: "Send a file")
				{
					argFile,
					optIncludeFileName,
					cmdTo
				}),
				(sendTestCommand = new (
					name: "test",
					description: "Send test data")
				{
					optTestSize,
					cmdTo
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
					argFileName,
					cmdFrom
				}),
				(receiveTestCommand = new (
					name: "test",
					description: "Receive test data")
				{
					optMaxSize,
					optMaxTime,
					cmdFrom
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
		globalOptVerbosity.Arity = ArgumentArity.ZeroOrOne;	// verbosity with no value defaults to verbose
		globalOptMeasured.Arity = ArgumentArity.ZeroOrOne;	// measured with no value defaults to measured

		rootCommand.AddGlobalOption (globalOptVerbosity);
		rootCommand.AddGlobalOption (globalOptMeasured);
		rootCommand.AddGlobalOption (globalOptPort);

		// ... set handlers...
		// rootCommand.SetHandler (SetLogLevel, verbosity); // GRR! This should be supported, at least for globals! :-/
		cmdTo.SetHandler (ToCommand, globalOptVerbosity, globalOptMeasured, globalOptPort, optRepeat,
				argFile, optIncludeFileName, optTestSize, argRecipient);

		cmdFrom.SetHandler (FromCommand, globalOptVerbosity, globalOptMeasured, globalOptPort,
				argFileName, optMaxSize, optMaxTime, argSender);

		// ... and let the magic happen!
		return await rootCommand.InvokeAsync (args);
	}
}
