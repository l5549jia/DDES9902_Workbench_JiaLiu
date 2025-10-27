using UnityEngine;

public class TreeFeedback : MonoBehaviour
{
    public Renderer leaves;     
    public AudioSource birds;   
    public AudioSource wind;    
    public AudioSource chime;   

    public void UpdateGarden(float balance)
    {
        if (leaves)
        {
            Color dry = new Color(0.8f, 0.6f, 0.3f); 
            Color lush = new Color(0.3f, 0.8f, 0.4f); 
            leaves.material.color = Color.Lerp(dry, lush, balance);

            Vector3 small = Vector3.one * 0.8f;
            Vector3 big = Vector3.one * 1.2f;
            transform.localScale = Vector3.Lerp(small, big, balance);
        }

        if (birds) birds.volume = Mathf.Clamp01(balance);
        if (wind) wind.volume = Mathf.Clamp01(1f - balance);
    }

    public void FlowerBloom()
    {
        if (chime) chime.Play();
    }
}
