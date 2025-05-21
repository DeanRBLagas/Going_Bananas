using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class InputPuzzle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sequenceText;
    private List<string> targetSequence = new List<string>();
    private int currentIndex = 0;

    private bool canInput = true;

    public delegate void PuzzleCompletedHandler();
    public event PuzzleCompletedHandler OnPuzzleCompleted;

    private string[] directions = { "W", "A", "S", "D" };

    private Dictionary<string, string> arrowMap = new Dictionary<string, string>()
    {
        { "W", "↑" },
        { "A", "←" },
        { "S", "↓" },
        { "D", "→" }
    };

    public void GenerateSequence(int length = 8)
    {
        targetSequence.Clear();
        currentIndex = 0;
        sequenceText.text = "";

        for (int i = 0; i < length; i++)
        {
            targetSequence.Add(directions[Random.Range(0, directions.Length)]);
        }

        UpdateSequenceText();
    }

    public void HandleInput(string input)
    {
        if (canInput)
        {
            if (currentIndex == targetSequence.Count) return;

            if (input == targetSequence[currentIndex])
            {
                currentIndex++;
                UpdateSequenceText();

                if (currentIndex == targetSequence.Count)
                {
                    OnPuzzleCompleted?.Invoke();
                }
            }
            else
            {
                currentIndex = 0;
                UpdateSequenceText(showError: true);
                StartCoroutine(ResetInput());
            }
        }
    }

    private void UpdateSequenceText(bool showError = false)
    {
        sequenceText.text = "";

        for (int i = 0; i < targetSequence.Count; i++)
        {
            string arrow = arrowMap[targetSequence[i]];

            if (showError)
            {
                sequenceText.text += $"<color=red>{arrow}</color> ";
            }
            else if (i < currentIndex)
            {
                sequenceText.text += $"<color=green>{arrow}</color> ";
            }
            else
            {
                sequenceText.text += $"<color=black>{arrow}</color> ";
            }
        }
    }

    private IEnumerator ResetInput()
    {
        canInput = false;
        yield return new WaitForSeconds(1f);
        UpdateSequenceText();
        canInput = true;
    }
}
