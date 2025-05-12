using UnityEngine;

[RequireComponent(typeof(AudioSource))]  // Garante que o AudioSource esteja presente
public class GunAudio : MonoBehaviour
{
    private AudioSource audioSource;  // Referência ao AudioSource

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Garantir que o pitch seja 1 no início
        if (audioSource != null)
        {
            audioSource.pitch = 1f; // Forçar o pitch para 1
        }
    }

    void Update()
    {
        // Garantir que o pitch seja 1 em tempo real, caso esteja sendo alterado
        if (audioSource != null && audioSource.pitch != 1f)
        {
            audioSource.pitch = 1f; // Forçar o pitch a 1
        }
    }
}
