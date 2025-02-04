using MessagePack;

namespace powdered_networking.messages;

[MessagePackObject]
public class NetworkVector3
{
    [Key(0)]
    public float xPos {get; set;}
    [Key(1)]
    public float yPos {get; set;}
    [Key(2)]
    public float zPos {get; set;}

    public NetworkVector3(float xPos, float yPos, float zPos)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.zPos = zPos;
    }
}



