using MessagePack;

namespace powdered_networking.messages;

[MessagePackObject]
public class NetworkVector2
{
    [Key(0)]
    public float xPos {get; set;}
    [Key(1)]
    public float yPos {get; set;}

    public NetworkVector2(float xPos = 0f, float yPos = 0f)
    {
        this.xPos = xPos;
        this.yPos = yPos;
    }
}