using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;


namespace System.CommandLine;


static class CommandExtensions
{
    //
    // Based on System.CommandLine.Handler.SetHandler (... Action) extension, as it only goes up to 8 type parameters!
    //

    /// <summary>
    /// Sets a command's handler based on an <see cref="Action{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/>.
    /// </summary>
    public static void SetHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9> (
            this Command command,
            Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> handle,
            IValueDescriptor<T1> symbol1,
            IValueDescriptor<T2> symbol2,
            IValueDescriptor<T3> symbol3,
            IValueDescriptor<T4> symbol4,
            IValueDescriptor<T5> symbol5,
            IValueDescriptor<T6> symbol6,
            IValueDescriptor<T7> symbol7,
            IValueDescriptor<T8> symbol8,
            IValueDescriptor<T9> symbol9)
        => command.Handler = new AnonymousCommandHandler (
            context =>
            {
                var value1 = GetValueForHandlerParameter (symbol1, context);
                var value2 = GetValueForHandlerParameter (symbol2, context);
                var value3 = GetValueForHandlerParameter (symbol3, context);
                var value4 = GetValueForHandlerParameter (symbol4, context);
                var value5 = GetValueForHandlerParameter (symbol5, context);
                var value6 = GetValueForHandlerParameter (symbol6, context);
                var value7 = GetValueForHandlerParameter (symbol7, context);
                var value8 = GetValueForHandlerParameter (symbol8, context);
                var value9 = GetValueForHandlerParameter (symbol9, context);

                handle (value1!, value2!, value3!, value4!, value5!, value6!, value7!, value8!, value9!);
            });

    //
    // Based on System.CommandLine.Handler.SetHandler (... Func) extension, as it only goes up to 8 type parameters!
    //
    /// <summary>
    /// Sets a command's handler based on an <see cref="Func{T1, T2, T3, T4, T5, T6, T7, T8, T9, Task}"/>.
    /// </summary>
    public static void SetHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9> (
            this Command command,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> handle,
            IValueDescriptor<T1> symbol1,
            IValueDescriptor<T2> symbol2,
            IValueDescriptor<T3> symbol3,
            IValueDescriptor<T4> symbol4,
            IValueDescriptor<T5> symbol5,
            IValueDescriptor<T6> symbol6,
            IValueDescriptor<T7> symbol7,
            IValueDescriptor<T8> symbol8,
            IValueDescriptor<T9> symbol9)
        => command.Handler = new AnonymousCommandHandler (
            context =>
            {
                var value1 = GetValueForHandlerParameter (symbol1, context);
                var value2 = GetValueForHandlerParameter (symbol2, context);
                var value3 = GetValueForHandlerParameter (symbol3, context);
                var value4 = GetValueForHandlerParameter (symbol4, context);
                var value5 = GetValueForHandlerParameter (symbol5, context);
                var value6 = GetValueForHandlerParameter (symbol6, context);
                var value7 = GetValueForHandlerParameter (symbol7, context);
                var value8 = GetValueForHandlerParameter (symbol8, context);
                var value9 = GetValueForHandlerParameter (symbol9, context);

                return handle (value1!, value2!, value3!, value4!, value5!, value6!, value7!, value8!, value9!);
            });

    //
    // Based on Handler.GetValueForHandlerParameter() as it's internal
    //
    static T? GetValueForHandlerParameter<T> (IValueDescriptor<T> symbol, InvocationContext context)
        => symbol is IValueSource valueSource &&
                valueSource.TryGetValue (symbol, context.BindingContext, out var boundValue) &&
                boundValue is T value
            ? value
            : context.ParseResult.GetValueFor (symbol);

    //
    // Based on ParseResult.GetValueFor() as it's internal
    //
    internal static T? GetValueFor<T> (this ParseResult parseResult, IValueDescriptor<T> symbol)
        => symbol switch
        {
            Argument<T> argument => parseResult.GetValueForArgument(argument),
            Option<T> option => parseResult.GetValueForOption(option),
            _ => throw new ArgumentOutOfRangeException (nameof (symbol))
        };
}
