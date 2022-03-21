using System;

public class NetworkSerializables
{

}
[Serializable]

public class Player
{
    public string username;
    public string id;
    // public Position position;
    public string text;
    public PlayerColor color;
}

[Serializable]
public class Positions
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Details
{
    public Positions pos;
    public Positions rot;
}

[Serializable]
public class PlayerColor
{
    public float r;
    public float g;
    public float b;
    public float a;
}

[Serializable]
public class JoinGame
{
    public string name;
}

[Serializable]
public class Get_Id
{
    public string c_scene;
    public string g_id;
}