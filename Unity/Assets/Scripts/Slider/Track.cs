using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Track : MonoBehaviour
{
    public Slider slider;
    public Button button;
    public Sprite playSprite;
    public Sprite pauseSprite;
    private bool play;

    // Start is called before the first frame update
    void Start()
    {
        play = false;
        button.onClick.AddListener(TaskOnClick);
    }
    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        play = !play;

    }
    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            slider.value += 0.1f;
            button.image.sprite = pauseSprite;
        } else
        {
            button.image.sprite = playSprite;
        }

        if (slider.value >= slider.maxValue)
        {
            button.image.sprite = playSprite;
            play = false;
        }
    }
}