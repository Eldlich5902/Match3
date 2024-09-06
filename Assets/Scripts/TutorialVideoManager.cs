using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;

    public TextMeshProUGUI tutorialShow; 
    public TextMeshProUGUI[] tutorialText;

    public Button playButton;
    public Button nextButton;
    public Button previousButton;

    private int currentVideoIndex = 0;
    private int currentTutorialIndex = 0;

    void Start()
    {
        // Gán sự kiện cho các nút
        //playButton.onClick.AddListener(PlayPauseVideo);
        nextButton.onClick.AddListener(PlayNextVideo);
        previousButton.onClick.AddListener(PlayPreviousVideo);

        // Bắt đầu với video đầu tiên
        if (videoClips.Length > 0)
        {
            videoPlayer.clip = videoClips[currentVideoIndex];
        }
        if (tutorialText.Length > 0)
        {
            tutorialShow.text = tutorialText[currentTutorialIndex].text;
        }
    }

    /*void PlayPauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }*/

    void PlayNextVideo()
    {
        currentVideoIndex = (currentVideoIndex + 1) % videoClips.Length;
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();

        currentTutorialIndex = (currentTutorialIndex + 1) % tutorialText.Length;
        tutorialShow.text = tutorialText[currentTutorialIndex].text;

    }

    void PlayPreviousVideo()
    {
        currentVideoIndex = (currentVideoIndex - 1 + videoClips.Length) % videoClips.Length;
        videoPlayer.clip = videoClips[currentVideoIndex];
        videoPlayer.Play();

        currentTutorialIndex = (currentTutorialIndex - 1 + tutorialText.Length) % tutorialText.Length;
        tutorialShow.text = tutorialText[currentTutorialIndex].text;

    }
}
