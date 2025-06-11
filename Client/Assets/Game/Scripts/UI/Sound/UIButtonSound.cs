using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [SerializeField]
    private string soundName = "sfx_common_button";
    [SerializeField]
    private bool PlaySound = true;
    private Button button;
    private void Awake()
    {
        if(PlaySound == false) {
            return;
        }
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayButtonSound);
        ResourceManager.Load<AudioClip>(soundName, (clip) => {
        });
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlayOneShot(soundName);
    }
}
