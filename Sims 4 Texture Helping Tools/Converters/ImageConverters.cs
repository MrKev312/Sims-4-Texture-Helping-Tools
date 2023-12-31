﻿using BCnEncoder.Decoder;
using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;

namespace Sims_4_Texture_Helping_Tools.Converters;
public static class ImageConverters
{
	public static Image ConvertDDSToPNG(DdsFile file)
	{
		BcDecoder decoder = new();
		return decoder.DecodeToImageRgba32(file);
	}

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

	public static DdsFile ConvertPNGToDDS(Image image, CompressionFormat compressionFormat = CompressionFormat.Rgba, bool generateMipMaps = false)
	{
		if (image is null)
			throw new ArgumentNullException(nameof(image));

		BcEncoder encoder = new();
		encoder.OutputOptions.GenerateMipMaps = generateMipMaps;
		encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
		encoder.OutputOptions.Format = compressionFormat;
		encoder.OutputOptions.FileFormat = OutputFileFormat.Dds;

		using Image<Rgba32> imageRGBA = image.CloneAs<Rgba32>();

		return encoder.EncodeToDds(imageRGBA);
	}
}
