using System.Collections.Generic;
using LearnXR.Core.Utilities;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Serialization;

public class MRUKDemo : MonoBehaviour
{
    [SerializeField] private MRUK mruk;
    [SerializeField] private OVRInput.Controller controller;
    [FormerlySerializedAs("objectForWallAnchors")] [SerializeField] private GameObject objectForWallAnchorsPrefab;

    private bool sceneHasBeenLoaded;
    private MRUKRoom currentRoom;
    private List<GameObject> wallAnchorObjectsCreated = new();

    private bool SceneAndRoomInfoAvailable => currentRoom != null && sceneHasBeenLoaded;
    
    private void OnEnable()
    {
        mruk.RoomCreatedEvent.AddListener(BindRoomInfo);
    }

    private void OnDisable()
    {
        mruk.RoomCreatedEvent.RemoveListener(BindRoomInfo);
    }

    public void EnableMRUKDemo()
    {
        sceneHasBeenLoaded = true;
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} has been enabled due to scene availability");
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller) && SceneAndRoomInfoAvailable)
        {
            if (wallAnchorObjectsCreated.Count == 0)
            {
                foreach (var wallAnchor in currentRoom.WallAnchors)
                {
                    var createdWallObject = Instantiate(objectForWallAnchorsPrefab, Vector3.zero, Quaternion.identity,
                        wallAnchor.transform);
                    createdWallObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    wallAnchorObjectsCreated.Add(createdWallObject);
                    SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} wall object created with Uuid: {wallAnchor.Anchor.Uuid}");
                }
                SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} wall objects added to all walls");
            }
            else
            {
                foreach (var wallObject in wallAnchorObjectsCreated)
                {
                    Destroy(wallObject);
                }
                wallAnchorObjectsCreated.Clear();
                SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} wall objects were deleted");
            }
        }
    }

    private void BindRoomInfo(MRUKRoom room)
    {
        currentRoom = room;
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} room was bound to current room");
    }
}
