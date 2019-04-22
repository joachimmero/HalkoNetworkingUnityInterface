using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    //Private properties:
    [SerializeField] Button setNameBtn;
    [SerializeField] InputField nameField;
    [SerializeField] GameObject[] menus;

    // Start is called before the first frame update
    void Start()
    {
        setNameBtn.onClick.AddListener(() => OpenMenu(1));
    }
    
    //Private properties:
    private void OpenMenu(int index)
    {
        if(index == 1 && nameField.text == "")
        {
            return;
        }
        else
        {
            FindObjectOfType<HalkoNetworking.HalkoNetwork>().clientName = nameField.text;
        }

        for (int i = 0; i < menus.Length; i++)
        {
            if(index != i)
            {
                menus[i].SetActive(false);
            }
            else
            {
                menus[i].SetActive(true);
            }
        }
    }
}
