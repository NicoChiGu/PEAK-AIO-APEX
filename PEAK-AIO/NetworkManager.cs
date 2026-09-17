using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// 网络传输与底层 Photon 参数智能加固管理器。
/// 提高客户端面对弱网丢包、网络抖动与高并发 RPC 时的抗中断韧性。
/// </summary>
public static class NetworkTuningManager
{
    public static bool IsTuningApplied { get; private set; }

    public static void ApplyOptimizations()
    {
        if (!Globals.enableNetworkTuning) return;

        try
        {
            // 1. 提高断开前的最大重发上限（原版默认 5 -> 改为 8），防止网络抖动导致的误判断线
            PhotonNetwork.MaxResendsBeforeDisconnect = 8;

            // 2. 开启快速重传尝试（3 次），显著降低单包丢失带来的队头阻塞延迟
            PhotonNetwork.QuickResends = 3;

            // 3. 启用 CRC 数据包校验，主动丢弃网络层畸变包
            PhotonNetwork.CrcCheckEnabled = true;

            // 4. 确保发包频率与序列化频率稳定在 30Hz
            PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 30;

            IsTuningApplied = true;

            if (ConfigManager.Logger != null)
            {
                ConfigManager.Logger.LogInfo("[PEAK AIO][NetworkTuning] Robust Photon network parameters applied (MaxResends=8, QuickResend=3, CRC=true).");
            }
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
            {
                ConfigManager.Logger.LogError("[PEAK AIO][NetworkTuning] Failed to apply network tuning: " + ex);
            }
        }
    }

    /// <summary>
    /// 立即将本地出站命令缓冲区推送到网络底层 socket，避免在大批量操作后排队延迟。
    /// </summary>
    public static void FlushOutgoingCommands()
    {
        try
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.SendAllOutgoingCommands();
            }
        }
        catch { }
    }
}

/// <summary>
/// 网络断线感知与平滑自愈重连服务。
/// 监听 Photon 网络生命周期，当遭遇非主动的超时、服务端缓冲区溢出等异常中断时，自动执行 ReconnectAndRejoin。
/// </summary>
public class AutoReconnectService : MonoBehaviourPunCallbacks
{
    public static AutoReconnectService Instance { get; private set; }

    private string _lastRoomName;
    private bool _isIntentionalDisconnect = false;
    private Coroutine _reconnectCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }

    public override void OnJoinedRoom()
    {
        _isIntentionalDisconnect = false;
        if (PhotonNetwork.CurrentRoom != null)
        {
            _lastRoomName = PhotonNetwork.CurrentRoom.Name;
        }

        // 进房时自动应用网络加固参数
        NetworkTuningManager.ApplyOptimizations();

        if (_reconnectCoroutine != null)
        {
            StopCoroutine(_reconnectCoroutine);
            _reconnectCoroutine = null;
        }
    }

    public override void OnLeftRoom()
    {
        // 主动退出房间处理
    }

    public void MarkIntentionalDisconnect()
    {
        _isIntentionalDisconnect = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (ConfigManager.Logger != null)
        {
            ConfigManager.Logger.LogWarning(string.Format("[PEAK AIO][AutoReconnect] Disconnected from Photon: {0}", cause));
        }

        if (!Globals.enableAutoReconnect) return;
        if (_isIntentionalDisconnect) return;
        if (string.IsNullOrEmpty(_lastRoomName)) return;

        // 过滤主动退出大厅的常规断开
        if (cause == DisconnectCause.DisconnectByClientLogic || cause == DisconnectCause.None)
        {
            return;
        }

        // 针对网络异常、超时或服务端丢弃等情况进行自愈
        if (cause == DisconnectCause.ServerTimeout ||
            cause == DisconnectCause.ClientTimeout ||
            cause == DisconnectCause.DisconnectByServerLogic ||
            cause == DisconnectCause.DisconnectByServerReasonUnknown ||
            cause == DisconnectCause.Exception ||
            cause == DisconnectCause.ExceptionOnConnect)
        {
            Globals.GlobalNotifier.ShowError(Localization.T("network.reconnect_toast", cause.ToString()), 5.0f);

            if (_reconnectCoroutine != null)
            {
                StopCoroutine(_reconnectCoroutine);
            }
            _reconnectCoroutine = StartCoroutine(ReconnectRoutine());
        }
    }

    private IEnumerator ReconnectRoutine()
    {
        // 等待 1.5 秒让底层 socket 关闭并稳定网络
        yield return new WaitForSeconds(1.5f);

        if (PhotonNetwork.IsConnected)
        {
            _reconnectCoroutine = null;
            yield break;
        }

        if (ConfigManager.Logger != null)
        {
            ConfigManager.Logger.LogInfo("[PEAK AIO][AutoReconnect] Executing PhotonNetwork.ReconnectAndRejoin()...");
        }

        bool rejoining = false;
        try
        {
            rejoining = PhotonNetwork.ReconnectAndRejoin();
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
            {
                ConfigManager.Logger.LogError("[PEAK AIO][AutoReconnect] ReconnectAndRejoin exception: " + ex);
            }
        }

        if (!rejoining)
        {
            // 若无法直接 Rejoin 原房间，退化尝试重连至 Master Server
            try
            {
                PhotonNetwork.Reconnect();
            }
            catch { }
        }

        _reconnectCoroutine = null;
    }
}

/// <summary>
/// 安全 RPC 发送扩展：自动校验网络状态、ViewID 有效性，并进行全局异常熔断防护。
/// </summary>
public static class SafeNetworkExtensions
{
    public static bool SafeRPC(this PhotonView view, string methodName, RpcTarget target, params object[] parameters)
    {
        if (view == null || !PhotonNetwork.InRoom || view.ViewID <= 0) return false;

        try
        {
            view.RPC(methodName, target, parameters);
            return true;
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
            {
                ConfigManager.Logger.LogError(string.Format("[PEAK AIO][SafeRPC] Failed calling '{0}' on ViewID {1}: {2}", methodName, view.ViewID, ex.Message));
            }
            return false;
        }
    }

    public static bool SafeRPC(this PhotonView view, string methodName, Photon.Realtime.Player targetPlayer, params object[] parameters)
    {
        if (view == null || !PhotonNetwork.InRoom || view.ViewID <= 0 || targetPlayer == null) return false;

        try
        {
            view.RPC(methodName, targetPlayer, parameters);
            return true;
        }
        catch (Exception ex)
        {
            if (ConfigManager.Logger != null)
            {
                ConfigManager.Logger.LogError(string.Format("[PEAK AIO][SafeRPC] Failed calling '{0}' for player '{1}': {2}", methodName, targetPlayer.NickName, ex.Message));
            }
            return false;
        }
    }
}
