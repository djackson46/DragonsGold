using UnityEngine; 
using System.Collections;
 
[RequireComponent (typeof (AudioSource))]
public class PlaySoundArray : MonoBehaviour {
     
    public float hSliderValue = 1.0f;
    public AudioClip[] clips;
    
    private bool playNow = true;
    private int cnt = 0;

    void Update () 
    {
        if( Input.GetKeyUp( KeyCode.M ) )
        {
            playNow = !playNow;
            audio.Stop();
        }
             
        if( playNow )
        {
            PlaySounds();
        } 
    }
 
    void PlaySounds()
    {
        if( !audio.isPlaying && cnt < clips.Length )
        {
            audio.clip = clips[cnt];
            audio.volume = hSliderValue;
            audio.Play();
            cnt = cnt + 1;
        }

        if( cnt == clips.Length ) cnt = 0;
    }
}