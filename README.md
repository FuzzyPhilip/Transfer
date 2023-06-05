# Transfer Utility

This command line utility transfers files or test data between two machines.

## Syntax

Example syntax (implemented using `System.CommandLine` functionality):

``` shell
transfer [--measured[=true]] send file fileName [--include-filename=true] to anyone [--repeat=false]
transfer [--measured[=true]] send [test] to clientMachine [--size=10] [--repeat=false]
transfer [--measured[=true]] receive file [fileName] from serverMachine
transfer [--measured[=true]] receive [test] [--max-size=X] from serverMachine
```

*Where:*

- `[]` indicates an optional command-line option
- `=value` indicates the default value for an optional command-line option

### Options

This section provides an overview of options; run with `-h` to see more details.

#### Global Options

`-h`, `/h`, `-?`, `/?`, `--help`: Shows global or command-specific help

`-v`, `--verbosity`: Specifies the level of detail output to the console

`-m`, `/m`, `--measured`: Writes performance and timing stats to the console

`-p`, `/p`, `--port`: Specifies the port to use for sending or receiving

#### Send Options

`-r`, `/r`, `--repeat`: Repeats file sending (by default the utility quits after sending the file or test data)

`-f`, `/f`, `--include-filename`: Includes the filename as a 'header' at the start of the file data (see below)

`-s`, `/s`, `--size`: Specifies the size of test data to send in MB, defaulting to 10, up to 10240 (10 GB)

#### Receive Options

`-m`, `/m`, `--max-size`: Specifies the maximum size of data to recieve, in MB

`-m`, `/m`, `--max-time`: Specifies the maximum time to wait for data to be received, in seconds

`--version`: Shows the version number

## Operation

Use `-h` or `/h` to get help for specific commands, e.g.:

``` shell
transfer -h  // shows general help
transfer send -h // shows help for the send command, listing its sub-commands and applicable options
transfer receive test -h // shows help for receiving test data and applicable options
etc.
```

The `test` command is implicit, so `transfer send to anyone` will send test data.

A `send` command will wait for a connection and then send the specified file or test data once a
valid client has connected.

A recipient can be a specific client machine name or IP address, or the name `anyone` (aliases:
`any` or `a`), in which case connects are accepted from any client.

The `--include-filename` option for the `send file` command, which is on by default, prepends the
'header' string `filename:name\n`, where `name` is the file's name, which is terminiated with a
newline. The `filename` header uses UTF8 encoding.

The `receive file` command looks for the `filename` header in the incoming file data and
automatically creates the file (if it doesn't already exist) with that name. If the incoming file
data doesn't start with the `filename` header the received data will be saved in the file named
`transfer.dat` in the current directory.
