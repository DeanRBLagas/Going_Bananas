using UnityEngine.InputSystem;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public PuzzleChest chest;
    [SerializeField] private GameObject puzzleUI;
    [SerializeField] private InputPuzzle inputPuzzle;

    public void OpenPuzzle()
    {
        if (chest != null && chest.canInteract)
        {
            chest.player.enabled = false;
            puzzleUI.SetActive(true);
            inputPuzzle.GenerateSequence();
            inputPuzzle.OnPuzzleCompleted += ExitPuzzle;
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
        inputPuzzle.OnPuzzleCompleted -= ExitPuzzle;
        chest.player.enabled = true;
        puzzleUI.SetActive(false);
    }
}
