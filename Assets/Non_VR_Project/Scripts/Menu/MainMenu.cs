using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    Vector3 Position = new Vector3(10.88206f, 5.523015f, 1.11195f);
    Vector3 Rotation = new Vector3(-3.917f, -36.477f, 0f);
    public GameObject MainMenu1;
    public GameObject PlayMenu;
    public GameObject SettingMenu;


    private void Awake()
    {
        instance = this;
    }
    //public void PlayerGame() {
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //}

    public void OnMouseClickPlay()
    {
        CameraInstance.instance.transform.position = new Vector3(-1.387136f, 2.042882f, 8.692932f);
        CameraInstance.instance.transform.rotation = Quaternion.Euler(new Vector3(-1.583f, -40.604f, 0));
        MainMenu1.SetActive(false);
        PlayMenu.SetActive(true);
        SettingMenu.SetActive(false);
    }
    public void OnMouseOverPlay()
    {
        CameraInstance.instance.transform.DOMove(new Vector3(-1.387136f, 2.042882f, 8.692932f), 0.5f).SetEase(Ease.InOutQuad);
        CameraInstance.instance.transform.DORotate(new Vector3(-1.583f, -40.604f, 0), 0.5f).SetEase(Ease.InOutQuad);
    }
    public void OnMouseOutPlay()
    {
        CameraInstance.instance.transform.DOMove(Position, 0.5f).SetEase(Ease.InOutQuad);
        CameraInstance.instance.transform.DORotate(Rotation, 0.5f).SetEase(Ease.InOutQuad);
    }

    public void OnMouseOverSetting()
    {
        CameraInstance.instance.transform.DOMove(new Vector3(6.479673f, 2.458701f, 9.382029f), 0.5f).SetEase(Ease.InOutQuad);
        CameraInstance.instance.transform.DORotate(new Vector3(0.208f, 0.306f, 0), 0.5f).SetEase(Ease.InOutQuad);
    }
    public void OnMouseOutSetting()
    {
        CameraInstance.instance.transform.DOMove(Position, 0.5f).SetEase(Ease.InOutQuad);
        CameraInstance.instance.transform.DORotate(Rotation, 0.5f).SetEase(Ease.InOutQuad);
    }


    public void OnMouseOverQuit()
    {
        CameraInstance.instance.transform.DOMove(new Vector3(11.41322f, 4.000476f, 8.336395f), 0.5f).SetEase(Ease.InOutQuad);
        CameraInstance.instance.transform.DORotate(new Vector3(-0.823f, 23.511f, 0), 0.5f).SetEase(Ease.InOutQuad);
    }
    public void OnMouseOutQuit()
    {
        CameraInstance.instance.transform.DOMove(Position, 0.5f).SetEase(Ease.InOutQuad);
        CameraInstance.instance.transform.DORotate(Rotation, 0.5f).SetEase(Ease.InOutQuad);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A)) CameraInstance.instance.transform.position = new Vector3(0, 0, 0);
    }
}
