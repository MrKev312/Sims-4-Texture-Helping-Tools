using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCnEncoder.Decoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared.ImageFiles;

namespace Sims_4_Texture_Helping_Tools.Converters;
public static class ImageConverters
{
	public static Image ConvertDDSToPNG(DdsFile file)
	{
		BcDecoder decoder = new();
		return decoder.DecodeToImageRgba32(file);
	}

	// A function that takes in a filepath pointing to a dds and converts this to a png file with a given output filepath
	public static Image ConvertDDSToPNG(DdsFile dds1, DdsFile dds2)
	{
		BcDecoder decoder = new();
		using Image<Rgba32> image1 = decoder.DecodeToImageRgba32(dds1);
		using Image<Rgba32> image2 = decoder.DecodeToImageRgba32(dds2);

		image2.Mutate(x =>
		{
			x.Resize(image1.Width, image1.Height);
		});

		Image<L8>[] channels = new Image<L8>[4];

		for (int i = 0; i < channels.Length; i++)
		{
			channels[i] = new(image1.Width, image1.Height);
		}

		for (int i = 0; i < image1.Width; i++)
		{
			for (int j = 0; j < image1.Height; j++)
			{
				channels[0][i, j] = new(image1[i, j].R); // Alpha
				channels[1][i, j] = new(image1[i, j].G); // Y
				channels[2][i, j] = new(image2[i, j].R); // Co
				channels[3][i, j] = new(image2[i, j].G); // Cg
			}
		}

		return ColorConverters.ConvertYCoCgToRGBA(channels[1], channels[2], channels[3], channels[0]);
	}
}
