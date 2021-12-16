using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Chessman;
using Utils;

public class BoardManager : MonoBehaviour
{
    private const float TILE_SIZE = 0.05f;

    // The prefab to instantiate on touch.
    [SerializeField]
    private GameObject _boradGameObject;

    [SerializeField]
    private List<GameObject> _chessmanPrefabsWhite;

    [SerializeField]
    private List<GameObject> _chessmanPrefabsBlack;

    // Cache ARRaycastManager GameObject from ARCoreSession
    private ARRaycastManager _raycastManager;

     // Cache ARAnchorManager GameObject from ARCoreSession
    private ARAnchorManager _anchorManager;

    // List for raycast hits is re-used by raycast manager
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    private Chessman.Chessman[,] _gamePositions;

    private bool placeMode;

    private (int x, int y)? _selectedPosition;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _gamePositions = new Chessman.Chessman[8, 8];
        PlaceWhiteChessman();
        PlaceBlackChessman();
        placeMode = true;
    }

    private void PlaceBlackChessman()
    {
        //King:
        PlaceChessman(new King() { Color = ChessmanColor.Black}, _chessmanPrefabsBlack[0], 4, 7);

        //Queen:
        PlaceChessman(new Queen() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[1], 3, 7);

        //Rooks:
        PlaceChessman(new Rook() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[2], 0, 7);
        PlaceChessman(new Rook() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[2], 7, 7);

        //Bishops:
        PlaceChessman(new Bishop() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[3], 2, 7);
        PlaceChessman(new Bishop() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[3], 5, 7);

        //Knights:
        PlaceChessman(new Knight() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[4], 1, 7);
        PlaceChessman(new Knight() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[4], 6, 7);

        //pawns:
        for (int i = 0; i < 8; i++)
        {
            PlaceChessman(new Pawn() { Color = ChessmanColor.Black }, _chessmanPrefabsBlack[5], i, 6);
        }
             
    }

    private void PlaceWhiteChessman()
    {
        //King:
        PlaceChessman(new King() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[0], 4, 0);

        //Queen:
        PlaceChessman(new Queen() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[1], 3, 0);

        //Rooks:
        PlaceChessman(new Rook() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[2], 0, 0);
        PlaceChessman(new Rook() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[2], 7, 0);

        //Bishops:
        PlaceChessman(new Bishop() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[3], 2, 0);
        PlaceChessman(new Bishop() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[3], 5, 0);

        //Knights:
        PlaceChessman(new Knight() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[4], 1, 0);
        PlaceChessman(new Knight() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[4], 6, 0);

        //pawns:
        for(int i = 0; i < 8; i++)
        {
            PlaceChessman(new Pawn() { Color = ChessmanColor.White }, _chessmanPrefabsWhite[5], i, 1);
        }

    }

    private void PlaceChessman(Chessman.Chessman chessman, GameObject prefab, int x, int y)
    {
        var rotation = chessman.Color == ChessmanColor.White ? 0 : 180;
        chessman.GameObject = Instantiate(prefab, new Vector3(x * TILE_SIZE, 0, y * TILE_SIZE), Quaternion.Euler(0, rotation, 0), _boradGameObject.transform);
        _gamePositions[x, y] = chessman;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount < 1)
        {
            if (placeMode)
            {
                var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                _raycastManager.Raycast(screenCenter, Hits, TrackableType.PlaneWithinBounds);
                ARRaycastHit? hit = Hits[0];
                _boradGameObject.transform.position = hit.Value.pose.position;
                _boradGameObject.transform.rotation = hit.Value.pose.rotation;
            }
            return; 
        }
        else
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && placeMode)
            {
                var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                _raycastManager.Raycast(screenCenter, Hits, TrackableType.PlaneWithinBounds);
                CreateAnchor(Hits[0]);
                placeMode = false;
            }

            HandleChessboardTouch(touch);
            
        }
    }
    
    internal (int x, int y) GetBoardPoint(Vector3 touchPoint)
    {        
        Vector3 relativePoint = _boradGameObject.transform.InverseTransformPoint(touchPoint);
        return ((int)(relativePoint.x +0.5), (int)(relativePoint.z +0.5));
    }

    internal bool CreateAnchor(in ARRaycastHit hit)
    {
        if (hit.trackable is ARPlane plane)
        {
            var planeManager = GetComponent<ARPlaneManager>();
            if (planeManager)
            {
                ARAnchor anchor = _anchorManager.AttachAnchor(plane, hit.pose);
                _boradGameObject.transform.SetParent(anchor.transform);

                Debug.Log($"Created anchor attachment for plane (id: {anchor.nativePtr}).");
                return true;
            }
        }
        return false;
    }

    internal void HandleChessboardTouch(Touch touch)
    {
        RaycastHit touchHit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out touchHit, 150.0f))
        {
            if (touch.phase == TouchPhase.Began)
            {
                _selectedPosition = GetBoardPoint(touchHit.point);
            }
            else if (touch.phase == TouchPhase.Moved && _selectedPosition != null)
            {
                _gamePositions[(int)_selectedPosition?.x, (int)_selectedPosition?.y].GameObject.transform.position = touchHit.point;
            }
            else if (touch.phase == TouchPhase.Ended && _selectedPosition != null)
            {
                var endPoint = GetBoardPoint(touchHit.point);
                _gamePositions[endPoint.x, endPoint.y] = _gamePositions[(int)_selectedPosition?.x, (int)_selectedPosition?.y];
                _gamePositions[endPoint.x, endPoint.y].GameObject.transform.localPosition = new Vector3(endPoint.x, 0, endPoint.y);
                _gamePositions[(int)_selectedPosition?.x, (int)_selectedPosition?.y] = null;
                _selectedPosition = null;
            }
        }
    }
   
    internal void ShowAvailableMoves() { }
    internal void MoveChessman() { }
    internal bool IsMovePossible() { return false; }
    internal void CaptureChessman() { }
    internal bool IsCapturePossible() { return false; }
}

