using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public void CreateGridButton()
    {
        if(inputField != null)
        {
            int integerValue;
            if(int.TryParse(inputField.text,out integerValue))
            {
                EventManager.Broadcast(GameEvent.OnClearCellList);
                EventManager.Broadcast(GameEvent.OnCreateGrid, integerValue);
            }
            else
            {
                Debug.Log("integer only");
            }
        }
    }
}
