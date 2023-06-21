using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;

namespace Sims_4_Texture_Helping_Tools;

internal static class CompressionFormatHelper
{
    public static CompressionFormat GetCompressionFormat(DdsFile file)
    {
        DxgiFormat format = file.header.ddsPixelFormat.IsDxt10Format ?
            file.dx10Header.dxgiFormat :
            file.header.ddsPixelFormat.DxgiFormat;

        switch (format)
        {
            case DxgiFormat.DxgiFormatR8Unorm:
                return CompressionFormat.R;

            case DxgiFormat.DxgiFormatR8G8Unorm:
                return CompressionFormat.Rg;

            // HINT: R8G8B8 has no DxgiFormat to convert from

            case DxgiFormat.DxgiFormatR8G8B8A8Unorm:
                return CompressionFormat.Rgba;

            case DxgiFormat.DxgiFormatB8G8R8A8Unorm:
                return CompressionFormat.Bgra;

            case DxgiFormat.DxgiFormatBc1Unorm:
            case DxgiFormat.DxgiFormatBc1UnormSrgb:
            case DxgiFormat.DxgiFormatBc1Typeless:
                if (file.header.ddsPixelFormat.dwFlags.HasFlag(PixelFormatFlags.DdpfAlphaPixels))
                    return CompressionFormat.Bc1WithAlpha;

                //if (InputOptions.DdsBc1ExpectAlpha)
                //    return CompressionFormat.Bc1WithAlpha;

                return CompressionFormat.Bc1;

            case DxgiFormat.DxgiFormatBc2Unorm:
            case DxgiFormat.DxgiFormatBc2UnormSrgb:
            case DxgiFormat.DxgiFormatBc2Typeless:
                return CompressionFormat.Bc2;

            case DxgiFormat.DxgiFormatBc3Unorm:
            case DxgiFormat.DxgiFormatBc3UnormSrgb:
            case DxgiFormat.DxgiFormatBc3Typeless:
                return CompressionFormat.Bc3;

            case DxgiFormat.DxgiFormatBc4Unorm:
            case DxgiFormat.DxgiFormatBc4Snorm:
            case DxgiFormat.DxgiFormatBc4Typeless:
                return CompressionFormat.Bc4;

            case DxgiFormat.DxgiFormatBc5Unorm:
            case DxgiFormat.DxgiFormatBc5Snorm:
            case DxgiFormat.DxgiFormatBc5Typeless:
                return CompressionFormat.Bc5;

            case DxgiFormat.DxgiFormatBc6HTypeless:
            case DxgiFormat.DxgiFormatBc6HUf16:
                return CompressionFormat.Bc6U;

            case DxgiFormat.DxgiFormatBc6HSf16:
                return CompressionFormat.Bc6S;

            case DxgiFormat.DxgiFormatBc7Unorm:
            case DxgiFormat.DxgiFormatBc7UnormSrgb:
            case DxgiFormat.DxgiFormatBc7Typeless:
                return CompressionFormat.Bc7;

            case DxgiFormat.DxgiFormatAtcExt:
                return CompressionFormat.Atc;

            case DxgiFormat.DxgiFormatAtcExplicitAlphaExt:
                return CompressionFormat.AtcExplicitAlpha;

            case DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt:
                return CompressionFormat.AtcInterpolatedAlpha;

            default:
                return CompressionFormat.Unknown;
        }
    }
}