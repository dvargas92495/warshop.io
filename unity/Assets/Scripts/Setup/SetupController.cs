﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

public class SetupController : MonoBehaviour
{
    public Button backButton;
    public Button startGameButton;
    public MaximizedRosterRobotController maximizedRosterRobot;
    public RobotRosterPanelController robotRosterPanel;
    public SceneReference lobbyScene;
    public SceneReference initialScene;
    public Sprite[] robotDir;
    public SquadPanelController mySquadPanel;
    public SquadPanelController opponentSquadPanel;
    public StatusModalController statusModal;
    public Text starText;
    
    void Start ()
    {
        BaseGameManager.InitializeSetup(this);
        
        mySquadPanel.SetAddCallback(AddSelectedToMySquad);
        startGameButton.onClick.AddListener(StartGame);
        robotRosterPanel.SetMaximizeCallback(maximizeSelection);
        robotDir.ToList().ForEach(robotRosterPanel.AddRobotImage);
    }

    public void EnterLobby()
    {
        SceneManager.LoadScene(lobbyScene);
    }

    public void EnterInitial()
    {
        SceneManager.LoadScene(initialScene);
    }

    public void maximizeSelection(Sprite selectedRobot)
    {
        maximizedRosterRobot.Select(selectedRobot);
        mySquadPanel.squadPanelButton.interactable= true;
        opponentSquadPanel.squadPanelButton.interactable=true;
    }

    public void AddSelectedToMySquad(SquadPanelController squadPanel)
    {
        UpdateStarText();
        AddSelectedToSquad(squadPanel, RemoveAddedFromMySquad);
    }

    public void AddSelectedToOpponentSquad(SquadPanelController squadPanel)
    {
        AddSelectedToSquad(squadPanel, RemoveAddedFromOpponentSquad);
    }

    public void AddSelectedToSquad(SquadPanelController squadPanel, UnityAction<RobotSquadImageController> removeCallback)
    {
        RobotSquadImageController addedRobot = squadPanel.AddRobotSquadImage();
        addedRobot.SetRemoveCallback(removeCallback);
        addedRobot.SetSprite(maximizedRosterRobot.GetRobotSprite());

        maximizedRosterRobot.Hide();
        mySquadPanel.squadPanelButton.interactable =(false);
        opponentSquadPanel.squadPanelButton.interactable =(false);
    }

    public void RemoveAddedFromMySquad(RobotSquadImageController robot)
    {
        UpdateStarText();
        RemoveAddedFromSquad(robot, mySquadPanel);
    }

    public void RemoveAddedFromOpponentSquad(RobotSquadImageController robot)
    {
        RemoveAddedFromSquad(robot, opponentSquadPanel);
    }

    public void RemoveAddedFromSquad(RobotSquadImageController robot, SquadPanelController panel)
    {
        Destroy(robot.gameObject);
        panel.RemoveRobotSquadImage(robot);
    }

    void StartGame()
    {
        statusModal.ShowLoading();
        string[] myRosterStrings = mySquadPanel.GetSquadRobotNames();
        BaseGameManager.SendPlayerInfo(myRosterStrings, ProfileController.username);
    }

    void UpdateStarText()
    {
        starText.text = mySquadPanel.GetNumRobots().ToString() + "/" + GameConstants.MAX_ROBOTS_ON_SQUAD.ToString();
        startGameButton.interactable = (mySquadPanel.GetNumRobots() <= GameConstants.MAX_ROBOTS_ON_SQUAD);
    }
}
