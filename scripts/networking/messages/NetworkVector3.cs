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

    public NetworkVector3(float xPos = 0f, float yPos = 0f, float zPos = 0f)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.zPos = zPos;
    }
}



