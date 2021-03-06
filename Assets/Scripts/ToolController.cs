﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolController : MonoBehaviour
{
    [Header("Controller references")]
    public GameObject rightController;
    public GameObject leftController;

    public Transform cameraTransform;
    public Transform leftPointer;
    public Transform rightPointer;

    [Header("Buttons")]
    public ButtonHandler MainMenuButtonHandler;
    public ButtonHandler LeftPointerButtonHandler;
    public ButtonHandler RightPointerButtonHandler;
    public ButtonHandler ToolSelectionMenuButtonHandler;

    [Header("Menus")]

    public GameObject MainMenuPrefab;
    public float MainMenuDistance = 0f;

    public GameObject ToolSelectionMenuPrefab;
    public Transform ToolSelectionMenuTransform;

    private static ToolController _instance;
    public static ToolController Instance => _instance;

    private Tool _selectedTool;

    public List<Tool> ToolPrefabs = new List<Tool>();

    [HideInInspector]
    public List<Tool> Tools = new List<Tool>();

    private GameObject _activeMainMenu = null;
    private GameObject _activeToolSelectionMenu = null;

    public Tool SelectedTool
    {
        get => _selectedTool;
        set
        {
            foreach (Tool tool in Tools)
            {
                if (tool == value) tool.Enable();
                else tool.Disable();
            }
            _selectedTool = value;
            Debug.Log($"Selected tool changed to {value.name}");
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        MainMenuButtonHandler.OnButtonDown += ShowMainMenu;

        LeftPointerButtonHandler.OnButtonDown += (controller) => ToggleLineVisual(controller, true);
        LeftPointerButtonHandler.OnButtonUp += (controller) => ToggleLineVisual(controller, false);

        RightPointerButtonHandler.OnButtonDown += (controller) => ToggleLineVisual(controller, true);
        RightPointerButtonHandler.OnButtonUp += (controller) => ToggleLineVisual(controller, false);

        ToolSelectionMenuButtonHandler.OnButtonDown += ShowToolSelectionMenu;

        foreach (Tool tool in ToolPrefabs)
        {
            Tools.Add(Instantiate(tool, transform));
        }
        SelectedTool = Tools[0];
    }

    private void ToggleLineVisual(XRController controller, bool value)
    {
        controller.GetComponentInParent<XRInteractorLineVisual>().enabled = value;
    }

    private void ShowMainMenu(XRController controller)
    {
        if (_activeMainMenu)
        {
            CloseMainMenu();
        }
        else
        {
            Vector3 lookDirection = leftController.transform.position - cameraTransform.position;
            lookDirection.y = 0;
            _activeMainMenu = Instantiate(MainMenuPrefab, leftPointer.position + lookDirection.normalized * MainMenuDistance, Quaternion.LookRotation(lookDirection, Vector3.up));
            MainMenuController mainMenu = _activeMainMenu.GetComponent<MainMenuController>();
            mainMenu.ExitButton.onClick.AddListener(CloseMainMenu);
        }
    }

    private void ShowToolSelectionMenu(XRController controller)
    {
        if (_activeToolSelectionMenu)
        {
            Destroy(_activeToolSelectionMenu);
            _activeToolSelectionMenu = null;
        }
        else
        {
            _activeToolSelectionMenu = Instantiate(ToolSelectionMenuPrefab, ToolSelectionMenuTransform.position, ToolSelectionMenuTransform.rotation, ToolSelectionMenuTransform);
        }
    }

    private void CloseMainMenu()
    {
        Destroy(_activeMainMenu);
        _activeMainMenu = null;
    }
}
