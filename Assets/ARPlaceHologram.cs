using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceHologram : MonoBehaviour
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

    private GameObject[,] _gamePositions;

    private bool placeMode;

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _gamePositions = new GameObject[8, 8];
        PlaceWhiteChessman();
        PlaceBlackChessman();
        placeMode = true;
    }

    private void PlaceBlackChessman()
    {
        //King:
        PlaceChessman(_chessmanPrefabsBlack[0], 4, 7, false);

        //Queen:
        PlaceChessman(_chessmanPrefabsBlack[1], 3, 7, false);

        //Rooks:
        PlaceChessman(_chessmanPrefabsBlack[2], 0, 7, false);
        PlaceChessman(_chessmanPrefabsBlack[2], 7, 7, false);

        //Bishops:
        PlaceChessman(_chessmanPrefabsBlack[3], 2, 7, false);
        PlaceChessman(_chessmanPrefabsBlack[3], 5, 7, false);

        //Knights:
        PlaceChessman(_chessmanPrefabsBlack[4], 1, 7, false);
        PlaceChessman(_chessmanPrefabsBlack[4], 6, 7, false);

        //pawns:
        for (int i = 0; i < 8; i++)
        {
            PlaceChessman(_chessmanPrefabsBlack[5], i, 6, false);
        }
             
    }

    private void PlaceWhiteChessman()
    {
        //King:
        PlaceChessman(_chessmanPrefabsWhite[0], 4, 0, true);

        //Queen:
        PlaceChessman(_chessmanPrefabsWhite[1], 3, 0, true);

        //Rooks:
        PlaceChessman(_chessmanPrefabsWhite[2], 0, 0, true);
        PlaceChessman(_chessmanPrefabsWhite[2], 7, 0, true);

        //Bishops:
        PlaceChessman(_chessmanPrefabsWhite[3], 2, 0, true);
        PlaceChessman(_chessmanPrefabsWhite[3], 5, 0, true);

        //Knights:
        PlaceChessman(_chessmanPrefabsWhite[4], 1, 0, true);
        PlaceChessman(_chessmanPrefabsWhite[4], 6, 0, true);

        //pawns:
        for(int i = 0; i < 8; i++)
        {
            PlaceChessman(_chessmanPrefabsWhite[5], i, 1, true);
        }

    }

    private void PlaceChessman(GameObject prefab, int x, int z, bool isWhite)
    {
        var rotation = isWhite ? 0 : 180;
        _gamePositions[x, z] = Instantiate(prefab, new Vector3(x * TILE_SIZE, 0, z * TILE_SIZE), Quaternion.Euler(0, rotation, 0), _boradGameObject.transform);
        _gamePositions[x, z].layer = LayerMask.GetMask("Chessman");
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
            RaycastHit touchHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out touchHit, 150.0f))
            {
                _gamePositions[0, 0].transform.position = touchHit.point;                
            };

            if (touch.phase == TouchPhase.Began && placeMode)
            {
                var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
                _raycastManager.Raycast(screenCenter, Hits, TrackableType.PlaneWithinBounds);
                CreateAnchor(Hits[0]);
                placeMode = false;
            }
        }
    }

    bool CreateAnchor(in ARRaycastHit hit)
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
}

