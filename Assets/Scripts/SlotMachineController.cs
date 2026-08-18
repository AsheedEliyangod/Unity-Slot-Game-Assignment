using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotMachineController : MonoBehaviour
{
    // =========================================================
    // REELS
    // =========================================================

    [Header("Reels")]
    public SlotReel reel1;
    public SlotReel reel2;
    public SlotReel reel3;


    // =========================================================
    // LEVER
    // =========================================================

    [Header("Lever")]
    public Button leverButton;

    public GameObject leverUp;
    public GameObject leverDown;

    [Header("Lever Animation")]
    public float leverSwitchDelay = 0.12f;


    // =========================================================
    // AUDIO
    // =========================================================

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip leverClick;
    public AudioClip slotMachineWheel;
    public AudioClip slotMachineWin;
    public AudioClip casinoBellsReward;
    public AudioClip payoutAward;
    public AudioClip clinkingCoins;


    [Header("Audio Volume")]

    [Range(0f, 1f)]
    public float leverVolume = 0.8f;

    [Range(0f, 1f)]
    public float spinningVolume = 0.45f;

    [Range(0f, 1f)]
    public float winVolume = 0.8f;

    [Range(0f, 1f)]
    public float jackpotVolume = 1f;

    [Range(0f, 1f)]
    public float payoutVolume = 0.8f;

    [Range(0f, 1f)]
    public float coinVolume = 0.7f;


    // =========================================================
    // COINS
    // =========================================================

    [Header("Coins")]

    public CoinManager coinManager;

    [Tooltip("Coins required for every spin.")]
    public int spinCost = 5;

    [Tooltip("Coins awarded for 777.")]
    public int jackpotReward = 50;

    [Tooltip("Coins awarded for BAR BAR BAR.")]
    public int barReward = 30;

    [Tooltip("Coins awarded for CHERRY CHERRY CHERRY.")]
    public int cherryReward = 20;

    [Tooltip("Coins awarded for BELL BELL BELL.")]
    public int bellReward = 10;


    // =========================================================
    // RESULT UI
    // =========================================================

    [Header("Result UI")]
    public TMP_Text resultText;

    // NEW
    public TMP_Text rewardText;

    [Tooltip("How long temporary messages stay visible.")]
    public float temporaryMessageDuration = 1.5f;


    // =========================================================
    // RESULT ANIMATION
    // =========================================================

    [Header("Result Animation")]

    [Tooltip("Enable the result text pop animation.")]
    public bool animateResult = true;

    [Tooltip("How large the result becomes during the pop.")]
    public float resultPopScale = 1.25f;

    [Tooltip("How quickly the result pops in.")]
    public float resultPopSpeed = 8f;

    [Tooltip("How long the result stays at full size.")]
    public float resultHoldTime = 0.12f;

    [Tooltip("How quickly the result returns to normal size.")]
    public float resultReturnSpeed = 6f;


    // =========================================================
    // INTERNAL
    // =========================================================

    private bool isSpinning = false;

    private Coroutine resultAnimationCoroutine;
    private Coroutine clearResultCoroutine;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Lever starts UP
        SetLeverUp();


        // Check CoinManager
        if (coinManager == null)
        {
            Debug.LogError(
                "Slot Machine: Coin Manager is NOT assigned!"
            );
        }


        // Connect button
        if (leverButton != null)
        {
            leverButton.onClick.RemoveAllListeners();
            leverButton.onClick.AddListener(PullLever);
        }


        // Clear result
        if (resultText != null)
        {
            resultText.text = "";
            resultText.transform.localScale = Vector3.one;
        }


        // NEW - Clear reward
        if (rewardText != null)
        {
            rewardText.text = "";
            rewardText.transform.localScale = Vector3.one;
        }


        // Configure audio
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
        }
    }


    // =========================================================
    // PULL LEVER
    // =========================================================

    public void PullLever()
    {
        if (isSpinning)
            return;


        // Make sure CoinManager exists
        if (coinManager == null)
        {
            Debug.LogError(
                "Slot Machine: Coin Manager is NOT assigned!"
            );

            return;
        }


        // =====================================================
        // CHECK COINS
        // =====================================================

        if (coinManager.Coins < spinCost)
        {
            ShowResult("NOT ENOUGH COINS!");

            Debug.Log(
                "Not enough coins! Current: " +
                coinManager.Coins +
                " | Required: " +
                spinCost
            );


            clearResultCoroutine =
                StartCoroutine(
                    ClearResultAfterDelay(
                        temporaryMessageDuration
                    )
                );


            return;
        }


        StartCoroutine(PullAndSpin());
    }


    // =========================================================
    // MAIN SLOT MACHINE SEQUENCE
    // =========================================================

    private IEnumerator PullAndSpin()
    {
        isSpinning = true;


        // Disable button while machine is running
        if (leverButton != null)
        {
            leverButton.interactable = false;
        }


        // Clear previous result
        ClearResultImmediately();


        // =====================================================
        // PAY FOR SPIN
        // =====================================================

        bool paymentSuccessful =
            coinManager.RemoveCoins(spinCost);


        // Safety check
        if (!paymentSuccessful)
        {
            ShowResult("NOT ENOUGH COINS!");


            isSpinning = false;


            if (leverButton != null)
            {
                leverButton.interactable = true;
            }


            clearResultCoroutine =
                StartCoroutine(
                    ClearResultAfterDelay(
                        temporaryMessageDuration
                    )
                );


            yield break;
        }


        Debug.Log(
            "Spin cost: -" +
            spinCost +
            " | Remaining coins: " +
            coinManager.Coins
        );


        // =====================================================
        // LEVER PULL
        // =====================================================

        SetLeverDown();


        PlaySound(
            leverClick,
            leverVolume
        );


        yield return new WaitForSeconds(
            leverSwitchDelay
        );


        // =====================================================
        // START SPINNING SOUND
        // =====================================================

        StartSpinningSound();


        // =====================================================
        // START ALL THREE REELS
        // =====================================================

        if (reel1 != null)
        {
            StartCoroutine(reel1.Spin());
        }


        if (reel2 != null)
        {
            StartCoroutine(reel2.Spin());
        }


        if (reel3 != null)
        {
            StartCoroutine(reel3.Spin());
        }


        // =====================================================
        // WAIT FOR ALL REELS
        // =====================================================

        while (
            (reel1 != null && reel1.IsSpinning) ||
            (reel2 != null && reel2.IsSpinning) ||
            (reel3 != null && reel3.IsSpinning)
        )
        {
            yield return null;
        }


        // =====================================================
        // STOP SPINNING SOUND
        // =====================================================

        StopSpinningSound();


        // Small pause
        yield return new WaitForSeconds(0.08f);


        // =====================================================
        // CHECK RESULT + GIVE REWARD
        // =====================================================

        CheckResult();


        // =====================================================
        // RETURN LEVER UP
        // =====================================================

        yield return new WaitForSeconds(0.1f);

        SetLeverUp();


        // =====================================================
        // ENABLE LEVER
        // =====================================================

        isSpinning = false;


        if (leverButton != null)
        {
            leverButton.interactable = true;
        }
    }


    // =========================================================
    // LEVER UP
    // =========================================================

    private void SetLeverUp()
    {
        if (leverUp != null)
        {
            leverUp.SetActive(true);
        }


        if (leverDown != null)
        {
            leverDown.SetActive(false);
        }
    }


    // =========================================================
    // LEVER DOWN
    // =========================================================

    private void SetLeverDown()
    {
        if (leverUp != null)
        {
            leverUp.SetActive(false);
        }


        if (leverDown != null)
        {
            leverDown.SetActive(true);
        }
    }


    // =========================================================
    // START SPINNING SOUND
    // =========================================================

    private void StartSpinningSound()
    {
        if (audioSource == null)
            return;


        if (slotMachineWheel == null)
            return;


        audioSource.Stop();


        audioSource.clip =
            slotMachineWheel;


        audioSource.volume =
            spinningVolume;


        audioSource.loop = true;


        audioSource.Play();
    }


    // =========================================================
    // STOP SPINNING SOUND
    // =========================================================

    private void StopSpinningSound()
    {
        if (audioSource == null)
            return;


        audioSource.loop = false;


        audioSource.Stop();


        audioSource.clip = null;
    }


    // =========================================================
    // PLAY ONE-SHOT SOUND
    // =========================================================

    private void PlaySound(
        AudioClip clip,
        float volume
    )
    {
        if (audioSource == null)
            return;


        if (clip == null)
            return;


        audioSource.PlayOneShot(
            clip,
            volume
        );
    }


    // =========================================================
    // RESULT CHECKING
    // =========================================================

    private void CheckResult()
    {
        if (
            reel1 == null ||
            reel2 == null ||
            reel3 == null
        )
        {
            Debug.LogError(
                "Slot Machine: One or more reels are missing."
            );

            return;
        }


        // Get final indexes
        int result1 =
            reel1.CurrentSymbolIndex;


        int result2 =
            reel2.CurrentSymbolIndex;


        int result3 =
            reel3.CurrentSymbolIndex;


        // Convert indexes to symbol types
        string symbol1 =
            GetSymbolType(result1);


        string symbol2 =
            GetSymbolType(result2);


        string symbol3 =
            GetSymbolType(result3);


        Debug.Log(
            "Result: " +
            symbol1 +
            " | " +
            symbol2 +
            " | " +
            symbol3
        );


        // =====================================================
        // 777 JACKPOT
        // =====================================================

        if (
            symbol1 == "7" &&
            symbol2 == "7" &&
            symbol3 == "7"
        )
        {
            ShowResult("JACKPOT!");

            AddRewardCoins(
                jackpotReward
            );

            PlayJackpotSounds();

            return;
        }


        // =====================================================
        // BAR WIN
        // =====================================================

        if (
            symbol1 == "BAR" &&
            symbol2 == "BAR" &&
            symbol3 == "BAR"
        )
        {
            ShowResult("BAR WIN!");

            AddRewardCoins(
                barReward
            );

            PlayNormalWinSounds();

            return;
        }


        // =====================================================
        // CHERRY WIN
        // =====================================================

        if (
            symbol1 == "CHERRY" &&
            symbol2 == "CHERRY" &&
            symbol3 == "CHERRY"
        )
        {
            ShowResult("CHERRY WIN!");

            AddRewardCoins(
                cherryReward
            );

            PlayNormalWinSounds();

            return;
        }


        // =====================================================
        // BELL WIN
        // =====================================================

        if (
            symbol1 == "BELL" &&
            symbol2 == "BELL" &&
            symbol3 == "BELL"
        )
        {
            ShowResult("BELL WIN!");

            AddRewardCoins(
                bellReward
            );

            PlayNormalWinSounds();

            return;
        }


        // =====================================================
        // NO WIN
        // =====================================================

        ShowResult("TRY AGAIN!");
    }


    // =========================================================
    // ADD COIN REWARD
    // =========================================================

    private void AddRewardCoins(int amount)
    {
        if (coinManager == null)
        {
            Debug.LogWarning(
                "Coin Manager is not assigned!"
            );

            return;
        }


        // Add coins
        coinManager.AddCoins(
            amount
        );


        // =====================================================
        // NEW - SHOW REWARD TEXT
        // =====================================================

        if (rewardText != null)
        {
            rewardText.text =
                "+" + amount + " COINS";

            rewardText.transform.localScale =
                Vector3.one;
        }


        Debug.Log(
            "Reward: +" +
            amount +
            " coins" +
            " | Total: " +
            coinManager.Coins
        );
    }


    // =========================================================
    // NORMAL WIN SOUNDS
    // =========================================================

    private void PlayNormalWinSounds()
    {
        PlaySound(
            slotMachineWin,
            winVolume
        );


        StartCoroutine(
            PlayRewardSoundDelayed()
        );
    }


    private IEnumerator PlayRewardSoundDelayed()
    {
        yield return new WaitForSeconds(
            0.12f
        );


        PlaySound(
            payoutAward,
            payoutVolume
        );


        yield return new WaitForSeconds(
            0.08f
        );


        PlaySound(
            clinkingCoins,
            coinVolume
        );
    }


    // =========================================================
    // JACKPOT SOUNDS
    // =========================================================

    private void PlayJackpotSounds()
    {
        PlaySound(
            casinoBellsReward,
            jackpotVolume
        );


        StartCoroutine(
            PlayJackpotRewardSounds()
        );
    }


    private IEnumerator PlayJackpotRewardSounds()
    {
        yield return new WaitForSeconds(
            0.25f
        );


        PlaySound(
            payoutAward,
            payoutVolume
        );


        yield return new WaitForSeconds(
            0.12f
        );


        PlaySound(
            clinkingCoins,
            coinVolume
        );
    }


    // =========================================================
    // SYMBOL POSITION MAPPING
    // =========================================================

    /*
        Symbol_1 = 7
        Symbol_2 = BAR
        Symbol_3 = CHERRY
        Symbol_4 = BELL
        Symbol_5 = 7
        Symbol_6 = BAR
        Symbol_7 = CHERRY
        Symbol_8 = BELL
        Symbol_9 = 7
    */

    private string GetSymbolType(int index)
    {
        switch (index)
        {
            // 7
            case 0:
            case 4:
            case 8:
                return "7";


            // BAR
            case 1:
            case 5:
                return "BAR";


            // CHERRY
            case 2:
            case 6:
                return "CHERRY";


            // BELL
            case 3:
            case 7:
                return "BELL";


            default:
                return "UNKNOWN";
        }
    }


    // =========================================================
    // SHOW RESULT
    // =========================================================

    private void ShowResult(string message)
    {
        Debug.Log(message);


        // =====================================================
        // CLEAR REWARD TEXT FIRST
        // =====================================================

        if (rewardText != null)
        {
            rewardText.text = "";
            rewardText.transform.localScale = Vector3.one;
        }


        if (resultText == null)
            return;


        // Stop previous result animation
        if (resultAnimationCoroutine != null)
        {
            StopCoroutine(resultAnimationCoroutine);
            resultAnimationCoroutine = null;
        }


        // Stop previous clear coroutine
        if (clearResultCoroutine != null)
        {
            StopCoroutine(clearResultCoroutine);
            clearResultCoroutine = null;
        }


        // Set message
        resultText.text = message;


        // Reset scale
        resultText.transform.localScale =
            Vector3.one;


        // Start animation
        if (animateResult)
        {
            resultAnimationCoroutine =
                StartCoroutine(
                    AnimateResult()
                );
        }
    }


    // =========================================================
    // RESULT POP ANIMATION
    // =========================================================

    private IEnumerator AnimateResult()
    {
        Transform resultTransform =
            resultText.transform;


        Vector3 normalScale =
            Vector3.one;


        Vector3 bigScale =
            Vector3.one * resultPopScale;


        // -----------------------------------------------------
        // POP UP
        // -----------------------------------------------------

        float time = 0f;


        while (time < 1f)
        {
            time +=
                Time.deltaTime *
                resultPopSpeed;


            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    time
                );


            resultTransform.localScale =
                Vector3.Lerp(
                    normalScale,
                    bigScale,
                    t
                );


            yield return null;
        }


        resultTransform.localScale =
            bigScale;


        // -----------------------------------------------------
        // HOLD
        // -----------------------------------------------------

        yield return new WaitForSeconds(
            resultHoldTime
        );


        // -----------------------------------------------------
        // RETURN TO NORMAL
        // -----------------------------------------------------

        time = 0f;


        while (time < 1f)
        {
            time +=
                Time.deltaTime *
                resultReturnSpeed;


            float t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    time
                );


            resultTransform.localScale =
                Vector3.Lerp(
                    bigScale,
                    normalScale,
                    t
                );


            yield return null;
        }


        resultTransform.localScale =
            normalScale;


        resultAnimationCoroutine = null;
    }


    // =========================================================
    // CLEAR RESULT IMMEDIATELY
    // =========================================================

    private void ClearResultImmediately()
    {
        if (clearResultCoroutine != null)
        {
            StopCoroutine(clearResultCoroutine);
            clearResultCoroutine = null;
        }


        if (resultAnimationCoroutine != null)
        {
            StopCoroutine(resultAnimationCoroutine);
            resultAnimationCoroutine = null;
        }


        if (resultText != null)
        {
            resultText.text = "";

            resultText.transform.localScale =
                Vector3.one;
        }


        // NEW - Clear reward text
        if (rewardText != null)
        {
            rewardText.text = "";

            rewardText.transform.localScale =
                Vector3.one;
        }
    }


    // =========================================================
    // CLEAR TEMPORARY MESSAGE
    // =========================================================

    private IEnumerator ClearResultAfterDelay(
        float delay
    )
    {
        yield return new WaitForSeconds(
            delay
        );


        if (
            !isSpinning &&
            resultText != null
        )
        {
            if (resultAnimationCoroutine != null)
            {
                StopCoroutine(
                    resultAnimationCoroutine
                );

                resultAnimationCoroutine = null;
            }


            resultText.text = "";


            resultText.transform.localScale =
                Vector3.one;
        }


        // NEW - Clear reward text
        if (rewardText != null)
        {
            rewardText.text = "";

            rewardText.transform.localScale =
                Vector3.one;
        }


        clearResultCoroutine = null;
    }
}