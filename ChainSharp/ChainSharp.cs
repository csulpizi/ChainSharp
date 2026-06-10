namespace ChainSharp;

/// <summary>
/// Provides a framework to chain together functions, similar to
///  `piping` in heavily functional programming languages like LISP
/// Supports both synchronous operations via `.Then()`
///  and asynchronous operations via `.WaitThen()`
/// Supports try/catch functionality in the middle of chains
///  by using `.Catch()`
/// The result type of all operations is a func of type Func<T0, T1>,
///  where T0 is the input of the first function in the chain
///  and T1 is the output of the final function in the chain, or
///  Task<T1> if a `.WaitThen()` operation was added in the chain
/// Provides out of the box support for Nullable<T> monadic patterns,
///  and can be extended to provide support for other monads
/// </summary>
public static class Chain
{
    /// <summary>
    /// Creates the starting point of a chain.
    /// Can alteÍrnatively use a lambda e.g. `(T x) => x;`
    ///  or any func F<T, T2>
    /// </summary>
    /// <typeparam name="T">The input type for the chain</typeparam>
    public static Func<T, T> Init<T>() => x => x;

    /// <summary>
    /// Adds the func `f` to the chain
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, TOut> Then<TIn, TMiddle, TOut>(
        this Func<TIn, TMiddle> @this,
        Func<TMiddle, TOut> f
    ) => x => f(@this(x));

    /// <summary>
    /// Adds the func `f` to the asynchronous chain, awaiting the
    ///  return of `@this` before invoking `f`
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, Task<TOut>> WaitThen<TIn, TMiddle, TOut>(
        this Func<TIn, Task<TMiddle>> @this,
        Func<TMiddle, TOut> f
    )
    {
        return async a =>
        {
            return f(await @this(a));
        };
    }

    /// <summary>
    /// '.Then()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, TOut?> ThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, TMiddle?> @this,
        Func<TMiddle, TOut> f
    )
        where TMiddle : struct
        where TOut : struct => @this.Then<TIn, TMiddle?, TOut?>(x => x is null ? null : f(x.Value));

    /// <summary>
    /// '.Then()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, TOut?> ThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, TMiddle?> @this,
        Func<TMiddle, TOut> f
    )
        where TMiddle : class
        where TOut : struct => @this.Then<TIn, TMiddle?, TOut?>(x => x is null ? null : f(x));

    /// <summary>
    /// '.Then()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, TOut?> ThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, TMiddle?> @this,
        Func<TMiddle, TOut?> f
    )
        where TMiddle : struct
        where TOut : struct => @this.Then(x => x is null ? null : f(x.Value));

    /// <summary>
    /// '.Then()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, TOut?> ThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, TMiddle?> @this,
        Func<TMiddle, TOut?> f
    )
        where TMiddle : class
        where TOut : struct => @this.Then(x => x is null ? null : f(x));

    /// <summary>
    /// '.WaitThen()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, Task<TOut?>> WaitThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, Task<TMiddle?>> @this,
        Func<TMiddle, TOut> f
    )
        where TMiddle : struct
        where TOut : struct =>
        @this.WaitThen<TIn, TMiddle?, TOut?>(x => x is null ? null : f(x.Value));

    /// <summary>
    /// '.WaitThen()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, Task<TOut?>> WaitThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, Task<TMiddle?>> @this,
        Func<TMiddle, TOut> f
    )
        where TMiddle : class
        where TOut : struct => @this.WaitThen<TIn, TMiddle?, TOut?>(x => x is null ? null : f(x));

    /// <summary>
    /// '.WaitThen()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, Task<TOut?>> WaitThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, Task<TMiddle?>> @this,
        Func<TMiddle, TOut?> f
    )
        where TMiddle : struct
        where TOut : struct => @this.WaitThen(x => x is null ? null : f(x.Value));

    /// <summary>
    /// '.WaitThen()' but gracefully handles null handling
    /// </summary>
    /// <param name="f">The function to chain</param>
    public static Func<TIn, Task<TOut?>> WaitThenOrNull<TIn, TMiddle, TOut>(
        this Func<TIn, Task<TMiddle?>> @this,
        Func<TMiddle, TOut?> f
    )
        where TMiddle : class
        where TOut : struct => @this.WaitThen(x => x is null ? null : f(x));

    /// <summary>
    /// Adds a catch around the chained function, catching an exception
    /// </summary>
    /// <param name="fCatch">
    /// The function to call if an exception is caught, where
    ///  the first parameter is the caught exception and the
    ///  second parameter is the input that caused the exception
    /// </param>
    public static Func<TIn, TOut> Catch<TIn, TOut>(
        this Func<TIn, TOut> @this,
        Func<Exception, TIn, TOut> fCatch
    ) => Catch<Exception, TIn, TOut>(@this, fCatch);

    /// <summary>
    /// Adds a catch around the chained function, catching an exception of type TException
    /// </summary>
    /// <param name="fCatch">
    /// The function to call if an exception is caught, where
    ///  the first parameter is the caught exception and the
    ///  second parameter is the input that caused the exception
    /// </param>
    public static Func<TIn, TOut> Catch<TException, TIn, TOut>(
        this Func<TIn, TOut> @this,
        Func<TException, TIn, TOut> fCatch
    )
        where TException : Exception
    {
        return x =>
        {
            try
            {
                return @this(x);
            }
            catch (TException e)
            {
                return fCatch(e, x);
            }
        };
    }

    /// <summary>
    /// Adds a catch around the asynchronously chained function,
    ///  catching an exception
    /// </summary>
    /// <param name="fCatch">
    /// The function to call if an exception is caught, where
    ///  the first parameter is the caught exception and the
    ///  second parameter is the input that caused the exception
    /// </param>
    public static Func<TIn, Task<TOut>> Catch<TIn, TOut>(
        this Func<TIn, Task<TOut>> @this,
        Func<Exception, TIn, TOut> fCatch
    ) => Catch<Exception, TIn, TOut>(@this, fCatch);

    /// <summary>
    /// Adds a catch around the asynchronously chained function,
    ///  catching an exception of type TException
    /// </summary>
    /// <param name="fCatch">
    /// The function to call if an exception is caught, where
    ///  the first parameter is the caught exception and the
    ///  second parameter is the input that caused the exception
    /// </param>
    public static Func<TIn, Task<TOut>> Catch<TException, TIn, TOut>(
        this Func<TIn, Task<TOut>> @this,
        Func<TException, TIn, TOut> fCatch
    )
        where TException : Exception
    {
        return async a =>
        {
            try
            {
                return await @this(a);
            }
            catch (TException e)
            {
                return fCatch(e, a);
            }
        };
    }
}
