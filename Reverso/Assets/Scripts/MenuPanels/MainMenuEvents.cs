﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;

public class MainMenuEvents : MonoBehaviour
{
    private System.Action<bool> _authCallback;
    private bool _signingIn = false;

    public GameObject[] DeactivateTheseGameObjectsForIOS;

    public Toggle hintToggle;
    public Toggle speedModeToggle;

    public GameObject[] tweenDown;
    public GameObject[] tweenUp;
    public float tweenDuration = 1f;
    public LeanTweenType tweenType;
    void Awake()
    {




    }
    void Start()
    {
#if UNITY_IOS
        foreach (GameObject item in DeactivateTheseGameObjectsForIOS)
        {
            item.SetActive(false);
        }
#endif
        hintToggle.isOn = ReversoPlayerPrefs.IsHintsOn();
        _authCallback = (bool success) =>
        {

            Debug.Log("In Auth callback, success = " + success);

            _signingIn = false;
            if (success)
            {
                Debug.Log("Auth SUCCESS!!");
            }
            else
            {
                Debug.Log("Auth failed!!");
            }
        };
        if (ReversoPlayerPrefs.ShouldLogIn())
        {
            ConfigPlayGames();
            ReversoPlayerPrefs.SetShouldLogIn(false);
        }
        AnimateUI();

    }
    void AnimateUI()
    {
        foreach (var tween in tweenDown)
        {
            var rectT = tween.GetComponent<RectTransform>();
            var startPosY = rectT.localPosition.y;
            rectT.anchoredPosition += Vector2.up * 400f;
            LeanTween.moveLocalY(tween, startPosY, 1f).setEase(tweenType).setDelay(0f);
        }
        foreach (var tween in tweenUp)
        {
            var rectT = tween.GetComponent<RectTransform>();
            var startPosY = rectT.localPosition.y;
            rectT.anchoredPosition -= Vector2.up * 400f;
            LeanTween.moveLocalY(tween, startPosY, 1f).setEase(tweenType).setDelay(0f);
        }

    }
    void Update()
    {
        UpdateInvitation();

        if (!PlayGamesPlatform.Instance.IsAuthenticated()) return;

    }
    public void UpdateInvitation()
    {

        if (InvitationManager.Instance == null)
        {
            return;
        }
        Invitation inv = InvitationManager.Instance.Invitation;
        if (inv != null)
        {
            if (InvitationManager.Instance.ShouldAutoAccept)
            {
                InvitationManager.Instance.Clear();
                //TODO OthelloManager//RaceManager.AcceptInvitation(inv.InvitationId);
                NavigationUtil.ShowPlayingPanel();
            }
            else
            {
                NavigationUtil.ShowInvitationPanel();
            }
        }
    }
    public void ConfigPlayGames()
    {
        var config = new PlayGamesClientConfiguration.Builder()
           .WithInvitationDelegate(InvitationManager.Instance.OnInvitationReceived)
           .Build();
        PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.DebugLogEnabled = true;
        Authorize(false);

    }
    void Authorize(bool silent)
    {
        if (!_signingIn && !PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Debug.Log("Starting sign-in...");
            _signingIn = true;
            PlayGamesPlatform.Instance.Authenticate(_authCallback, silent);

        }

        else
        {
            Debug.Log("Already started signing in");
        }

    }




    public void OnThemesClicked()
    {
        NavigationUtil.ShowThemesPanel();
    }





    public void HintsToggle_ValueChanged()
    {
        ReversoPlayerPrefs.SetHints(hintToggle.isOn);
    }

    public void OnTwoPlayer()
    {
        OthelloManager.StartVersus();
        OthelloManager.Instance.UseHints = hintToggle.isOn;
        OthelloManager.Instance.SpeedMode = speedModeToggle.isOn;
    }

    public void OnSinglePlayer()
    {
        NavigationUtil.ShowComputerLevelSelectionPanel();
        OthelloManager.Instance.UseHints = hintToggle.isOn;
        OthelloManager.Instance.SpeedMode = speedModeToggle.isOn;
    }
    public void StartComputerEasy()
    {
        OthelloManager.StartComputer();
        OthelloManager.Instance.ComputerLevel = OthelloManager.ComputerLevelEnum.One;
    }
    public void StartComputerNormal()
    {
        OthelloManager.StartComputer();
        OthelloManager.Instance.ComputerLevel = OthelloManager.ComputerLevelEnum.Two;
    }
    public void StartComputerHard()
    {
        OthelloManager.StartComputer();
        OthelloManager.Instance.ComputerLevel = OthelloManager.ComputerLevelEnum.Three;
    }
    public void StartComputerVeryHard()
    {
        OthelloManager.StartComputer();
        OthelloManager.Instance.ComputerLevel = OthelloManager.ComputerLevelEnum.Four;
    }

    public void OnPlayOnline()
    {
        print("PLAY ONLINE");
#if UNITY_EDITOR
        NavigationUtil.ShowOnlineMenuPanel();
#endif
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {

            NavigationUtil.ShowOnlineMenuPanel();
        }
        else
        {
            Authorize(false);
        }

    }

    public void OnLogout()
    {
        if (PlayGamesPlatform.Instance != null)
        {
            PlayGamesPlatform.Instance.SignOut();

        }

    }

    public void OnOptionsPressed()
    {
        NavigationUtil.ShowOptionsPanel();
    }
}
