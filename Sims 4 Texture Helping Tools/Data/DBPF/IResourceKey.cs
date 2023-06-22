namespace Sims_4_Texture_Helping_Tools.Data.DBPF;
public interface IResourceKey
{
	public uint Type { get; set; }
	public uint Group { get; set; }
	public ulong InstanceID { get; set; }
}
