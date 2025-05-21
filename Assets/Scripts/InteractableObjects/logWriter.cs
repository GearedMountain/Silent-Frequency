using UnityEngine;

public class logWriter : MonoBehaviour
{
    public GameObject animatedLogSheet;

    private Animator logSheetAnimator;
    
    public GameObject selectionHighlight;
    public GameObject holePrefab;

    public float xStart = -0.0565f;
    public float xStep = 0.1f;
    public float yStart = .34f;
    public float yStep = -0.12f;

    public Vector3[,] gridPositions = new Vector3[5, 5];
    private bool[,] punched = new bool[5, 5];

    public int currentRow = 0;
    public int currentCol = 0;
    public void Start()
    {
        logSheetAnimator = animatedLogSheet.GetComponent<Animator>();
        GenerateGridPositions();
        MoveHighlightToCurrent();
    }
    
    public void Interact(){
        logSheetAnimator.SetTrigger ("NewCardGrabbed");
        Debug.Log("New Log Created");
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        HandlePunchInput();
    }

     void GenerateGridPositions()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                // Skip top-right if needed
                if (row == 0 && col == 4)
                    continue;

                float x = xStart + (col * xStep);
                float y = yStart + (row * yStep);
                gridPositions[row, col] = new Vector3(x, y, 0);
            }
        }
    }

     void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) currentRow = Mathf.Max(currentRow - 1, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) currentRow = Mathf.Min(currentRow + 1, 4);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) currentCol = Mathf.Max(currentCol - 1, 0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) currentCol = Mathf.Min(currentCol + 1, 4);

        // Skip (0,4) if navigating
        if (currentRow == 0 && currentCol == 4)
        {
            currentCol = 3; // or move elsewhere
        }

        MoveHighlightToCurrent();
    }

    void HandlePunchInput()
    {
        if (Input.GetMouseButtonDown(0) && !punched[currentRow, currentCol])
        {
            punched[currentRow, currentCol] = true;
            Vector3 position = gridPositions[currentRow, currentCol];
            GameObject hole = Instantiate(holePrefab, Vector3.zero, Quaternion.identity, animatedLogSheet.transform);
            hole.transform.localPosition = gridPositions[currentRow, currentCol];
        }
    }

    void MoveHighlightToCurrent()
    {
        selectionHighlight.transform.localPosition  = gridPositions[currentRow, currentCol];
    }
}
