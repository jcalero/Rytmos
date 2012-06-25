using UnityEngine;
using System.Collections;

public class GameManagerSpawner : MonoBehaviour {

    #region Fields
    public GameObject gmPrefab;
    private GameObject gm;
    private GameObject gmClone;
    #endregion

    #region Functions
    void Awake() {
        gm = GameObject.Find("GameManager");
        gmClone = GameObject.Find("GameManager(Clone)");
        if (gm == null && gmClone == null) {
            GameObject gmInstance = (GameObject)Instantiate(gmPrefab, Vector3.zero, Quaternion.identity);
            gmInstance.GetComponent<Game>().isTemp = true;
        }
    }
    #endregion
}
