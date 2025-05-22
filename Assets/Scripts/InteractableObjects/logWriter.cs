using UnityEngine;

public class logWriter : MonoBehaviour
{
    private bool hasCreatedLog = false;
    public GameObject animatedLogSheet;

    private Animator logSheetAnimator;
    private AudioSource holePuncherAudio;
    public GameObject selectionHighlight;
    public GameObject holePrefab;

    public holePunchShaderManager holePunchShaderManager;

    public float xStart = -0.0565f;
    public float xStep = 0.1f;
    public float yStart = .34f;
    public float yStep = -0.12f;

    public Vector3[,] gridPositions = new Vector3[5, 5];
    private bool[,] punched = new bool[5, 5];

    public int currentRow = 0;
    public int currentCol = 0;

    public GameObject submitLogGameobject;
    public GameObject trashLogGameobject;

    public void Start()
    {
        logSheetAnimator = animatedLogSheet.GetComponent<Animator>();
        holePuncherAudio = selectionHighlight.GetComponent<AudioSource>();
        GenerateGridPositions();
        MoveHighlightToCurrent();
    }
    
    public void Interact(){
        hasCreatedLog = true;
        logSheetAnimator.SetTrigger ("NewCardGrabbed");
        trashLogGameobject.SetActive(true);
        submitLogGameobject.SetActive(true);
        Debug.Log("New Log Created");
    }

    public void Reset(){
        punched = new bool[5, 5];
        for (int i = holePunchShaderManager.punchers.Count - 1; i > 0; i--){
            Transform hole = holePunchShaderManager.punchers[i];
            holePunchShaderManager.punchers.Remove(hole);
            Destroy(hole.gameObject);
        }
        trashLogGameobject.SetActive(false);
        submitLogGameobject.SetActive(false);
        hasCreatedLog = false;
        logSheetAnimator.SetTrigger ("SubmitCard");

    }

    // Update is called once per frame
    void Update()
    {
        // ONLY ALLOW THESE CLICKS IF YOU HAVE OPENED A NEW LOG
        if(hasCreatedLog){
            HandleMovementInput();
            HandlePunchInput();
        }
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
        if (Input.GetKeyDown(KeyCode.W)) currentRow = Mathf.Max(currentRow - 1, 0);
        if (Input.GetKeyDown(KeyCode.S)) currentRow = Mathf.Min(currentRow + 1, 4);
        if (Input.GetKeyDown(KeyCode.A)) currentCol = Mathf.Max(currentCol - 1, 0);
        if (Input.GetKeyDown(KeyCode.D)) currentCol = Mathf.Min(currentCol + 1, 4);

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
            holePunchShaderManager.punchers.Add(hole.transform);
            holePuncherAudio.Play();
        }
    }

    void MoveHighlightToCurrent()
    {
        selectionHighlight.transform.localPosition  = gridPositions[currentRow, currentCol];
    }
}
