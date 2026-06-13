namespace SWeetJelly.Debug.Contract
{
    public interface IDebugger
    {
        void Publish(DebugEntry entry, string formattedMessage);
    }
}
