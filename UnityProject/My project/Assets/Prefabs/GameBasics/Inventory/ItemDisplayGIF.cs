using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayGIF : MonoBehaviour
{

    public Item item;
    private Image iconImage;
    private int currentFrame;
    private float timer;

    // Start is called before the first frame update
    private void Start()
    {
        iconImage = GetComponent<Image>();

        //start with static icon if frames are not provided
        if (item.animatedIconFrames == null || item.animatedIconFrames.Length == 0)
        {
            iconImage.sprite = item.itemIcon;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //animate only if animatedIconFrames is set
        if(item.animatedIconFrames != null && item.animatedIconFrames.Length > 0)
        {
            timer += Time.deltaTime;

            float frameRate = item.animationFrameRate > 0 ? item.animationFrameRate : 5f;//default frame rate

            if (timer >= 1f / item.animationFrameRate)
            {
                //update icon to next
                currentFrame = (currentFrame + 1) % item.animatedIconFrames.Length;
                //iconImage.sprite = item.animatedIconFrames[currentFrame];
                timer = 0f;
            }
        }
    }
}
