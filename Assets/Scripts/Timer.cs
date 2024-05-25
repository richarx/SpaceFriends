using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Timer : NetworkBehaviour
{
    public static UnityEvent<float, bool> OnTriggerTimer = new UnityEvent<float, bool>();
    
    [SerializeField] private TextMeshProUGUI timerText;

    private float timer = 0.0f;
    private bool chill = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        OnTriggerTimer?.AddListener(ResetTimerRpc);
    }

    private void Update()
    {
        if (timer > 0.0f)
            timer -= Time.deltaTime;

        timerText.text = $"{ComputePrefix()}{ComputeTimer()}";

        if (IsServer && PlayerInputs.CheckForSwapMap())
            timer = 0.0f;
    }

    private string ComputeTimer()
    {
        if (chill)
            return timer.ToString("0");
        
        return timer.ToString("0.00");
    }
    
    private string ComputePrefix()
    {
        if (chill)
            return "<color=\"green\">";
        
        if (timer <= 5.0f)
            return "<color=\"red\">";
        
        if (timer <= 10.0f)
            return "<color=\"yellow\">";

        return "";
    }

    [Rpc(SendTo.Everyone)]
    private void ResetTimerRpc(float duration, bool isChill)
    {
        if (!timerText.gameObject.activeSelf)
            timerText.gameObject.SetActive(true);

        chill = isChill;
        timer = duration;
    }
}
