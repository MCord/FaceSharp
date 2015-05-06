using System.Collections.Generic;
using System.Linq;

namespace Studio.Common
{
    public class ImageProcessorChain: List<IImageProcessor>, IImageProcessor
    {
        public ProcessedImage Process(ProcessedImage source)
        {
            return this.Aggregate(source, (current, p) => p.Process(current));
        }
    }
}