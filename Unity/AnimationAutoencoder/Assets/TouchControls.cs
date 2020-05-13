using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchControls : MonoBehaviour
{
    private float width;
    private float height;

    // stuff to record touches
    List<Vector2> touches = new List<Vector2>();
    int ti = 0; 
    int indexDirection = 1;
    bool animate = false;
    int animateFPS = 30;
    float animateTime;


    [Header("UI Elements")]
    public Text status;
    public GameObject tracer; 

    void Awake()
    {
        width = (float)Screen.width / 4.0f; // -2/+2
        height = (float)Screen.height / 4.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        status.text = "Swipe to define a sequence  \nTap to define a pose";
    }

    void RecordTouch()
    {
        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                touches = new List<Vector2>();
                animate = false;
                status.text = "Swipe to define a sequence  \nTap to define a pose";
                Vector2 pos = touch.position;
                pos.x = (pos.x - width) / width;
                pos.y = (pos.y - height) / height;
                //touches.Add(pos);
                GetComponent<AnimationDecoder>().inputs = new float[] { pos.x, pos.y };
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 pos = touch.position;
                pos.x = (pos.x - width) / width;
                pos.y = (pos.y - height) / height;
                touches.Add(pos);
                status.text = "Sequencing...";
                animate = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                ti = 0;
            }
        }
    }

    void Animation()
    {
        if (animate)
        {
            GetComponent<AnimationDecoder>().inputs = new float[] {touches[ti].x, touches[ti].y};
            status.text = "Animating...";

            // wtf always gives a null reference exception.....
/*            tracer.transform.position = new Vector3(
                touches[ti].x,
                touches[ti].y,
                tracer.transform.position.z
            );*/

            ti += indexDirection;
            if (ti == touches.Count)
            {
                indexDirection = -1;
                ti -= 1;
            }
            else if (ti <= 0)
            {
                indexDirection = 1;
                ti += 1;
            }
        }
    }

    void Update()
    {
        if(Time.time > animateTime)
        {
            RecordTouch();
            Animation();
            animateTime = Time.time + 1f/animateFPS;
        }
    }
}
