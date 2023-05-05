namespace Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var ff = FakeAlphaFeatureFlag.Enable();
        }
    }
}

