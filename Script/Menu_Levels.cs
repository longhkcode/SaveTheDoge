using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Levels : MonoBehaviour
{
    private void LoadMapWithEnergy(string mapName)
    {
        if (EnergyBar.Instance != null)
        {
            if (EnergyBar.Instance.UseEnergy(20))
            {
                SceneManager.LoadScene(mapName);
            }
        }
        else
        {
            Debug.LogWarning("Chưa khởi tạo EnergyBar Instance!");
            SceneManager.LoadScene(mapName);
        }
    }

    public void Game_Map1() => LoadMapWithEnergy("Map1");
    public void Game_Map2() => LoadMapWithEnergy("Map2");
    public void Game_Map3() => LoadMapWithEnergy("Map3");
    public void Game_Map4() => LoadMapWithEnergy("Map4");
    public void Game_Map5() => LoadMapWithEnergy("Map5");
    public void Game_Map6() => LoadMapWithEnergy("Map6");
    public void Game_Map7() => LoadMapWithEnergy("Map7");
    public void Game_Map8() => LoadMapWithEnergy("Map8");
    public void Game_Map9() => LoadMapWithEnergy("Map9");
    
    

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadLevelThienDuong()
    {
        SceneManager.LoadScene("Levels_ThienDuong");
    }
    
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Levels");
    }
    
    public void LoadLevelDiaNguc()
    {
        SceneManager.LoadScene("Levels_DiaNguc");
    }
    
    public void TD0() => LoadMapWithEnergy("TD0");
    public void TD1() => LoadMapWithEnergy("TD1");
    public void TD2() => LoadMapWithEnergy("TD2");
    public void TD3() => LoadMapWithEnergy("TD3");
    public void TD4() => LoadMapWithEnergy("TD4");
    public void TD5() => LoadMapWithEnergy("TD5");
    public void TD6() => LoadMapWithEnergy("TD6");
    public void TD7() => LoadMapWithEnergy("TD7");
    public void TD8() => LoadMapWithEnergy("TD8");
    public void TD9() => LoadMapWithEnergy("TD9");
    public void TD10() => LoadMapWithEnergy("TD10");
    
    
    public void DN0() => LoadMapWithEnergy("DN0");
    public void DN1() => LoadMapWithEnergy("DN1");
    public void DN2() => LoadMapWithEnergy("DN2");
    public void DN3() => LoadMapWithEnergy("DN3");
    public void DN4() => LoadMapWithEnergy("DN4");
    public void DN5() => LoadMapWithEnergy("DN5");
    public void DN6() => LoadMapWithEnergy("DN6");
    public void DN7() => LoadMapWithEnergy("DN7");
    public void DN8() => LoadMapWithEnergy("DN8");
    public void DN9() => LoadMapWithEnergy("DN9");
}