using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceName : MonoBehaviour
{
   [HideInInspector] public string name;
   [HideInInspector] public bool isSkill = false;
   private Button _button
   {
      get { return gameObject.GetComponent<Button>(); }
   }
   private void Start()
   {
      _button.onClick.AddListener(SendChoice);
   }
   
   void SendChoice()
   {   
      var children = Model.Instance.buttons.GetComponentsInChildren<Transform>().ToList();
      children.Remove(children[0]);
      foreach (var kid in children)
         Destroy(kid.gameObject);
      
      if(isSkill) Model.Instance.ChooseSkill(name);
      else Model.Instance.ChooseTarget(name);
      _button.onClick.RemoveListener(SendChoice);
   }
}
