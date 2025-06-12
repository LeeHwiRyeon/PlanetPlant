using UnityEngine;

public class UnityCannonInput : ICannonInput
{
    public bool FirePressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
}
