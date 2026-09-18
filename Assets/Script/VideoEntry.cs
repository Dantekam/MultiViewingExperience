using System;
using UnityEngine;
using UnityEngine.Video;

[Serializable]
public class VideoEntry
{
    [Header("Information")]
    public string title;

    [TextArea]
    public string description;

    [Header("Source")]
    public VideoSourceMode sourceMode = VideoSourceMode.LocalClip;

    public VideoClip localClip;

    public string url;

    [Tooltip("Filename used when the video is stored in persistentDataPath.")]
    public string fileName;

    [Header("UI")]
    public Texture2D thumbnail;
}