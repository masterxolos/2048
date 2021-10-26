using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class game : MonoBehaviour
{
    public static int gridWidth = 4;
    public static int gridHeight = 4;
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];
    public enum Swipe { None, Up, Down, Left, Right };
    public float minSwipeLength = 200f;
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
    public static Swipe swipeDirection;

    public Canvas GameOverlayCanvas;
    public Text gameScoreText;
    public int score = 0;
    // Start is called before the first frame update
    void Start()
    {
        GenerateNewTile(2);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver() != true){
            CheckUserInput();
        } else {
            GameOverlayCanvas.gameObject.SetActive(true);
        }
    }
    
    void CheckUserInput () {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor){
            bool down = Input.GetKeyDown(KeyCode.DownArrow);
            bool up = Input.GetKeyDown(KeyCode.UpArrow);
            bool left = Input.GetKeyDown(KeyCode.LeftArrow);
            bool right = Input.GetKeyDown(KeyCode.RightArrow);

            if (down ||up || left || right){
                PrepareTilesForMerging();

                if (down) {
                    MoveAllTiles(Vector2.down);
                }
                if (up) {
                    MoveAllTiles(Vector2.up);
                }
                if (left) {
                    MoveAllTiles(Vector2.left);
                }
                if (right) {
                    MoveAllTiles(Vector2.right);
                }
            }
        }

        if (Application.platform == RuntimePlatform.Android){
            detectSwipe();
            bool down = (swipeDirection == Swipe.Down);
            bool up = (swipeDirection == Swipe.Up);
            bool left = (swipeDirection == Swipe.Left);
            bool right = (swipeDirection == Swipe.Right);

            if (down ||up || left || right){
                PrepareTilesForMerging();

                if (down) {
                    MoveAllTiles(Vector2.down);
                }
                if (up) {
                    MoveAllTiles(Vector2.up);
                }
                if (left) {
                    MoveAllTiles(Vector2.left);
                }
                if (right) {
                    MoveAllTiles(Vector2.right);
                }
            }
        } 
    }
    void detectSwipe(){
            if (Input.touches.Length > 0) {
             Touch t = Input.GetTouch(0);
    
                if (t.phase == TouchPhase.Began) {
                    firstPressPos = new Vector2(t.position.x, t.position.y);
                }
    
                if (t.phase == TouchPhase.Ended) {
                    secondPressPos = new Vector2(t.position.x, t.position.y);
                    currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            
                    // Make sure it was a legit swipe, not a tap
                    if (currentSwipe.magnitude < minSwipeLength) {
                        swipeDirection = Swipe.None;
                        return;
                    }
            
                    currentSwipe.Normalize();
    
                    // Swipe up
                    if (currentSwipe.y > 0  && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
                        swipeDirection = Swipe.Up;
                    // Swipe down
                    } else if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f) {
                        swipeDirection = Swipe.Down;
                    // Swipe left
                    } else if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                        swipeDirection = Swipe.Left;
                    // Swipe right
                    } else if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f) {
                        swipeDirection = Swipe.Right;
                    }
                }
            } else {
                swipeDirection = Swipe.None;
            }
    }
    void updateScore(){
        gameScoreText.text = score.ToString("00000000");
    }

    void MoveAllTiles (Vector2 direction) {
        int MovedTiles = 0;
        if (direction == Vector2.left) {
            for (int x = 0; x <gridWidth; x++) {
                for(int y = 0; y< gridHeight; y++) {
                    if (grid[x,y] != null) {
                        if (MoveTile(grid[x, y], direction)) {
                            MovedTiles++;
                        }
                    }
                }
            }
        }

        if (direction == Vector2.right) {
            for (int x = gridWidth - 1; x >= 0; x--) {
                for (int y = 0; y < gridHeight; y++){
                    if (grid[x, y] != null) {
                        if (MoveTile(grid[x, y], direction)) {
                            MovedTiles++;
                        }
                    }
                }
            }
        }
            
        if (direction == Vector2.down){
            for (int x = 0; x < gridWidth; x++){
                for(int y = 0; y < gridHeight; y++){
                    if (grid[x,y] != null){
                        if (MoveTile(grid[x, y], direction)){
                            MovedTiles++;
                        }
                    }
                }
            }
        }

        if  (direction == Vector2.up) {
            for (int x = 0; x< gridWidth; x++) {
                for (int y = gridHeight -1; y >= 0; y--){
                    if (grid[x, y] != null) {
                        if (MoveTile(grid[x, y], direction)){
                            MovedTiles++;
                        }
                    }
                }
            }
        }

        if (MovedTiles != 0){
            GenerateNewTile(1);
        }
    }
    bool isGameOver(){
        if (transform.childCount < gridWidth * gridHeight){
            return false;
        }
        for(int x = 0; x < gridWidth; x++){
            for (int y = 0; y < gridHeight; y++){
                Transform currentTile = grid[x, y];
                Transform tileDown = null;
                Transform tileUp = null;

                if( y != 0){
                    tileDown = grid[x, y -1];
                }
                if (x != gridWidth -1){
                    tileUp = grid[x + 1, y];
                }
                if (tileUp != null) {
                    if (currentTile.GetComponent<block>().tileValue == tileUp.GetComponent<block>().tileValue){
                        return false;
                    }
                }
                if (tileDown != null){
                    if (currentTile.GetComponent<block>().tileValue == tileDown.GetComponent<block>().tileValue){
                        return false;
                    }
                }
            }
        }
        return true;
        Debug.Log("game is over");
    }

    bool MoveTile (Transform tile, Vector2 direction) {
        Vector2 startingPosition = tile.localPosition;

        while (true) {
            tile.transform.localPosition += (Vector3)direction;

            Vector2 position = tile.transform.localPosition;
            
            if (CheckIsInsideGrid(position)){
                if(CheckIfItsAtValidPosition (position)) {
                    UpdateGrid();
                }
                else {
                    if (!CombineTiles(tile)){
                        tile.transform.localPosition += -(Vector3)direction;

                        if (tile.transform.localPosition == (Vector3)startingPosition) {
                            return false;
                        }
                        else {
                            return true;
                        }
                    }
                }
            } 

            else {
                tile.transform.localPosition += -(Vector3)direction;

                if (tile.transform.localPosition == (Vector3)startingPosition) {
                    return false;
                }
                else {
                    return true;
                }
            }
            
        }
    }

    bool CombineTiles (Transform MovingTile) {
        Vector2 position = MovingTile.transform.localPosition;
        Transform collidingTile = grid[(int)position.x, (int)position.y];

        int movingTileValue = MovingTile.GetComponent<block>().tileValue;
        int collidingTileValue = collidingTile.GetComponent<block>().tileValue;

        if(movingTileValue == collidingTileValue && !MovingTile.GetComponent<block>().mergedThisTurn && !collidingTile.GetComponent<block>().mergedThisTurn){
            Destroy(MovingTile.gameObject);
            Destroy(collidingTile.gameObject);

            grid[(int)position.x, (int)position.y] = null;

            string NewTileName = (movingTileValue * 2).ToString();

            GameObject newTile = (GameObject)Instantiate(Resources.Load(NewTileName, typeof(GameObject)), position, Quaternion.identity);

            newTile.transform.parent = transform;

            newTile.GetComponent<block>().mergedThisTurn = true;
            UpdateGrid();
            score += movingTileValue * 2;
            updateScore();
            return true;

        }
        return false;
    }

    void GenerateNewTile(int amount){
        for(int i = 0; i< amount; i++){
            Vector2 newTileLocation = GetRandomLocationForNewTile();

            string tile = "2";

            float chanceOf2 = Random.Range(0f,1f);

            if (chanceOf2 > 0.9f){
                tile = "4";
            }

            GameObject newTile = (GameObject)Instantiate(Resources.Load(tile, typeof(GameObject)), newTileLocation, Quaternion.identity);
            newTile.transform.parent = transform;
        }
        UpdateGrid();
    }

    void UpdateGrid() {
        for(int y = 0; y< gridHeight; ++y){
            for(int x = 0; x<gridWidth; ++x){
                if (grid[x,y] != null) {
                    if (grid[x,y].parent == transform){
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform tile in transform) {
            Vector2 v = new Vector2 (Mathf.Round(tile.position.x), Mathf.Round(tile.position.y));
            grid[(int)v.x, (int)v.y] = tile;
        }
    }

    Vector2 GetRandomLocationForNewTile (){
        List<int> x = new List<int>();
        List<int> y = new List<int>();
        
        for (int i = 0; i < gridWidth; i++){
            for(int j = 0; j < gridHeight; j++){
                if (grid[i,j] == null){
                    x.Add(i);
                    y.Add(j); 
                }
            }
        }
        int randomIndex = Random.Range(0, x.Count);
        int randX = x.ElementAt(randomIndex);
        int randY = y.ElementAt(randomIndex);
        Debug.Log("New Random Tile Location"+ randX + ", " + randY);
        return new Vector2(randX, randY);
        
    }

    bool CheckIsInsideGrid(Vector2 position) {
        if (position.x >= 0 && position.x <= gridWidth -1 && position.y >= 0 && position.y <= gridHeight -1){
            return true;
        }
        return false;
    }
    bool CheckIfItsAtValidPosition (Vector2 position) {
        if (grid[(int)position.x, (int)position.y] == null) {
            return true;
        }
        return false;
    }

    void PrepareTilesForMerging() {
        foreach (Transform t in transform) {
            t.GetComponent<block>().mergedThisTurn = false;
        }
    }
    
    public void PlayAgain(){
        grid = new Transform[gridWidth, gridHeight]; 
        score = 0;

        List<GameObject> children = new List<GameObject>();

        foreach (Transform t in transform){
            children.Add(t.gameObject);
        }

        children.ForEach(t => DestroyImmediate(t));

        GameOverlayCanvas.gameObject.SetActive(false);
        updateScore();
        GenerateNewTile(2);
    }
}

