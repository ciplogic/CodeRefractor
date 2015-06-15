namespace CodeRefractor.FrontEnd.SimpleOperations.Identifiers
{
    /// <summary>
    ///     For a speciffic variable it is said which is the way is states on stack
    /// </summary>
    public enum EscapingMode
    {
        /// <summary>
        ///     The reference needs to be reference counted
        /// </summary>
        Smart,

        /// <summary>
        ///     The reference doesn't need to be reference counted, all usages do increase and
        ///     decrease the refcount by the same amount
        /// </summary>
        Pointer,

        /// <summary>
        ///     The reference should not be refcounted. Is either a value type (like "int")
        ///     or it can be declared just in function and it doesn't escape
        /// </summary>
        Stack,

        /// <summary>
        ///     This variable is not used at all or it cannot be reached for the final output
        /// </summary>
        Unused
    }
}