using System;
using System.Collections;
using UnityEngine;
using System.Windows.Forms;
#if LUA
using LuaInterface;
#endif

/// <summary>
/// 用于心跳检测和发起重连
/// 重连走Lua登录流程
/// </summary>
/// 
public class HeartBeaterChecker
{
	private enum RunState 
    {
        Normal,
        ApplicationPause,
        WaitKeepAliveMsg,
        Pause,
        Connecting,
    };

	private const int ConnectTypeGameReconnect   = 1;           //游戏中重连服务器方式 0 - 第一次登录方式  1 - 连接游戏服  2 - 连接登录服方式

#if LUA
    private LuaFunction m_luaReceiveHeartCallAction = null;
#endif
    private System.Action m_receiveHeartCallAction  = null;
	private RunState m_curState                     = RunState.Pause;               //很特殊的状态，不参与检测
	private int m_intervalTime                      = 0;
    private int m_intervalSystemTicksMS             = 0;
    private DateTime m_nowTime                      = DateTime.Now;
	private int m_lastBeatTicksMS                   = 0;
    private int m_intervalCheckTime                 = 0;
    public int DefaultHeartInternalTimeFromPause    = 2 * 1000; //从后台返回首次心跳检测发送间隔，2秒（防止异常时间间隔少于10秒当加速处理）
	public int DefaultHeartInternalTime             = 11 * 1000; //心跳检测发送间隔，10秒 + 1秒（防止异常时间间隔少于10秒当加速处理）
    public int DefaultReceiveHeartTime              = 15 * 1000; //15秒没有收到返回就主动断线重连

    private uint m_resVersion = 0;
        
    public DateTime LoginTime ;
    #region 根据返回时间记录为ping值

    private DateTime lastHeartTime = DateTime.Now;//记录上一次发送心跳时间
    private int heartOnceInterval = 0;//单位:Millisecond  延迟

    #endregion

    public bool Initialize(bool sharedState = true)
	{
        m_curState              = RunState.Pause;
        m_intervalCheckTime     = DefaultHeartInternalTime;
        m_intervalTime          = 0;
        m_intervalSystemTicksMS = 0;
        m_nowTime               = DateTime.Now;
		m_lastBeatTicksMS       = Utils.GetSystemTicksMS();
		Networker.Instance.Register(MessageType.MSG_GS2U_KeepAlive, OnGS2U_KeepAlive);
        Networker.Instance.onStateChanged += ( state ) =>
        {
            OnNetStateChanged( state );
        };
        lastHeartTime           = DateTime.Now;

        return true;
	}

    public void Uninitialize()
	{
        Pause();
		Networker.Instance.UnRegister(MessageType.MSG_GS2U_KeepAlive, OnGS2U_KeepAlive);
	}

    #region 逻辑更新,处理心跳包
    public void Update()
	{
        if(m_curState == RunState.Connecting || m_curState == RunState.Pause || m_curState == RunState.ApplicationPause)
        {
            return;
        }

        m_intervalTime += (int)(DateTime.Now - m_nowTime).TotalMilliseconds;
        m_nowTime = DateTime.Now;

        m_intervalSystemTicksMS = Utils.GetSystemTicksMS() - m_lastBeatTicksMS;

        if(m_curState == RunState.Normal)
        {
            if(m_receiveHeartCallAction != null)
            {
                m_receiveHeartCallAction();
                m_receiveHeartCallAction = null;
            }
#if LUA
            if(m_luaReceiveHeartCallAction != null)
            {
                m_luaReceiveHeartCallAction.Call();
                m_luaReceiveHeartCallAction = null;
            }
#endif
            if(m_intervalTime >= m_intervalCheckTime || m_intervalSystemTicksMS >= m_intervalCheckTime)
            {
                SendHeartBeat();
            }
        }
        else if(m_curState == RunState.WaitKeepAliveMsg)
        {
            if (m_intervalTime >= DefaultReceiveHeartTime)
			{
				SetHeartBeatOutTime("heartOutTime");
			}
        }
	}
#endregion

    #region 网络状态变化
    /// <summary>
    /// 网络状态变化时超时按超时处理
    /// </summary>
    /// <param name="netState"></param>
    public void OnNetStateChanged(SocketClient.Status netState)
    {
        if(m_curState == RunState.Pause)
        {
            return;
        }
        if(netState == SocketClient.Status.Disconnected)
        {
            SetHeartBeatOutTime("netStateChanged"); //断网当心跳超时处理，直接弹出重连面板
        }
    }
    #endregion

