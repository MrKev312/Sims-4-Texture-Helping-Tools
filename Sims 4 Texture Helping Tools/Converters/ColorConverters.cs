namespace Sims_4_Texture_Helping_Tools.Converters;

public static class ColorConverters
{
    public static (Image<Rgba32>, Image<Rgba32>) ConvertRGBAToYCoCgA(Image<Rgba32> input)
    {
        // Create a new image for the output
        Image<Rgba32> output1 = new(input.Width, input.Height);
        Image<Rgba32> output2 = new(input.Width, input.Height);

        // Loop through the pixels of the images
        for (int i = 0; i < input.Width; i++)
        {
            for (int j = 0; j < input.Height; j++)
            {
                (L8 A, L8 y, L8 co, L8 cg) = ConvertRGBAToYCoCgA(input[i, j]);

                // Set the pixel to the output image
                output1[i, j] = new Rgba32(A.PackedValue, y.PackedValue, 0);
                output2[i, j] = new Rgba32(co.PackedValue, cg.PackedValue, 0);
            }
        }

        return (output1, output2);
    }

    public static Image<Rgba32> ConvertYCoCgToRGBA(Image<L8> y, Image<L8> co, Image<L8> cg, Image<L8> alpha)
    {
        // Check that the images have the same dimensions
        if (alpha.Width != y.Width || alpha.Height != y.Height ||
            alpha.Width != co.Width || alpha.Height != co.Height ||
            alpha.Width != cg.Width || alpha.Height != cg.Height)
        {
            throw new ArgumentException("The images must have the same dimensions.");
        }

        // Create a new image for the output
        Image<Rgba32> output = new(alpha.Width, alpha.Height);

        // Loop through the pixels of the images
        for (int i = 0; i < alpha.Width; i++)
        {
            for (int j = 0; j < alpha.Height; j++)
            {
                (int R, int G, int B, int A) = ConvertYCoCgAToRGBA(y[i, j], co[i, j], cg[i, j], alpha[i, j]);

                // Set the pixel to the output image
                output[i, j] = new Rgba32((byte)R, (byte)G, (byte)B, (byte)A);
            }
        }

        // Return the output image
        return output;
    }

    public static (L8 y, L8 co, L8 cg, L8 A) ConvertRGBAToYCoCgA(Rgba32 input)
    {
        // Get the values of the alpha, y, co and cg channels
        int R = input.R;
        int G = input.G;
        int B = input.B;
        int A = input.A;

        int y = (((R + B) >> 1) + G + 1) >> 1;
        int co = (R + (B ^ 0xff) + 1) >> 1;
        int cg = ((((R + B + 1) >> 1) ^ 0xff) + G + 1) >> 1;

        y = Math.Clamp(y, 0, 255);
        co = Math.Clamp(co, 0, 255);
        cg = Math.Clamp(cg, 0, 255);

        return (new((byte)y), new((byte)co), new((byte)cg), new((byte)A));
    }

    public static (int R, int G, int B, int A) ConvertYCoCgAToRGBA(L8 y, L8 co, L8 cg, L8 alpha)
    {
        // Get the values of the alpha, y, co and cg channels
        int Y = y.PackedValue;
        int Co = co.PackedValue;
        int Cg = cg.PackedValue;
        int a = alpha.PackedValue;

        int r = Y + Co - Cg;
        int g = Y + Cg - 128;
        int b = Y - Co - Cg + 255;

        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);

        return (r, g, b, a);
    }
}