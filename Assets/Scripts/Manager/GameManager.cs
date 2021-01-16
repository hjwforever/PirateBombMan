using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager  : MonoBehaviour
{
    public static GameManager instance;
    private PlayerController player;
    private Door exitDoor;
    private VariableJoystick variableJoystick;

    public bool gameOver;

    public List<Enemy> enemyList;
    public static GameManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);

      /*
       * 改用观察者模式实现这些代码     
        player = FindObjectOfType<PlayerController>();
        exitDoor = FindObjectOfType<Door>();*/
    }

    // 保存游戏数据
    public void Save()
    {
        // 保存血量
        PlayerPrefs.SetFloat("playerHealth", player.health);
        PlayerPrefs.Save();
    }

    // 加载游戏数据
    public void Load()
    {
        // 加载血量
        if (!PlayerPrefs.HasKey("playerHealth"))
        {
            // 设置血量数据
            Debug.Log("设置血量数据，血量: " + player.maxHealth);
            PlayerPrefs.SetFloat("playerHealth", player.maxHealth);
        }
        else
        {
            // 加载血量数据
            Debug.Log("加载血量数据，血量: " + PlayerPrefs.GetFloat("playerHealth"));
            player.health = PlayerPrefs.GetFloat("playerHealth");
        }

        if (player.health != player.maxHealth)
        {
            UIManager.GetInstance().UpdateHealth(player.maxHealth, player.health);
        }

        UIManager.GetInstance().ChangeControllerModeText(PlayerPrefs.GetInt("ControllerMode"));
    }

    // 重新开始游戏
    public void ReStart()
    {
        SceneManager.LoadScene(1);
        PlayerPrefs.SetFloat("playerHealth", player.maxHealth);
        Time.timeScale = 1;
    }
    
    // 满血重新开始本关卡
    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("playerHealth", player.maxHealth);
    }

    // 暂停游戏
    public void PauseGame()
    {
        FindObjectOfType<UIManager>().PauseGame();
        Time.timeScale = 0;  // 时间静止
    }

    // 保存游戏进度
    public void SaveGame(int level = 1, float health = 3)
    {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetFloat("playerHealth", health);
    }

    // 加载游戏进度
    public void LoadGame(int level)
    {
        // 无存档则提示
        if (!PlayerPrefs.HasKey("level") || !PlayerPrefs.HasKey("playerHealth"))
        {
            Debug.Log("无存档，从第一关开始");
        }

        // 加载进度, 默认从第一关重新开始
        SceneManager.LoadScene(level > 0 ? level : PlayerPrefs.GetInt("level", 1));

        // 存档
        PlayerPrefs.SetInt("level", level);
        // PlayerPrefs.SetFloat("playerHealth", player.health);
        if (level > 0)
            PlayerPrefs.SetFloat("playerHealth", PlayerPrefs.HasKey("playerHealth") ? PlayerPrefs.GetFloat("playerHealth") : player.maxHealth);
        else
            PlayerPrefs.SetFloat("playerHealth", player.maxHealth);

        Time.timeScale = 1;
    }

    // 改变游戏进度
    public void ChangeScene(int level = 1, float health = 3)
    {
        SceneManager.LoadScene(level);
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetFloat("playerHealth", health);
        Time.timeScale = 1;

    }

    // 退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }

    // 返回主界面
    public void ReturnMain()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void NextScence()
    {
        Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }

    public void SetDoor(Door door)
    {
        this.exitDoor = door;
    }

    // 将敌人添加至敌人列表
    public void AddToEnemyList(Enemy enemy)
    {
        enemyList.Add(enemy);
    } 
    
    // 将敌人从敌人列表移除
    public void RemoveFromEnemyList(Enemy enemy)
    {
        if (enemyList.Contains(enemy))
        {
            enemyList.Remove(enemy);
            if (enemyList.Count == 0)
                exitDoor.OpenDoor();
        }
        
    }

    // 判断敌人是否在敌人列表
    public bool InEnemyList(Enemy enemy)
    {
        return enemyList.Contains(enemy);
    }
}
