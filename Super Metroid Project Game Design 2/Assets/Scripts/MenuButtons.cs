using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
 public void playGame(){
    SceneManager.LoadScene("ceres_1");
 }

 public void quitGame(){
    Application.Quit();
 }
}
