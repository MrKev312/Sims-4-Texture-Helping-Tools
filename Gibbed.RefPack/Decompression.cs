/* Copyright (c) 2013 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.IO;

namespace Gibbed.RefPack;

public static class Decompression
{
    public static byte[] Decompress(byte[] input)
    {
        using MemoryStream data = new(input, false);
        return Decompress(data);
    }

    public static byte[] Decompress(Stream input)
    {
        if (input is null)
            throw new ArgumentNullException(nameof(input));

        byte[] dummy = new byte[4];
        if (input.Read(dummy, 0, 2) != 2)
        {
            throw new EndOfStreamException("could not read header");
        }

        int header = (dummy[0] << 8) | dummy[1];
        if ((header & 0x1FFF) != 0x10FB)
        {
            throw new InvalidOperationException("input is not compressed");
        }

        bool isLong = (header & 0x8000) != 0;
        bool isDoubled = (header & 0x0100) != 0;

        if (isDoubled == true)
        {
            throw new InvalidOperationException("this should never happen");
        }

        uint uncompressedSize;
        if (isLong == true)
        {
            if (input.Read(dummy, 0, 4) != 4)
            {
                throw new EndOfStreamException("could not read uncompressed size");
            }

            uncompressedSize = (uint)(dummy[0] << 24) |
                               (uint)(dummy[1] << 16) |
                               (uint)(dummy[2] << 8) |
                               (uint)(dummy[3] << 0);
        }
        else
        {
            if (input.Read(dummy, 0, 3) != 3)
            {
                throw new EndOfStreamException("could not read uncompressed size");
            }

            uncompressedSize = (uint)(dummy[0] << 16) |
                               (uint)(dummy[1] << 8) |
                               (uint)(dummy[2] << 0);
        }

        byte[] data = new byte[uncompressedSize];
        uint offset = 0;
        while (true)
        {
            bool stop = false;
            uint plainSize;
            uint copySize = 0u;
            uint copyOffset = 0u;

            if (input.Read(dummy, 0, 1) != 1)
            {
                throw new EndOfStreamException("could not read prefix");
            }

            byte prefix = dummy[0];

            if (prefix < 0x80)
            {
                if (input.Read(dummy, 0, 1) != 1)
                {
                    throw new EndOfStreamException("could not read extra");
                }

                plainSize = (uint)(prefix & 0x03);
                copySize = (uint)(((prefix & 0x1C) >> 2) + 3);
                copyOffset = (uint)((((prefix & 0x60) << 3) | dummy[0]) + 1);
            }
            else if (prefix < 0xC0)
            {
                if (input.Read(dummy, 0, 2) != 2)
                {
                    throw new EndOfStreamException("could not read extra");
                }

                plainSize = (uint)(dummy[0] >> 6);
                copySize = (uint)((prefix & 0x3F) + 4);
                copyOffset = (uint)((((dummy[0] & 0x3F) << 8) | dummy[1]) + 1);
            }
            else if (prefix < 0xE0)
            {
                if (input.Read(dummy, 0, 3) != 3)
                {
                    throw new EndOfStreamException("could not read extra");
                }

                plainSize = (uint)(prefix & 3);
                copySize = (uint)((((prefix & 0x0C) << 6) | dummy[2]) + 5);
                copyOffset = (uint)((((((prefix & 0x10) << 4) | dummy[0]) << 8) | dummy[1]) + 1);
            }
            else if (prefix < 0xFC)
            {
                plainSize = (uint)(((prefix & 0x1F) + 1) * 4);
            }
            else
            {
                plainSize = (uint)(prefix & 3);
                stop = true;
            }

            if (plainSize > 0)
            {
                if (input.Read(data, (int)offset, (int)plainSize) != (int)plainSize)
                {
                    throw new EndOfStreamException("could not read plain");
                }

                offset += plainSize;
            }

            if (copySize > 0)
            {
                for (uint i = 0; i < copySize; i++)
                {
                    data[offset + i] = data[offset - copyOffset + i];
                }

                offset += copySize;
            }

            if (stop)
            {
                break;
            }
        }

        return data;
    }
}
