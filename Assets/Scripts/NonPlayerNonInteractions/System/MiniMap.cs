using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control the minimap
/// </summary>
public class MiniMap : MonoBehaviour
{
    public Vector2 realMapSize; // The size of the real map
    public Vector2 realMapOffset; // The offset of the real map from 0,0 in unity
    public Vector2 customSize; // Do we use a custom size for the minimap or use the size of the image
    public RectTransform playerIcon; // The image that represents player on the minimap
    public RectTransform minimapImage; // The minimap image
    public RectTransform minimapOnHUD; // The minimap area (mask) on the HUD
    public float zoomRatio; // How much the map should zoom in according to the player's speed (zoom in when slower, out when faster)
    public Vector2 realMapDirections; // The x and z directions of the real map

    public Vector2 actualMinimapSize; // The size of the actual minimap image RectTransform
    //public Vector2 minimapSizeOnHUD; // The size of the minimap area on the HUD
    public float minimapSizeToViewAreaRatio; // The ratio of the size of the minimap size to the minimap area on HUD

    // Use this for initialization
    void Start()
    {
        // Match the rect size of the RectTransform of the minimap to the size of the minimap image
        //minimapImage.GetComponent<Image>().SetNativeSize();

        // If there is no custom size of the minimap defined, then use the minimap image's size
        if (customSize == Vector2.zero)
        {
            actualMinimapSize = minimapImage.rect.size;
        }
        else
        {
            actualMinimapSize = customSize;
        }

        // Decide whether we should calculate the "imageToAreaRatio" with the length or the width of the minimap image
        if (minimapImage.GetComponent<Image>().mainTexture.width / minimapImage.GetComponent<Image>().mainTexture.height >=
            minimapOnHUD.rect.width / minimapOnHUD.rect.height)
        {
            minimapSizeToViewAreaRatio = actualMinimapSize.x / minimapOnHUD.rect.width;
            actualMinimapSize.y = actualMinimapSize.x * minimapImage.GetComponent<Image>().mainTexture.height /
                                                        minimapImage.GetComponent<Image>().mainTexture.width;
        }
        else
        {
            minimapSizeToViewAreaRatio = actualMinimapSize.y / minimapOnHUD.rect.height;
            actualMinimapSize.x = actualMinimapSize.y * minimapImage.GetComponent<Image>().mainTexture.width /
                                                        minimapImage.GetComponent<Image>().mainTexture.height;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMinimapPosition();
        UpdatePlayerIconRotation();
    }

    /// <summary>
    /// Update the scale of the minimap according to player's current speed
    /// </summary>
    public void UpdateMinimapScale()
    {

    }

    /// <summary>
    /// Update the position of the minimap on the HUD minimap area
    /// </summary>
    public void UpdateMinimapPosition()
    {
        Vector2 newPlayerIconPosition;
        newPlayerIconPosition.x = (GameManager.gameManager.playerKart.transform.position.x -
                                   realMapOffset.x -
                                   (realMapSize.x * 0.5f * realMapDirections.x)) * 
                                  actualMinimapSize.x / realMapSize.x * realMapDirections.x * -1;
        newPlayerIconPosition.y = (GameManager.gameManager.playerKart.transform.position.z -
                                   realMapOffset.y -
                                   (realMapSize.y * 0.5f * realMapDirections.y)) * 
                                  actualMinimapSize.y / realMapSize.y * realMapDirections.y;

        playerIcon.localPosition = newPlayerIconPosition;
    }

    /// <summary>
    /// Update the rotation of the player's icon on the minimap
    /// </summary>
    public void UpdatePlayerIconRotation()
    {
        playerIcon.localEulerAngles = new Vector3(180, 0, GameManager.gameManager.playerKart.transform.eulerAngles.y);
    }

    /// <summary>
    /// Update the position of the player's icon on the minimap
    /// </summary>
    public void UpdatePlayerIconPosition()
    {

    }
}
