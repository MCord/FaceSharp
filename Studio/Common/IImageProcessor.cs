namespace Studio.Common
{
    public interface IImageProcessor
    {
        ProcessedImage Process(ProcessedImage source);
    }
}