    #region 状态判断
    public bool IsConnecting()
    {
        return m_curState == RunState.Connecting;
    }
    public void Pause()
    {
        m_curState = RunState.Pause;
    }
    public void ApplicationPause()
    {
        m_curState = RunState.ApplicationPause;
        m_intervalCheckTime = DefaultHeartInternalTimeFromPause;
    }
    public bool IsPaused()
    {
        return m_curState == RunState.Pause;
    }

    /// <summary>
    /// 当从后台切回来时判断一下距上一次SDK登录是否跨天，如果是，返回到登录重新走SDK登录流程
    /// </summary>
    public void OnApplicationReturn()
    {
        //if(GameConfigManager.Instance.loginType == LOGIN_TYPE.SDK && Application.platform == RuntimePlatform.Android)
        //{
        //    if (GameSceneManager.Instance.localPlayer != null)
        //    {
        //        if (LoginTime!= null && LoginTime.DayOfYear != DateTime.Now.DayOfYear )
        //        {
        //            var timeSpan = DateTime.Now - LoginTime;
        //            if(timeSpan.TotalSeconds > 180)
        //            {
        //                BackToLogin();
        //            }
        //        }
        //    }
        //}
    }
    #endregion

    #region 重连成功后的状态恢复

    /// <summary>
    /// 重连成功后的状态恢复
    /// </summary>
    public void Resume()
	{
        if( IsConnecting() )
        {
            Networker.Instance.SendCacheMsgs();
        }
        m_curState = RunState.Normal;
        m_intervalCheckTime = DefaultHeartInternalTime;
        m_intervalTime = 0;  //确保重连成功后立刻发送一条消息确认
        m_intervalSystemTicksMS = 0;
        m_lastBeatTicksMS = Utils.GetSystemTicksMS();
        m_nowTime = DateTime.Now;
        LuaScriptReset();
        NetDelayChecker.Instance.ClearDelay();
    }

    private void LuaScriptReset()
    {
        //关闭重连提示
        /*var LoadingPanel = UIManager.Instance.FindUISceneByName<LuaUIBaseExecutor>( "LoadingPanel" );
        if( LoadingPanel != null && LoadingPanel.isActiveAndEnabled)
        {
            LuaTable m_luaTable = LoadingPanel.GetLuaTable() as LuaTable;
            LuaFunction enableLoading = m_luaTable.RawGet<string,LuaFunction>("EnableLoading");  
            if( enableLoading != null )
            {
                enableLoading.Call<object>( false );
            }
        }
        //主界面刷新
        var mainMenuPanel = UIManager.Instance.FindUISceneByName<LuaUIBaseExecutor>( "NewMainMenuPanel" );
        if( mainMenuPanel != null)
        {
            LuaTable m_luaTable = mainMenuPanel.GetLuaTable() as LuaTable;
            LuaFunction updateSecondCDTime = m_luaTable.RawGet<string,LuaFunction>("ResetUpdateSecondCDTime");  //重置刷新CD时间
            if( updateSecondCDTime != null )
            {
                updateSecondCDTime.Call();
            }
        }*/
    }
    #endregion

#region 心跳消息发送和接收处理
#if LUA
    public void SendHeartBeatInLua(LuaFunction luaf = null)
    {
        SendHeartBeat(null);
        m_luaReceiveHeartCallAction = luaf;
    }
#endif


