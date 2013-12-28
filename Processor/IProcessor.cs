using HWRG.Input;

namespace HWRG.Processor
{
    public interface IProcessor
    {
        void Write(string outputDir, Report report);
    }
}
