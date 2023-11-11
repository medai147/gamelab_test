using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{

    [Header("保存先の設定")]
    [SerializeField]
    string folderName = "Screenshots";

    bool isCreatingScreenShot = false;
    string path;
    //SoundManager soundManager;

    void Start()
    {
        path = Application.dataPath + "/Resources/" + folderName + "/";
    }

    public void PrintScreen()
    {
        StartCoroutine("PrintScreenInternal");
        
    }

    IEnumerator PrintScreenInternal()
    {
        if (isCreatingScreenShot)
        {
            yield break;
        }

        isCreatingScreenShot = true;

        yield return null;

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string date = DateTime.Now.ToString("yy-MM-dd_HH-mm-ss");
        string fileName = path + date + ".png";

        ScreenCapture.CaptureScreenshot(fileName);

        GameManager.instance.screenshotpath = folderName + "/" + date;


        yield return new WaitUntil(() => File.Exists(fileName));

        isCreatingScreenShot = false;
    }

}