	/// <summary>
    /// 发送心跳
    /// </summary>
    /// <param name="callAction"></param>
    /// <param name="forceSend"></param>
    /// <returns></returns>
    public bool SendHeartBeat(System.Action callAction = null, bool forceSend = false)
	{
        if( m_intervalTime < m_intervalCheckTime && 
            m_intervalSystemTicksMS < m_intervalCheckTime && 
            forceSend == false)
        {
            Resume();
            return false;
        }
		if (Networker.Instance.isConnected)
		{
//#if UNITY_EDITOR
//                Common.UDebug.LogWarning("发送心跳时间系统间隔:" + (Common.Utils.GetSystemTicksMS() - m_lastBeatTicksMS));
//#endif
            NetSender.Instance.SendRequestKeepAlive();
			m_curState                      = RunState.WaitKeepAliveMsg;
			m_intervalTime                  = 0;
            m_intervalSystemTicksMS         = 0;
            m_lastBeatTicksMS               = Utils.GetSystemTicksMS();
            m_nowTime                       = DateTime.Now;
            lastHeartTime                   = DateTime.Now;

        }
        else
		{
            SetHeartBeatOutTime("net disconnect"); //当心跳超时逻辑处理，重连检测
		}
        m_receiveHeartCallAction = callAction;
		return true;
	}
	/// <summary>
    /// 心跳超时处理
    /// </summary>
    public void SetHeartBeatOutTime(string reason = "unknown")
	{
        if( !IsConnecting() )
        {
            m_curState = RunState.Connecting;
            m_intervalTime = 0;
            m_intervalSystemTicksMS = 0;

            /*if( !DownLoadAssetHelper.IsLoginState )
            {
                LuaCallCS.LocalPlayerStop();
                GameSceneManager.Instance.ClosePhotoPanel(); //相机恢复
            }
            else
            {
                Common.ULogFile.sharedInstance.LogEx( "SocketClient", string.Format( "ReConnection By {0},ping = {1}ms", reason, heartOnceInterval ) );
            }
            //由于创角之前的逻辑不会走心跳的检测，DownLoadAssetHelper.IsLoginState为True时只会是创角流程，这里直接连游戏服即可  
            StartConnection( DownLoadAssetHelper.IsLoginState ? 0 : 1 );      //走登录流程，先发送logout，再断开连接再重连游戏服务器  */
            StartConnection(0);
            return;
        }
    }



	/// <summary>
    /// 心跳消息处理
    /// </summary>
    /// <param name="msg"></param>
    public void OnGS2U_KeepAlive(BaseMessage msg)
	{
        var pingvalue = Utils.GetSystemTicksMS() - m_lastBeatTicksMS;
        NetDelayChecker.Instance.SetHeartDelay(pingvalue);
        var keepAlive = msg as GS2U_KeepAlive;
        //前16位代表安卓版本号，后16位代表ios版本号
#if UNITY_IPHONE
        m_resVersion = keepAlive.m_resVersion & 0xFFFF;
#else
        m_resVersion = keepAlive.m_resVersion >> 16;
#endif
#if LUA
        LuaScriptManager.GetInstance().SetLuaServerTime(keepAlive.m_timesTamp);
#endif
		Resume();

        heartOnceInterval = DateTime.Now.Second == lastHeartTime.Second ? (DateTime.Now.Millisecond - lastHeartTime.Millisecond) : (DateTime.Now - lastHeartTime).Milliseconds;           

    }
#endregion

    /// <summary>
    /// 开始重连
    /// </summary>
    public void StartConnection( int type = 1 )
    {
        NetDelayChecker.Instance.ClearDelay();
        /*var loadingPanel = LuaCallCS.TryOpenUIScene( "LoadingPanel", UISceneChain.ReconnectGameLoadingPanel, "ui/LoginPanel/LoadingPanel.lua", 3 );
        if( loadingPanel != null )
        {
            LuaTable m_luaTable = loadingPanel.GetLuaTable() as LuaTable;
            m_luaTable.Call( "SetConnectType", type );//设置连接类型
            if( type == 1 )
            {
                m_luaTable.Call( "SetPlayerID", GameSceneManager.Instance.localPlayer.code );//设置连接类型
            }
            m_luaTable.Call( "StartGSConnecting" ); //连接游戏服第一步
        }
        LuaCallCS.CloseWaitMsgPanel();
        LuaCallCS.CloseSampleWait();
        LuaCallCS.CloseWaitPanel();*/
    }

