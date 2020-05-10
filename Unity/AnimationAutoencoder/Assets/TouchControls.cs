using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
    
    List<Vector2> touches = new List<Vector2>();
    int ti = 0; 
    int indexDirection = 1;
    private float width;
    private float height;
    bool animate = false;

    Text StatusText;

    void Awake()
    {
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Move the cube if the screen has the finger moving.
            if (touch.phase == TouchPhase.Moved)
            {
                animate = false;
                Vector2 pos = touch.position;

                // add UI text
                pos.x = (pos.x - width) / width;
                pos.y = (pos.y - height) / height;

                StatusText.text = pos.ToString();
                touches.Add(pos);

            }
            else if(touch.phase == TouchPhase.Ended)
            {
                animate = true;
                ti = 0;
            }
        }

        // animate speed/record speed
        if (animate)
        {
            // GetComponent<Decoder>().inputs = touches[ti];
            ti += indexDirection;
            if(ti==touches.Length)
            {
                indexDirection = -1;
                ti -= 1;
            }
            else if(ti ==0)
            {
                indexDirection = 1;
                ti += 1;
            }
        }

    }
}
