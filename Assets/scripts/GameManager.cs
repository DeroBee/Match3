using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Private Methods

    // Start is called before the first frame update
    private void Awake()
    {
        PlayerPrefs.SetInt("HighScore", 0);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods
}