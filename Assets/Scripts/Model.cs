using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public static Model Instance;
    public GameObject Enemy;
    public GameObject Friends;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum CharacterType
{
    Enemy,
    Friend,
}
public class CharacterObject
{
    public CharacterObject(string name, CharacterType type)
    {
        var obj = new GameObject();
        obj.transform.parent = type == CharacterType.Friend
            ? Model.Instance.Friends.transform
            : Model.Instance.Enemy.transform;
        obj.name = name;
    }
}
