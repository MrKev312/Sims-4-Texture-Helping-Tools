namespace Sims_4_Texture_Helping_Tools.Data.DBPF;
public class ResourceKey
{
	public uint Type { get; set; }
	public uint Group { get; set; }
	public ulong InstanceID { get; set; }

	public override bool Equals(object? obj)
	{
		return obj is ResourceKey key &&
			   Type == key.Type &&
			   Group == key.Group &&
			   InstanceID == key.InstanceID;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Type, Group, InstanceID);
	}
}
