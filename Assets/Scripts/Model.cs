using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Model : MonoBehaviour
{
    public static Model Instance;
    public GameObject Enemy;
    public GameObject Friends;
    public TextAsset characterText;
    public TextAsset moveText;
    [HideInInspector]
    public List<CharacterObject> EnemyList = new List<CharacterObject>();
    [HideInInspector]
    public List<CharacterObject> FriendList = new List<CharacterObject>();
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        _InitCharacter(characterText);
        foreach (var enemy in EnemyList)
            enemy.Init();
        foreach (var enemy in FriendList)
            enemy.Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void _InitCharacter(TextAsset toParse)
    {
        var fileText = toParse.text;
        var lines = fileText.Split("\n"[0]).ToList();
        lines.Remove(lines[0]);
        foreach (var line in lines)
        {
            var data = line.Trim().Split(","[0]).ToList();
            
            var character = new CharacterObject();
            character.type = (CharacterType) Enum.Parse(typeof(CharacterType), data[0]);
            character.name = data[1];
            character.life = Convert.ToInt32(data[2].Trim());
            if(character.type == CharacterType.Enemy) EnemyList.Add(character);
            else FriendList.Add(character);
        }
    }

    private void _CreateSelection<T>(List<T> selectionList)
    {
        
    }

    public void ChooseIndex()
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
    public CharacterType type;
    public string name;
    public int life;
    public GameObject characterObj;
    public TextMeshPro nameText
    {
        get
        {
            var list = characterObj.GetComponentsInChildren<TextMeshPro>().ToList();
            foreach (var item in list)
                if (item.gameObject.name == "name") return item;
            return null;
        }
    }
    public TextMeshPro lifeText
    {
        get
        {
            var list = characterObj.GetComponentsInChildren<TextMeshPro>().ToList();
            foreach (var item in list)
                if (item.gameObject.name == "life") return item;
            return null;
        }
    }

    public void Init()
    {
        characterObj = GameObject.Instantiate(Resources.Load("Character"),
            type == CharacterType.Enemy ? Model.Instance.Enemy.transform : Model.Instance.Friends.transform) as GameObject;
        var num = type == CharacterType.Enemy? Model.Instance.EnemyList.Count : Model.Instance.FriendList.Count;
        characterObj.transform.position = new Vector3(characterObj.transform.position.x,characterObj.transform.position.y - 2.5f * num,characterObj.transform.position.z);
        nameText.text = name;
        lifeText.text = life.ToString();
    }

    public void Update()
    {
        lifeText.text = life.ToString();
        if(life<=0) GameObject.Destroy(characterObj);
    }
}


