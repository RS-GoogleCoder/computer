namespace Computer.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void FibonacciTest()
    {
        Computer.Program.Run(File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "../../../../Computer/programs/fibonacci.bin")));
        Assert.Pass();
    }
}