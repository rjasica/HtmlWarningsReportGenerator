namespace HWRG.Processor
{
    public interface IProcessorFactory
    {
        IProcessor Create(string name);
    }
}