namespace Sims_4_Texture_Helping_Tools.Data.IO.Serialization;

public interface ISerializable
{
	void Read(Stream stream);

	void Write(Stream stream);
}
