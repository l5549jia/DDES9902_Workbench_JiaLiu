using UnityEngine;

public class AlwaysPlayTrigger : MonoBehaviour
{
    public AudioSource chime;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (chime) chime.Play();
        }
    }
}
