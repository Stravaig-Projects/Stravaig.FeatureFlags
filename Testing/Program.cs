using Example.Testing;
using Stravaig.FeatureFlags;

namespace Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IAlphaFeatureFlag ff = FakeAlphaFeatureFlag.Enabled;
            
            Console.WriteLine($"Is Enabled = {ff.IsEnabled()}");
        }
    }
}

