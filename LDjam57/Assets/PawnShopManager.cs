using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PawnShopManager : MonoBehaviour
{
    [SerializeField] public Image artifactDisplay;
    [SerializeField] public Image yourPriceDisplay;
    [SerializeField] public Image realPriceDisplay;
    
    // public void Start()
    // {
    //    StartCoroutine(PawnShopCoroutine());
    // }
    //
    IEnumerator PawnShopCoroutine()
    {
        int i = 0;
        foreach (ArtifactData data in GameManager.Instance.playerInventory.artifacts)
        {
            if (data == null)
            {
                Debug.LogWarning($"ArtifactData is null. Skipping.");
                continue;
            }
            // show artifact dispaly and your price
            
            //wait a little
            
            // show real price
            
            //react
            
            // change currency according to the price
            
            //wait
            
            
        }
        
        yield return new WaitForSeconds(1f);
    }
}
