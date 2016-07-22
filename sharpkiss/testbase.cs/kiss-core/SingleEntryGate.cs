using System.Threading;

namespace Kiss
{
    /// <summary>
    /// used to synchronize access to a single-use consumable resource
    /// </summary>
    public sealed class SingleEntryGate
    {
        private const int NOT_ENTERED = 0;
        private const int ENTERED = 1;

        private int _status;

        /// <summary>
        /// returns true if this is the first call to TryEnter(), false otherwise
        /// </summary>
        /// <returns></returns>
        public bool TryEnter()
        {
            int oldStatus = Interlocked.Exchange(ref _status, ENTERED);
            return (oldStatus == NOT_ENTERED);
        }
    }
}
