using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [Header("UI Elements")]
    public GameObject healthBar;
    public Slider bosshHealthBar;
    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject gameOverPanel;
    public Dropdown controllerMode;

    public static UIManager GetInstance()
    {
        return instance;
    }

    public void Awake()
    {
        if (!instance)
            instance = this;      
        else
            Destroy(gameObject);
    }

    public void UpdateHealth(float totalHealth, float currentHealth)
    {
        if (currentHealth > 0)
        {
            for (int i = 0; i < totalHealth; i++)
            {
                if (i < currentHealth)
                {
                    // Debug.Log("第" + i + "个为true");
                    healthBar.transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    // Debug.Log("第" + i + "个为false");
                    healthBar.transform.GetChild(i).gameObject.SetActive(false);
                }
                   
            }
        }
        else
            for (int i = 0; i < totalHealth; i++)
            {
                    healthBar.transform.GetChild(i).gameObject.SetActive(false);
            }
        /*        switch (currentHealth)
                {
                    case 3:

                        break;

                    default:
                        break;
                }*/
    } 

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);

        // Time.timeScale = 0;  // 暂停游戏
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);

        Time.timeScale = 1;
    }

    // 设置Boss最大血量
    public void SetBossHealth(float maxHealth)
    {
        bosshHealthBar.transform.parent.gameObject.SetActive(true);
        bosshHealthBar.maxValue = maxHealth;
        bosshHealthBar.value = maxHealth;
    } 
    
    public void UpdateBossHealth(float health)
    {
        bosshHealthBar.value = health;
    }

    public void ShowGameOverUI(bool playerIsDead)
    {
        gameOverPanel.SetActive(playerIsDead);
        pauseButton.SetActive(!playerIsDead);
    }

    public void ChangeControllerModeText(int modeType)
    {
        
        switch (modeType)
        {
            case 0:
                controllerMode.SetValueWithoutNotify(0);
                break;
            case 1:
                controllerMode.SetValueWithoutNotify(1);
                break;
            case 2:
                controllerMode.SetValueWithoutNotify(2);
                break;
            default:
                controllerMode.SetValueWithoutNotify(0);
                break;
        }
    }
}
