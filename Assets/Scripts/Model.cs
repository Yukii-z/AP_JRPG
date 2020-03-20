using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = System.Random;

public class Model : MonoBehaviour
{
    public static Model Instance;
    public GameObject Enemy;
    public GameObject Friends;
    public GameObject buttons;
    public TextAsset characterText;
    public TextAsset skillText;
    [HideInInspector] public Dictionary<string, CharacterObject> enemyList = new Dictionary<string, CharacterObject>();
    [HideInInspector]
    public Dictionary<string, CharacterObject> friendList = new Dictionary<string, CharacterObject>();
    [HideInInspector]
    public List<KeyValuePair<CharacterObject,Skill>> moveList = new List<KeyValuePair<CharacterObject, Skill>>();
    public Dictionary<string, Skill> skillInfo = new Dictionary<string, Skill>();
    
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        _InitCharacter(characterText);
        foreach (var enemy in enemyList.Values)
            enemy.Init();
        foreach (var enemy in friendList.Values)
            enemy.Init();
        _InitSkill(skillText);
        _CreateSkillSelectButton(skillInfo.Values.ToList());
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
            character.maxLife = character.life = Convert.ToInt32(data[2].Trim());
            var list = new Dictionary<string,CharacterObject>();
            if (character.type == CharacterType.Enemy)
                list = enemyList;
            else
                list = friendList;
            var potentialName = character.name + "0";
            var i = 0;
            while (list.ContainsKey(potentialName))
            {
                i++;
                potentialName = potentialName.Substring(0, potentialName.Length - 1);
                potentialName = potentialName + i.ToString();
            }
            list.Add(potentialName,character);
        }
    }
    private void _InitSkill(TextAsset toParse)
    {
        var fileText = toParse.text;
        var lines = fileText.Split("\n"[0]).ToList();
        lines.Remove(lines[0]);
        foreach (var line in lines)
        {
            var data = line.Trim().Split(","[0]).ToList();
            
            var skill = new Skill();
            skill.name = data[0]; 
            skill.targetType = (CharacterType) Enum.Parse(typeof(CharacterType), data[1]);
            skill.range = Convert.ToInt32(data[2]);
            skill.effect = Convert.ToInt32(data[3]);
            skillInfo.Add(skill.name,skill);
        }
    }
    private void _CreateSkillSelectButton(List<Skill> selectionList)
    {
        
        foreach (var selection in selectionList)
        {
            var obj = GameObject.Instantiate(Resources.Load("Button"), buttons.transform) as GameObject;
            obj.GetComponentInChildren<Text>().text = selection.name;
            obj.GetComponent<ChoiceName>().name = selection.name;
            obj.GetComponent<ChoiceName>().isSkill = true;
        }
    }
    private void _CreateTargetSelectButton(Dictionary<string,CharacterObject> selectionDic)
    {

        foreach (var selection in selectionDic)
        {
            var obj = GameObject.Instantiate(Resources.Load("Button"), buttons.transform) as GameObject;
            obj.GetComponent<ChoiceName>().name = selection.Key;
            obj.GetComponentInChildren<Text>().text = selection.Value.name;
        }
    }
    
    private KeyValuePair<CharacterObject, Skill> tempMove = new KeyValuePair<CharacterObject, Skill>();
    public void ChooseSkill(string name)
    {
        var skill = skillInfo[name];
        tempMove = new KeyValuePair<CharacterObject, Skill>(tempMove.Key,skill);
        if(skill.targetType == CharacterType.Enemy) _CreateTargetSelectButton(enemyList);
        else _CreateTargetSelectButton(friendList);
    }

    public void ChooseTarget(string name)
    {
        var skillTargetType = tempMove.Value.targetType;
        if(skillTargetType == CharacterType.Enemy) tempMove = new KeyValuePair<CharacterObject, Skill>(enemyList[name],tempMove.Value);
        else tempMove = new KeyValuePair<CharacterObject, Skill>(friendList[name],tempMove.Value);
        moveList.Add(tempMove);
        _ComputerChoice();
    }

    private void _ComputerChoice()
    {
        var randomInt = UnityEngine.Random.Range(0, skillInfo.Count);
        var skill = skillInfo.Values.ToList()[randomInt];
        var target = new CharacterObject();
        if (skill.targetType == CharacterType.Enemy)
        {
            randomInt = UnityEngine.Random.Range(0, friendList.Count);
            target = friendList.Values.ToList()[randomInt];
        }
        else
        {
            randomInt = UnityEngine.Random.Range(0, enemyList.Count);
            target = enemyList.Values.ToList()[randomInt];
        }
        moveList.Add( new KeyValuePair<CharacterObject, Skill>(target,skill));

        StartCoroutine(_RunMove());
    }

    private IEnumerator _RunMove()
    {
        foreach (var move in moveList)
        {
            var targetList = new List<CharacterObject>();
            if (move.Key.type == CharacterType.Friend) targetList = friendList.Values.ToList();
            else targetList = enemyList.Values.ToList();
            var targetIndex = targetList.IndexOf(move.Key);
            targetList = targetList.GetRange(Mathf.Max(targetIndex - move.Value.range + 1, 0),
                Mathf.Min((2 * move.Value.range - 1),
                    targetList.Count - Mathf.Max(targetIndex - move.Value.range + 1, 0)));
            foreach (var target in targetList)
            {
                if (target.characterObj.activeSelf)
                {
                    target.life += move.Value.effect;
                    target.Update();
                }
            }
            yield return new WaitForSeconds(2f);
        }
        moveList.Clear();
        _CreateSkillSelectButton(skillInfo.Values.ToList());
    }
}

public struct Skill
{
    public string name;
    public CharacterType targetType;
    public int range;
    public int effect;
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
    public int maxLife;
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
        var num = type == CharacterType.Enemy? Model.Instance.enemyList.Values.ToList().IndexOf(this) 
            : Model.Instance.friendList.Values.ToList().IndexOf(this);
        characterObj.transform.position = new Vector3(characterObj.transform.position.x,characterObj.transform.position.y - 2.5f * num,characterObj.transform.position.z);
        nameText.text = name;
        lifeText.text = life.ToString();
    }

    public void Update()
    {
        lifeText.text = life.ToString();
        if (life <= 0)
            characterObj.SetActive(false);
    }
}


