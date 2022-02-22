using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject InstructionsPanel;
    void Update()
    {
        if (Input.GetButtonDown("F1"))
        {
            ToggleInstructionsPanel();
        }
    }

    private void ToggleInstructionsPanel()
    {
        InstructionsPanel.SetActive(!InstructionsPanel.activeInHierarchy);
    }
}
