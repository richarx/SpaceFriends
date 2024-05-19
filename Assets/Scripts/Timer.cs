using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private float timer = 0.0f;

    private void Update()
    {
        if (timer > 0.0f)
            timer -= Time.deltaTime;

        string prefix = "";
        if (timer <= 3.0f)
            prefix = "<color=\"red\">";
        else if (timer <= 6.0f)
            prefix = "<color=\"yellow\">";

        text.text = $"{prefix}{timer:0.00}";
        
        if (IsServer && PlayerInputs.CheckForResetObjective())
            ResetTimerRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void ResetTimerRpc()
    {
        if (!text.gameObject.activeSelf)
            text.gameObject.SetActive(true);
            
        timer = 10.0f;
    }
}
