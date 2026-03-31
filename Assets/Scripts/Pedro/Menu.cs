using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void ChangeScene(int sceneidnex)
    {
        SceneManager.LoadScene(sceneidnex);
        Debug.Log("Scene changed to index: " + sceneidnex); 
    }
}
