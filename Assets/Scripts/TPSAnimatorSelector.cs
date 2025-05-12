using UnityEngine;
using System.Collections;
using System.Linq;

public class TPSAnimatorSelector : MonoBehaviour
{
    [Header("Avatares de Referência")]
    public Avatar maleAvatar;
    public Avatar femaleAvatar;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("❌ Nenhum Animator encontrado neste GameObject.");
            return;
        }

        StartCoroutine(WaitAndApplyCorrectAvatar());
    }

    private IEnumerator WaitAndApplyCorrectAvatar()
    {
        // Espera até que o personagem certo esteja ativo
        Transform characterParent = transform.Find("CharacterParent");

        if (characterParent == null)
        {
            Debug.LogWarning("⚠️ 'CharacterParent' não encontrado.");
            yield break;
        }

        // Esperar até algum personagem estar ativo
        yield return new WaitUntil(() => characterParent.Cast<Transform>().Any(t => t.gameObject.activeInHierarchy));

        // Agora detecta o personagem ativo
        var activeCharacter = characterParent.Cast<Transform>().First(t => t.gameObject.activeInHierarchy);

        string characterName = activeCharacter.name.ToLower();

        // Aplica o avatar correto baseado no nome
        if (characterName.Contains("female"))
        {
            animator.avatar = femaleAvatar;
            Debug.Log("✅ Avatar feminino aplicado para: " + characterName);
        }
        else if (characterName.Contains("male"))
        {
            animator.avatar = maleAvatar;
            Debug.Log("✅ Avatar masculino aplicado para: " + characterName);
        }
        else
        {
            Debug.LogWarning($"❓ Nome de personagem não reconhecido: {characterName}. Nenhum avatar foi aplicado.");
        }
    }
}