    #region 返回到登录界面
    /*public static IEnumerator DoBackToLogin()
    {
        try
        {
            BattleUIManager.Instance.Uninitialize();
            GameSceneManager.Instance.localPlayer.BackToLogin();
            var list = PlayerRoleManager.Instance.GetLocalPlayerRoleList();
            for (int ix = 0; ix < list.Count; ix++)
            {
                GameSceneManager.Instance.entities.Remove(list[ix].code);
                list[ix].Uninitialize();
            }
            PlayerRoleManager.Instance.Uninitialize();
            GameSceneManager.Instance.UnloadSceneEntitys();
            SinglePanelManger.Instance.GameLoadingPanel.SetLoadingProgress(0.2f);
            yield return Common.Root.defaultWaitForEndOfFrame;
            GameSceneManager.Instance.LoadNullLevel();
            SinglePanelManger.Instance.GameLoadingPanel.SetLoadingProgress(0.4f);
            yield return Common.Root.defaultWaitForEndOfFrame;
            ClientProxy.LoadLoginScene();
            SinglePanelManger.Instance.GameLoadingPanel.SetLoadingProgress(0.6f);
            yield return Common.Root.defaultWaitForEndOfFrame;
            GameSceneManager.Instance.ForceLoadSceneAssets();
            SinglePanelManger.Instance.EnableRefreshResources(true);
            CameraManager.Instance.EnableMainCamera(false);
            CameraManager.Instance.EnableCameraControl(false);
            SinglePanelManger.Instance.GameLoadingPanel.SetLoadingProgress(0.8f);
            yield return Common.Root.defaultWaitForEndOfFrame;
            CameraManager.Instance.LoadCamera();
            CameraManager.Instance.SetCameraLayerCullDistances(200);
            HUD.NHUD.HUDSchemeManager.Destroy();
            yield return Common.Root.defaultWaitForEndOfFrame;
            LuaExecState luaE = new LuaExecState();
            luaE.InitLua("gamestate/LoginState.lua");
            StateMachine.Instance.ChangeState(luaE);
            SinglePanelManger.Instance.GameLoadingPanel.SetLoadingProgress(1.0f);
            yield return Common.Root.defaultWaitForEndOfFrame;
            yield return Common.Root.defaultWaitForEndOfFrame;
            ClientProxy.InitLoginSceneEffect();
            SinglePanelManger.Instance.GameLoadingPanel.EndLoading();
        }
        finally
        {
            //Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }*/
    /// <summary>
    /// 返回到登录界面
    /// </summary>
    public void BackToLogin(bool forceBackLogin = false)
    {
#if UNITY_ANDROID || UNITY_IPHONE
            
        if (!forceBackLogin && !GameSceneManager.Instance.IsLoadingAllOk && m_curState != RunState.Connecting)
        {
            //正在切场景的时候，收到sdk的退出消息，如果马上在切换到登陆界面，android上游戏会闪退
            return;
        }
#endif
        /*
        LuaScriptManager.Instance.CallPushWithVoid("ScreenOrienttionMgr.ResetOrientation");
        var lvName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (lvName.Equals(AssetsPathManager.DefaultLoginScene))
        {
            LuaUIBaseExecutor GameServerPanel = (LuaUIBaseExecutor)UIManager.Instance.FindUISceneByNameInLua("GameServerPanel");
            if (GameServerPanel)
            {
                LuaTable LuaClass = (LuaTable)GameServerPanel.GetLuaTable();
                if (LuaClass != null)
                {
                    var tempRef = LuaClass.RawGet<string, LuaFunction>("OnClickBack");
                    if (tempRef != null)
                    {
                        tempRef.Call();
						tempRef.Dispose();
                    }                      
                }                  
            }
            return;
        }

        //停止所有协程
        Common.Root.UILoadingCoro.StopAllCoroutines();*/
        //暂停心跳检测
        GameCore.Instance.HeartBeater.Pause();
        //断开网络,准备重连登录服
        Networker.Instance.Disconnect(); 
        /*
        //释放UI
        UIManager.Instance.CloseAllScene();
        UIManager.Instance.HideAllUIScene(null, string.Empty);
        //UIManager.Instance.UpdateHideScene();
        UIManager.Instance.EnableAllSceneCanvas(false);
        //释放场景
        GameSceneManager.Instance.IsChangedSceneAsset = true;
        GameSceneState.ReleaseAll();
        //打开loading面板
        var gameloading = SinglePanelManger.Instance.GameLoadingPanel;
        gameloading.StartLoading();
        gameloading.SetLoadingProgress(0f);
        gameloading.HideAllTips();
        if(gameloading is GameLoadingPanel)
        {
            ((GameLoadingPanel)gameloading).ClearAllText();
        }
        Common.Root.coro.StartCoroutine(DoBackToLogin());
        */
    }

    #endregion

    #region //lua中用于返回登陆 踢人的情况处理
    public void ShowGoLogin(string str = "", bool autoConfim = false, bool force = false)
    {
        GameCore.Instance.HeartBeater.Pause();
        string info = str;
        if (string.IsNullOrEmpty(info))
        {
            if (string.IsNullOrEmpty(str))
                info = "连接超时, 网络已断开连接";
            else
                info = str;
        }
        MessageBox.Show(info);
    }

    private static void RestartGame()
    {
#if UNITY_ANDROID && !UNITY_EDITOR //安卓更新处理
        PlatformToGame.restarAPP();
#elif UNITY_IPHONE && !UNITY_EDITOR //IOS更新处理
        PlatformToGame.restarAPP();
#elif USE_UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    } 
    #endregion
       
}
