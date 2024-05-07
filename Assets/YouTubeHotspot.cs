using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using ARLocation;

public class VimeoHotspot : MonoBehaviour
{
    [Serializable]
    public class BillboardData
    {
        public int _id;
        public LocationData location;
        public float priceCharge;
        public string region;
    }

    [Serializable]
    public class LocationData
    {
        public double lat;
        public double lon;
        public double height;
    }

    public string vimeoUrl;
    public GameObject videoPlayerPrefab;

    private List<Hotspot> hotspots = new List<Hotspot>();

    private void Start()
    {
        // Fetch locations from the backend
        StartCoroutine(FetchLocationsFromBackend());
    }

    private IEnumerator FetchLocationsFromBackend()
    {
        string backendUrl = "http://horizon-420212.el.r.appspot.com/allbillboards";

        UnityWebRequest request = UnityWebRequest.Get(backendUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError($"Error fetching locations: {request.error}");
        }
        else
        {
            // Parse the response and create hotspots
            BillboardData[] billboardData = JsonUtility.FromJson<BillboardData[]>(request.downloadHandler.text);

            foreach (BillboardData data in billboardData)
            {
                ARLocation.Location location = new ARLocation.Location(data.location.lat, data.location.lon, data.location.height);
                Hotspot.HotspotSettingsData settings = new Hotspot.HotspotSettingsData
                {
                    Prefab = videoPlayerPrefab,
                    PositionMode = Hotspot.PositionModes.HotspotCenter,
                    ActivationRadius = 10f // Adjust the activation radius as needed
                };

                GameObject hotspotGO = Hotspot.CreateHotspotGameObject(location, settings, $"Vimeo Hotspot {data._id}");
                Hotspot hotspot = hotspotGO.GetComponent<Hotspot>();
                hotspot.OnHotspotActivated.AddListener(SpawnVideoPlayer);
                hotspots.Add(hotspot);
            }
        }
    }

    private void SpawnVideoPlayer(GameObject instance)
    {
        // Spawn the video player and set the Vimeo URL
        VideoPlayer videoPlayer = instance.AddComponent<VideoPlayer>();
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = vimeoUrl;
        videoPlayer.Prepare();
    }
}