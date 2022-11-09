using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class TitleManager: MonoBehaviour
{
    public CanvasGroup titleCanvasGroup;
    public CanvasGroup mainCanvasGroup;
    public SpriteRenderer ghost;
    
    private float  duration =1.5f;

    private void Start()
    {
        SaveData.SavePlayerData(new SaveData.FailedData());
    }
    
    public void ChangeScene()
    {
        titleCanvasGroup.blocksRaycasts = false;

        ghost.DOFade(0, duration);

        titleCanvasGroup.DOFade(0, duration).OnComplete(
            () =>
            {
                mainCanvasGroup.DOFade(1, duration).OnComplete(
                    () =>
                    {
                        mainCanvasGroup.blocksRaycasts = true;
                    });
            });
        /*
        CanvasGroup.DOFade(0, 0.8f).OnComplete(
            () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
            });
        */
    }
}