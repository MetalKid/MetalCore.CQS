namespace MetalCore.CQS.Validation
{
    /// <summary>
    /// This enum is used to determine what thread(s) the given rules will run under.
    /// </summary>
    public enum ThreadRuleRunTypeEnum
    {
        /// <summary>
        /// Invalid enum value.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The given rule(s) will run together on a separate thread.
        /// </summary>
        Single = 1,

        /// <summary>
        /// The given rule(s) will all run on separate threads.
        /// </summary>
        Multiple = 2,

        /// <summary>
        /// The given rule(s) will run on a single thread with all the other Shared rules.
        /// </summary>
        Shared = 3
    }
}