using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualController : MonoBehaviour
{
    [SerializeField]
    private GameObject manualPanel;

    public void OnManualBtnClick()
    {
        manualPanel.SetActive(true);
    }

    public void OnManualExitBtnClick()
    {
        manualPanel.SetActive(false);
    }
}
