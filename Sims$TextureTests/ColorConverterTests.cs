using Sims_4_Texture_Helping_Tools.Converters;

namespace Sims_TextureTests;

[TestClass]
public class ColorConverterTests
{
	// This is a test method that checks if the ConvertARGBToYCoCgA method returns the correct output values
	[TestMethod]
	public void TestConvertRGBAToYCoCgA()
	{
		// Arrange
		// Create a mock input pixel
		Rgba32 input = new(180, 121, 5, 70); // Red

		// Create the expected output values
		L8 expectedY = new(107);
		L8 expectedCo = new(215);
		L8 expectedCg = new(142);
		L8 expectedA = new(70);

		// Act
		// Call the method under test and get the actual output values
		(L8 actualY, L8 actualCo, L8 actualCg, L8 actualA) = ColorConverters.ConvertRGBAToYCoCgA(input);

		// Assert
		// Compare the actual output values with the expected output values
		Assert.AreEqual(expectedY, actualY);
		Assert.AreEqual(expectedCo, actualCo);
		Assert.AreEqual(expectedCg, actualCg);
		Assert.AreEqual(expectedA, actualA);
	}

	// This is a test method that checks if the ConvertYCoCgAToRGBA method returns the correct output values
	[TestMethod]
	public void TestConvertYCoCgAToRGBA()
	{
		// Arrange
		// Create mock input values
		L8 y = new(107); // Y channel
		L8 co = new(215); // Co channel
		L8 cg = new(142); // Cg channel
		L8 alpha = new(70); // Alpha channel

		// Create the expected output values
		int expectedR = 180;
		int expectedG = 121;
		int expectedB = 5;
		int expectedA = 70;

		// Act
		// Call the method under test and get the actual output values
		(int actualR, int actualG, int actualB, int actualA) = ColorConverters.ConvertYCoCgAToRGBA(y, co, cg, alpha);

		// Assert
		// Compare the actual output values with the expected output values
		Assert.AreEqual(expectedR, actualR);
		Assert.AreEqual(expectedG, actualG);
		Assert.AreEqual(expectedB, actualB);
		Assert.AreEqual(expectedA, actualA);
	}
}