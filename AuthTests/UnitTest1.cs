namespace AuthTests;

public class UnitTest1
{
    [Fact]
    public void TestAdd()
    {
        // Arrange
        int a = 3;
        int b = 5;
        int expected = 8;

        // Act
        int result = MyMath.Add(a, b);

        // Assert
        Assert.Equal(expected, result);
    }
}

// Class containing the function to be tested
public static class MyMath
{
    // Function to be tested
    public static int Add(int a, int b)
    {
        return a + b;
    }
}