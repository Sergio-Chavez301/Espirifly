using UnityEngine;
using UnityEngine.SceneManagement;

public class menuSystem : MonoBehaviour
{
    void Start()
    {
        Debug.Log("menuSystem activo");
    }

    public void jugar()
    {
        Debug.Log("Botón JUGAR presionado");
        int siguiente = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log("Escenas en la build: " + SceneManager.sceneCountInBuildSettings + " | Siguiente: " + siguiente);

        if (siguiente < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(siguiente);
        else
            Debug.LogError("No hay siguiente escena. Revisa File > Build Profiles > Scene List.");
    }

    public void salir()
    {
        Debug.Log("Botón SALIR presionado");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}