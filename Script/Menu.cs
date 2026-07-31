using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        if (shopBGR != null)
        {
            shopBGR.SetActive(false);
        }

        if (bagBGR != null)
        {
            bagBGR.SetActive(false);
        }

        if (settingBGR != null)
        {
            settingBGR.SetActive(false);
        }
    }

    public void MenuLevels()
    {
        SceneManager.LoadScene("Levels_ThienDuong");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    [SerializeField] private GameObject shopBGR;
    public void OutShop()
    {
        if (shopBGR != null)
        {
            shopBGR.SetActive(false);
        }
    }

    public void OpenShop()
    {
        shopBGR.SetActive(true);
    }

    [SerializeField] private GameObject bagBGR;
    public void OpenBag()
    {
        bagBGR.SetActive(true);
    }
    
    public void CloseBag()
    {
        bagBGR.SetActive(false);
    }

    [SerializeField] private GameObject settingBGR;
    public void OpenSetting()
    {
        settingBGR.SetActive(true);
    }
    public void OutSetting()
    {
        settingBGR.SetActive(false);
    }
}

