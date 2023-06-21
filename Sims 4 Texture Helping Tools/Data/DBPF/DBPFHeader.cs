using System.Text;
using ISerializable = Sims_4_Texture_Helping_Tools.Data.IO.Serialization.ISerializable;

namespace Sims_4_Texture_Helping_Tools.Data.DBPF;

[Serializable]
public class DBPFHeader : ISerializable
{
    public string Magic { get; } = "DBPF";
    public uint MajorVersion { get; private set; }
    public uint MinorVersion { get; private set; }
    public uint MajorUserVersion { get; private set; }
    public uint MinorUserVersion { get; private set; }
    public uint Flags { get; private set; }
    public uint CreatedDate { get; private set; }
    public uint UpdatedDate { get; private set; }
    public uint IndexMajorVersion { get; private set; }
    public uint IndexEntryCount { get; private set; }
    public uint IndexLocation { get; private set; }
    public uint IndexSize { get; private set; }
    public uint HoleIndexEntryCount { get; private set; }
    public uint HoleIndexLocation { get; private set; }
    public uint HoleIndexSize { get; private set; }
    public uint IndexMinorVersion { get; private set; }
    public uint IndexOffset { get; private set; }
    public uint[] Reserved { get; } = new uint[6];
    public uint HeaderSize { get; } = 96;

    public DBPFHeader()
    {
        MajorVersion = 2;
        MinorVersion = 1;

        IndexMinorVersion = 3;
    }

    public DBPFHeader(Stream stream)
    {
        Read(stream);
    }

    public void Read(Stream stream)
    {
        BinaryReader binaryReader = new(stream);

        // Check the magic value "DBPF"
        if (Encoding.ASCII.GetString(binaryReader.ReadBytes(4)) != Magic)
        {
            throw new Exception();
        }

        // Versioning
        MajorVersion = binaryReader.ReadUInt32();
        MinorVersion = binaryReader.ReadUInt32();
        MajorUserVersion = binaryReader.ReadUInt32();
        MinorUserVersion = binaryReader.ReadUInt32();

        // Flags
        Flags = binaryReader.ReadUInt32();

        // Dates
        CreatedDate = binaryReader.ReadUInt32();
        UpdatedDate = binaryReader.ReadUInt32();

        // Index
        IndexMajorVersion = binaryReader.ReadUInt32();
        IndexEntryCount = binaryReader.ReadUInt32();
        IndexLocation = binaryReader.ReadUInt32();
        IndexSize = binaryReader.ReadUInt32();

        // Hole
        HoleIndexEntryCount = binaryReader.ReadUInt32();
        HoleIndexLocation = binaryReader.ReadUInt32();
        HoleIndexSize = binaryReader.ReadUInt32();

        // Rest of index
        IndexMinorVersion = binaryReader.ReadUInt32();
        IndexOffset = binaryReader.ReadUInt32();

        // Reserved
        for (int j = 0; j < Reserved.Length; j++)
        {
            Reserved[j] = binaryReader.ReadUInt32();
        }
    }

    public void Write(Stream stream)
    {
        BinaryWriter binaryWriter = new(stream);

        // The magic "DBPF"
        binaryWriter.Write(Encoding.ASCII.GetBytes(Magic));

        // Verioning
        binaryWriter.Write(MajorVersion);
        binaryWriter.Write(MinorVersion);
        binaryWriter.Write(MajorUserVersion);
        binaryWriter.Write(MinorUserVersion);

        // Unused flags
        binaryWriter.Write(Flags);

        // Dates
        binaryWriter.Write(CreatedDate);
        binaryWriter.Write(UpdatedDate);

        // Index
        binaryWriter.Write(IndexMajorVersion);
        binaryWriter.Write(IndexEntryCount);
        binaryWriter.Write(IndexLocation);
        binaryWriter.Write(IndexSize);

        // Hole
        binaryWriter.Write(HoleIndexEntryCount);
        binaryWriter.Write(HoleIndexLocation);
        binaryWriter.Write(HoleIndexSize);

        // Rest of index
        binaryWriter.Write(IndexMinorVersion);
        binaryWriter.Write(IndexOffset);

        // Reserved
        foreach (uint reserved in Reserved)
        {
            binaryWriter.Write(reserved);
        }
    }
}
