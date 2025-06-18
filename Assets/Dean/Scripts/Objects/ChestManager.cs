using UnityEngine.InputSystem;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public PuzzleChest chest;
    [SerializeField] private GameObject puzzleUI;
    [SerializeField] private InputPuzzle inputPuzzle;

    public void OpenPuzzle()
    {
        if (chest != null && chest.canInteract && chest.player.money >= chest.cost)
        {
            chest.player.money -= chest.cost;
            chest.player.enabled = false;
            chest.canInteract = false;
            puzzleUI.SetActive(true);
            inputPuzzle.GenerateSequence();
            inputPuzzle.OnPuzzleCompleted += ExitPuzzle;
            inputPuzzle.OnPuzzleCompleted += Reward;
        }
    }

    public void OnDirectionInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();

            if (input == Vector2.up) inputPuzzle.HandleInput("W");
            else if (input == Vector2.down) inputPuzzle.HandleInput("S");
            else if (input == Vector2.left) inputPuzzle.HandleInput("A");
            else if (input == Vector2.right) inputPuzzle.HandleInput("D");
        }
    }

    public void ExitPuzzle()
    {
        if (chest != null)
        {
            inputPuzzle.OnPuzzleCompleted -= ExitPuzzle;
            Debug.Log("Puzzle exit activated!");
            inputPuzzle.OnPuzzleCompleted -= Reward;
            Debug.Log("Puzzle reward activated!");
            chest.player.enabled = true;
            Debug.Log("Player enabled!");
            puzzleUI.SetActive(false);
            Debug.Log("Puzzle UI disabled!");
        }
    }

    public void Reward()
    {
        int reward = Random.Range(0, chest.possibleRewards.Count);
        Instantiate(chest.possibleRewards[reward], chest.transform.position + transform.up * 1f, chest.transform.rotation);
    }
}
