using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotReel : MonoBehaviour
{
    [Header("Symbols")]
    public Image[] symbols;

    [Header("Spin Settings")]
    [Tooltip("Maximum spinning speed.")]
    public float maxSpinSpeed = 1800f;

    [Tooltip("Total time this reel spins.")]
    public float spinTime = 2.2f;

    [Tooltip("Distance between each symbol.")]
    public float symbolSpacing = 100f;

    [Tooltip("Time used to accelerate.")]
    public float accelerationTime = 0.25f;

    [Tooltip("Time used to slow down.")]
    public float decelerationTime = 0.75f;

    [Tooltip("Time used to settle on final result.")]
    public float settleTime = 0.15f;

    [Header("Symbol Probability")]
    [Range(0f, 100f)]
    public float sevenChance = 10f;

    [Range(0f, 100f)]
    public float barChance = 20f;

    [Range(0f, 100f)]
    public float cherryChance = 30f;

    [Range(0f, 100f)]
    public float bellChance = 40f;

    private RectTransform[] symbolRects;

    private bool isSpinning = false;

    private int currentSymbolIndex;

    public bool IsSpinning => isSpinning;

    public int CurrentSymbolIndex => currentSymbolIndex;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (symbols == null || symbols.Length != 9)
        {
            Debug.LogError(
                gameObject.name +
                " must have exactly 9 symbols assigned."
            );

            return;
        }

        symbolRects =
            new RectTransform[symbols.Length];

        for (int i = 0; i < symbols.Length; i++)
        {
            if (symbols[i] != null)
            {
                symbolRects[i] =
                    symbols[i].rectTransform;
            }
        }

        // Automatically detect symbol spacing
        if (
            symbols.Length >= 2 &&
            symbols[0] != null &&
            symbols[1] != null
        )
        {
            float distance =
                Mathf.Abs(
                    symbols[0]
                        .rectTransform
                        .localPosition.y
                    -
                    symbols[1]
                        .rectTransform
                        .localPosition.y
                );

            if (distance > 1f)
            {
                symbolSpacing = distance;
            }
        }
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        if (
            symbols == null ||
            symbols.Length != 9
        )
        {
            return;
        }

        currentSymbolIndex =
            GetWeightedResult();

        ArrangeSymbols(
            currentSymbolIndex
        );
    }


    // ============================================================
    // SPIN
    // ============================================================

    public IEnumerator Spin()
    {
        if (isSpinning)
            yield break;

        if (
            symbols == null ||
            symbols.Length != 9
        )
        {
            yield break;
        }

        isSpinning = true;


        // --------------------------------------------------------
        // Choose final result BEFORE spinning
        // --------------------------------------------------------

        int finalResult =
            GetWeightedResult();


        // --------------------------------------------------------
        // Make sure spin time is valid
        // --------------------------------------------------------

        float totalSpinTime =
            Mathf.Max(
                spinTime,
                accelerationTime +
                decelerationTime +
                0.4f
            );


        float cruiseTime =
            totalSpinTime
            -
            accelerationTime
            -
            decelerationTime;


        // --------------------------------------------------------
        // Spin timer
        // --------------------------------------------------------

        float timer = 0f;


        while (timer < totalSpinTime)
        {
            timer +=
                Time.deltaTime;

            float speed;


            // ====================================================
            // ACCELERATION
            // ====================================================

            if (timer < accelerationTime)
            {
                float t =
                    timer /
                    accelerationTime;

                t =
                    Mathf.Clamp01(t);

                t =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        t
                    );

                speed =
                    Mathf.Lerp(
                        0f,
                        maxSpinSpeed,
                        t
                    );
            }


            // ====================================================
            // FAST SPIN
            // ====================================================

            else if (
                timer <
                accelerationTime +
                cruiseTime
            )
            {
                speed =
                    maxSpinSpeed;
            }


            // ====================================================
            // DECELERATION
            // ====================================================

            else
            {
                float decelerationTimer =
                    timer
                    -
                    accelerationTime
                    -
                    cruiseTime;

                float t =
                    decelerationTimer /
                    decelerationTime;

                t =
                    Mathf.Clamp01(t);

                t =
                    Mathf.SmoothStep(
                        0f,
                        1f,
                        t
                    );

                speed =
                    Mathf.Lerp(
                        maxSpinSpeed,
                        0f,
                        t
                    );
            }


            // ====================================================
            // MOVE REEL
            // ====================================================

            MoveSymbols(
                speed *
                Time.deltaTime
            );

            WrapSymbols();

            yield return null;
        }


        // ========================================================
        // SET FINAL RESULT
        // ========================================================

        currentSymbolIndex =
            finalResult;


        // ========================================================
        // SMOOTHLY SETTLE
        // ========================================================

        yield return StartCoroutine(
            SmoothArrangeSymbols(
                finalResult,
                settleTime
            )
        );


        isSpinning = false;
    }


    // ============================================================
    // MOVE SYMBOLS
    // ============================================================

    private void MoveSymbols(
        float movement
    )
    {
        if (symbolRects == null)
            return;

        for (
            int i = 0;
            i < symbolRects.Length;
            i++
        )
        {
            if (symbolRects[i] == null)
                continue;

            Vector3 position =
                symbolRects[i]
                    .localPosition;

            // Reel moves downward
            position.y -= movement;

            symbolRects[i]
                .localPosition =
                position;
        }
    }


    // ============================================================
    // WRAP SYMBOLS
    // ============================================================

    private void WrapSymbols()
    {
        if (symbolRects == null)
            return;

        float totalHeight =
            symbolSpacing *
            symbols.Length;

        float bottomLimit =
            -symbolSpacing * 4.5f;


        for (
            int i = 0;
            i < symbolRects.Length;
            i++
        )
        {
            if (symbolRects[i] == null)
                continue;

            Vector3 position =
                symbolRects[i]
                    .localPosition;

            if (
                position.y <
                bottomLimit
            )
            {
                position.y +=
                    totalHeight;

                symbolRects[i]
                    .localPosition =
                    position;
            }
        }
    }


    // ============================================================
    // PROBABILITY SYSTEM
    // ============================================================

    private int GetWeightedResult()
    {
        float total =
            Mathf.Max(
                0f,
                sevenChance
            )
            +
            Mathf.Max(
                0f,
                barChance
            )
            +
            Mathf.Max(
                0f,
                cherryChance
            )
            +
            Mathf.Max(
                0f,
                bellChance
            );


        if (total <= 0f)
        {
            Debug.LogWarning(
                gameObject.name +
                ": All probabilities are 0. " +
                "Using Bell."
            );

            return 3;
        }


        float randomValue =
            Random.Range(
                0f,
                total
            );


        // ========================================================
        // 7
        //
        // S1 = 7
        // S5 = 7
        // S9 = 7
        // ========================================================

        if (randomValue < sevenChance)
        {
            int[] sevenPositions =
            {
                0,
                4,
                8
            };

            return sevenPositions[
                Random.Range(
                    0,
                    sevenPositions.Length
                )
            ];
        }

        randomValue -=
            sevenChance;


        // ========================================================
        // BAR
        //
        // S2 = BAR
        // S6 = BAR
        // ========================================================

        if (randomValue < barChance)
        {
            int[] barPositions =
            {
                1,
                5
            };

            return barPositions[
                Random.Range(
                    0,
                    barPositions.Length
                )
            ];
        }

        randomValue -=
            barChance;


        // ========================================================
        // CHERRY
        //
        // S3 = CHERRY
        // S7 = CHERRY
        // ========================================================

        if (randomValue < cherryChance)
        {
            int[] cherryPositions =
            {
                2,
                6
            };

            return cherryPositions[
                Random.Range(
                    0,
                    cherryPositions.Length
                )
            ];
        }


        // ========================================================
        // BELL
        //
        // S4 = BELL
        // S8 = BELL
        // ========================================================

        int[] bellPositions =
        {
            3,
            7
        };

        return bellPositions[
            Random.Range(
                0,
                bellPositions.Length
            )
        ];
    }


    // ============================================================
    // SMOOTH FINAL RESULT
    // ============================================================

    private IEnumerator SmoothArrangeSymbols(
        int selectedIndex,
        float duration
    )
    {
        if (
            symbols == null ||
            symbols.Length != 9
        )
        {
            yield break;
        }


        selectedIndex =
            Mathf.Clamp(
                selectedIndex,
                0,
                8
            );


        Vector3[] startPositions =
            new Vector3[
                symbols.Length
            ];

        Vector3[] targetPositions =
            new Vector3[
                symbols.Length
            ];


        // --------------------------------------------------------
        // Calculate target positions
        // --------------------------------------------------------

        for (
            int i = 0;
            i < symbols.Length;
            i++
        )
        {
            if (symbolRects[i] == null)
                continue;


            startPositions[i] =
                symbolRects[i]
                    .localPosition;


            int difference =
                i -
                selectedIndex;


            // Circular reel
            if (difference > 4)
            {
                difference -= 9;
            }

            if (difference < -4)
            {
                difference += 9;
            }


            targetPositions[i] =
                startPositions[i];


            targetPositions[i].y =
                -difference *
                symbolSpacing;
        }


        // --------------------------------------------------------
        // Smooth movement
        // --------------------------------------------------------

        float timer = 0f;


        while (timer < duration)
        {
            timer +=
                Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    timer /
                    duration
                );

            t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            for (
                int i = 0;
                i < symbols.Length;
                i++
            )
            {
                if (symbolRects[i] == null)
                    continue;

                symbolRects[i]
                    .localPosition =
                    Vector3.Lerp(
                        startPositions[i],
                        targetPositions[i],
                        t
                    );
            }


            yield return null;
        }


        // --------------------------------------------------------
        // Guarantee exact final position
        // --------------------------------------------------------

        for (
            int i = 0;
            i < symbols.Length;
            i++
        )
        {
            if (symbolRects[i] == null)
                continue;

            symbolRects[i]
                .localPosition =
                targetPositions[i];
        }
    }


    // ============================================================
    // ARRANGE SYMBOLS
    // ============================================================

    private void ArrangeSymbols(
        int selectedIndex
    )
    {
        if (
            symbols == null ||
            symbols.Length != 9
        )
        {
            return;
        }


        selectedIndex =
            Mathf.Clamp(
                selectedIndex,
                0,
                8
            );


        for (
            int i = 0;
            i < symbols.Length;
            i++
        )
        {
            if (symbolRects[i] == null)
                continue;


            int difference =
                i -
                selectedIndex;


            if (difference > 4)
            {
                difference -= 9;
            }

            if (difference < -4)
            {
                difference += 9;
            }


            Vector3 position =
                symbolRects[i]
                    .localPosition;


            position.y =
                -difference *
                symbolSpacing;


            symbolRects[i]
                .localPosition =
                position;
        }
    }


    // ============================================================
    // MANUAL RESULT
    // ============================================================

    public void SetResult(
        int index
    )
    {
        if (
            symbols == null ||
            symbols.Length != 9
        )
        {
            return;
        }


        currentSymbolIndex =
            Mathf.Clamp(
                index,
                0,
                8
            );


        ArrangeSymbols(
            currentSymbolIndex
        );
    }
}